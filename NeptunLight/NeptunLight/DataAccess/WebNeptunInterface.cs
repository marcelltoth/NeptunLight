using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using JetBrains.Annotations;
using NeptunLight.Helpers;
using NeptunLight.Models;
using NeptunLight.Services;
using Newtonsoft.Json.Linq;
using NodaTime;
using Period = NeptunLight.Models.Period;


namespace NeptunLight.DataAccess
{
    public class WebNeptunInterface : INeptunInterface
    {
        private const string USERNAME_SETTING_KEY = "NEPTUN_USERNAME";
        private const string PASSWORD_SETTING_KEY = "NEPTUN_PASSWORD";
        private const string BASE_URL_SETTING_KEY = "NEPTUN_BASEURL";

        [CanBeNull]
        private readonly IMailContentCache _mailContentCache;

        [CanBeNull]
        private readonly IPrimitiveStorage _primitiveStorage;

        private readonly Func<WebScraperClient> _clientFactory;

        private Uri _baseUri;
        private WebScraperClient _client;
        private string _password;

        private string _username;

        public WebNeptunInterface([CanBeNull] IMailContentCache mailContentCache, [CanBeNull] IPrimitiveStorage primitiveStorage, Func<WebScraperClient> clientFactory)
        {
            _mailContentCache = mailContentCache;
            _primitiveStorage = primitiveStorage;
            _clientFactory = clientFactory;
        }

        public string Username
        {
            get => _username ?? (_primitiveStorage != null && _primitiveStorage.ContainsKey(USERNAME_SETTING_KEY) ? _primitiveStorage.GetString(USERNAME_SETTING_KEY) : String.Empty);
            set
            {
                _username = value;
                _primitiveStorage?.PutString(USERNAME_SETTING_KEY, value);
            }
        }

        public string Password
        {
            get => _password ?? (_primitiveStorage != null && _primitiveStorage.ContainsKey(PASSWORD_SETTING_KEY) ? _primitiveStorage.GetString(PASSWORD_SETTING_KEY) : String.Empty);
            set
            {
                _password = value;
                _primitiveStorage?.PutString(PASSWORD_SETTING_KEY, value);
            }
        }

        public Uri BaseUri
        {
            get
            {
                if (_baseUri != null)
                    return _baseUri;
                if (_primitiveStorage != null && _primitiveStorage.ContainsKey(BASE_URL_SETTING_KEY))
                    return new Uri(_primitiveStorage.GetString(BASE_URL_SETTING_KEY));
                return default(Uri);
            }
            set
            {
                _baseUri = value;
                _primitiveStorage?.PutString(BASE_URL_SETTING_KEY, value.AbsoluteUri);
            }
        }

        public async Task LoginAsync()
        {
            try
            {
                _client = _clientFactory();
                _client.BaseUri = BaseUri;
                await _client.PostJsonObjectAsnyc("Login.aspx/GetMaxTryNumber", "");
                JObject r2 = await _client.PostJsonObjectAsnyc("Login.aspx/CheckLoginEnable", $"{{user: \"{Username}\", pwd: \"{Password}\", UserLogin: null, GUID: null, captcha: \"\"}}");
                JObject loginResult = JObject.Parse(r2.Value<string>("d"));
                if (!string.Equals(loginResult.Value<string>("success"), "True", StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException();
                await _client.PostJsonObjectAsnyc("Login.aspx/SavePopupState", "{state: \"hidden\", PopupID: \"upLoginWait_popupLoginWait\"}");
            }
            catch (FormatException) // UriFormatException - missing from fkin pcl
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception exc) when (!(exc is UnauthorizedAccessException))
            {
                throw new NetworkException("Error loading neptun", exc);
            }
        }

        public async Task<BasicNeptunData> RefreshBasicDataAsync()
        {
            await LoginAsync();
            IDocument mainPage = await _client.GetDocumentAsnyc("main.aspx");
            string[] topNameParts = mainPage.GetElementById("upTraining_topname").TextContent.Split('-');
            string trainingName = mainPage.GetElementById("lblTrainingName").TextContent.Split('-').First().Trim();
            return new BasicNeptunData(topNameParts.First().Trim(), topNameParts.Last().Trim(), trainingName);
        }

        public IObservable<Mail> RefreshMessages(IProgress<MessageLoadingProgress> progress = null)
        {
            return Observable.Create<Mail>(async (observer, ct) =>
            {
                await LoginAsync();
                IDocument inboxPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=inbox", ct);
                string rawMessages = await _client.GetRawAsnyc("HandleRequest.ashx?RequestType=GetData&GridID=c_messages_gridMessages&pageindex=1&pagesize=500&sort1=SendDate%20DESC&sort2=&fixedheader=false&searchcol=&searchtext=&searchexpanded=false&allsubrowsexpanded=False&selectedid=undefined&functionname=&level=", ct);
                HtmlParser parser = new HtmlParser();
                IDocument rawMessagesDocument = await parser.ParseAsync(rawMessages, ct);
                IHtmlTableElement messageHeaderTable = (IHtmlTableElement) rawMessagesDocument.GetElementById("c_messages_gridMessages_bodytable");

                IList<MailHeader> mailHeaders = ParseMailHeaderTable(messageHeaderTable);
                int messagesToLoad = Math.Min(mailHeaders.Count, 100);
                for (int i = 0; i < messagesToLoad; i++)
                {
                    if (ct.IsCancellationRequested)
                    {
                        observer.OnCompleted();
                        return;
                    }
                    MailHeader mailHeader = mailHeaders[i];

                    progress?.Report(new MessageLoadingProgress(i + 1, messagesToLoad));

                    await Task.Run(async () =>
                    {
                        try
                        {
                            if (_mailContentCache != null)
                            {
                                // try to load the message body from the cache
                                Mail ret = await _mailContentCache.TryRetrieveAsync(mailHeader);
                                if (ret != null)
                                {
                                    observer.OnNext(ret);
                                    return;
                                }
                            }

                            // load the mail itself
                            await Task.Delay(50, ct);
                            IDocument popupDocument = await _client.PostFormAsnyc(
                                "main.aspx?ismenuclick=true&ctrl=inbox",
                                inboxPage,
                                new[]
                                {
                                    new KeyValuePair<string, string>("__EVENTTARGET", "upFunction$c_messages$upMain$upGrid$gridMessages"),
                                    new KeyValuePair<string, string>("__EVENTARGUMENT", $"commandname=Subject;commandsource=select;id={mailHeader.TrId};level=1")
                                },
                                false, ct);

                            string content = popupDocument.GetElementById("Readmessage1_lblMessage").InnerHtml;
                            Mail ret2 = new Mail(mailHeader, content);

                            if (_mailContentCache != null)
                                await _mailContentCache.StoreAsync(mailHeader, ret2);

                            observer.OnNext(ret2);
                        }
                        catch (Exception e)
                        {
                            // Do not crash if a single mail throws
                            // TODO: implement logging
                        }
                    }, ct);
                }

                observer.OnCompleted();
            });
        }

        public async Task<IReadOnlyCollection<CalendarEvent>> RefreshCalendarAsnyc()
        {
            await LoginAsync();
            IDocument exportPage = await _client.GetDocumentAsnyc("main.aspx?ctrl=0104");
            string majorId = exportPage.GetElementById("calexport_cmbTraining").Children[0].GetAttribute("value");
            await _client.PostJsonObjectAsnyc("main.aspx/GetICS", $"{{\"ID\":\"1_1_0_1_0_0_1\",\"fromDate\":\"{DateTime.Today.AddYears(-1):yyyy.MM.dd}\",\"toDate\":\"{DateTime.Today.AddYears(1):yyyy.MM.dd}\",\"trainingId\":\"{majorId}\"}}");
            string ics = await _client.GetRawAsnyc($"CommonControls/SaveFileDialog.aspx?id=1_1_0_1_0_0_1&Func=exportcalendar&from={DateTime.Today.AddYears(-1):yyyy.MM.dd}&to={DateTime.Today.AddYears(1):yyyy.MM.dd}&trainingid={majorId}");
            if(!ics.StartsWith("BEGIN:VCALENDAR")) // in case of an error - for example  the selected range does not contain any events - we will get a html result instead.
                return new List<CalendarEvent>();
            return Ical.Net.Calendar.Load(ics).Events.Select(ice =>
            {
                string[] summaryParts = ice.Summary.Split(new[] {" - "}, StringSplitOptions.None);
                string title = summaryParts[0];
                if (title.Contains("("))
                    title = title.Substring(0, summaryParts[0].IndexOf('(')).Trim();
                string details = summaryParts[0].Replace(title, "").Trim();
                var timezone = DateTimeZoneProviders.Tzdb["Europe/Budapest"];
                return new CalendarEvent(Instant.FromDateTimeUtc(ice.DtStart.AsUtc).InZone(timezone).ToDateTimeUnspecified(), Instant.FromDateTimeUtc(ice.DtEnd.AsUtc).InZone(timezone).ToDateTimeUnspecified(), ice.Location, summaryParts.Last(), title, summaryParts[1], details, summaryParts.Length > 3 ? summaryParts[2] : null);
            }).ToList();
        }

        public async Task<IReadOnlyDictionary<Semester, IReadOnlyCollection<Subject>>> RefreshSubjectsAsnyc()
        {
            await LoginAsync();
            Dictionary<Semester, IReadOnlyCollection<Subject>> result = new Dictionary<Semester, IReadOnlyCollection<Subject>>();

            IDocument subjectsPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=0304");
            IEnumerable<IElement> semesterOptions = subjectsPage.GetElementById("cmb_cmb").Children.Where(opt => opt.GetAttribute("value") != "-1");
            foreach (IElement option in semesterOptions)
            {
                await Task.Delay(200);
                Semester semester = Semester.Parse(option.TextContent);
                List<Subject> subjectList = new List<Subject>();

                // load subcject data in semester
                subjectsPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=0304");
                await Task.Delay(100);
                IDocument semesterSubjectData = await _client.PostFormAsnyc("main.aspx?ismenuclick=true&ctrl=0304", subjectsPage, new[] {new KeyValuePair<string, string>("upFilter$cmb$m_cmb", option.GetAttribute("value"))});
                IHtmlTableElement subjectDataTable = (IHtmlTableElement) semesterSubjectData.GetElementById("h_addedsubjects_gridAddedSubjects_bodytable");

                // load course data

                await Task.Delay(200);
                IDocument coursesPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=0302");
                await Task.Delay(100);
                IElement optionToSelect = coursesPage.GetElementById("cmb_cmb").Children.FirstOrDefault(e => e.TextContent.StartsWith(semester.Name));
                if (optionToSelect == null)
                    continue;
                IDocument semesterCourseData = await _client.PostFormAsnyc("main.aspx?ismenuclick=true&ctrl=0302", coursesPage, new[] {new KeyValuePair<string, string>("upFilter$cmb$m_cmb", optionToSelect.GetAttribute("value"))});
                IHtmlTableElement courseDataTable = (IHtmlTableElement) semesterCourseData.GetElementById("h_actual_courses_gridCourses_bodytable");

                foreach (IHtmlTableRowElement dataRow in subjectDataTable.Bodies[0].Rows)
                {
                    if (dataRow.ClassList.Contains("NoMatch"))
                        continue;
                    string subjectCode = dataRow.Cells[1].TextContent;
                    string subjectName = dataRow.Cells[2].TextContent;
                    int creditCount = Int32.Parse(dataRow.Cells[3].TextContent);
                    int attemptCount = Int32.Parse(dataRow.Cells[4].TextContent);
                    IEnumerable<Course> courses = courseDataTable.Bodies[0].Rows.Where(r => r.Cells.Length >= 8 && r.Cells[1].TextContent == subjectCode).Select(r =>
                    {
                        string courseCode = r.Cells[3].TextContent;
                        string courseType = r.Cells[4].TextContent;
                        int periodCount;
                        if (!Int32.TryParse(r.Cells[5].TextContent, out periodCount))
                            periodCount = 1;
                        string scheduleInfo = r.Cells[6].TextContent;
                        IEnumerable<string> instructors = r.Cells[7].TextContent.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
                        return new Course(courseCode, courseType, periodCount, scheduleInfo, instructors);
                    });

                    subjectList.Add(new Subject(subjectCode, subjectName, creditCount, attemptCount, courses));
                }

                result.Add(semester, subjectList);
            }

            return result;
        }

        public async Task<IReadOnlyDictionary<Semester, IReadOnlyCollection<Exam>>> RefreshExamsAsnyc()
        {
            await LoginAsync();
            Dictionary<Semester, IReadOnlyCollection<Exam>> result = new Dictionary<Semester, IReadOnlyCollection<Exam>>();

            IDocument examsPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=0402");
            IEnumerable<IElement> semesterOptions = examsPage.GetElementById("upFilter_cmbTerms").Children.Where(opt => opt.GetAttribute("value") != "-1");
            foreach (IElement option in semesterOptions.Take(8))
            {
                try
                {
                    Semester semester = Semester.Parse(option.TextContent);
                    if (semester.IsFarFuture)
                        continue;

                    List<Exam> examList = new List<Exam>();

                    // load exam data in semester
                    await Task.Delay(200);
                    examsPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=0402");
                    await Task.Delay(100);
                    IDocument semesterExamData = await _client.PostFormAsnyc("main.aspx?ismenuclick=true&ctrl=0402", examsPage, new[] {new KeyValuePair<string, string>("upFilter$cmbTerms", option.GetAttribute("value"))});
                    IHtmlTableElement subjectDataTable = (IHtmlTableElement) semesterExamData.GetElementById("h_signedexams_gridExamList_bodytable");

                    foreach (IHtmlTableRowElement dataRow in subjectDataTable.Bodies[0].Rows)
                    {
                        if (dataRow.ClassList.Contains("NoMatch"))
                            continue;
                        string subject = dataRow.Cells[1].GetFirstLineOfText();
                        string course = dataRow.Cells[3].GetFirstLineOfText();
                        string type = dataRow.Cells[4].GetFirstLineOfText();
                        string attemptType = dataRow.Cells[5].GetFirstLineOfText();
                        string startTimeText = dataRow.Cells[6].GetFirstLineOfText();
                        DateTime startTime = DateTime.ParseExact(startTimeText, "yyyy.MM.dd. H:mm:ss", DateTimeFormatInfo.InvariantInfo);
                        string location = dataRow.Cells[7].GetFirstLineOfText();
                        IEnumerable<string> instructors = dataRow.Cells[8].GetFirstLineOfText().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
                        string[] placeCountParts = dataRow.Cells[9].GetFirstLineOfText().Split(' ')[0].Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                        int placesTaken = Int32.Parse(placeCountParts[0]);
                        int placesTotal = placeCountParts.Length > 1 ? Int32.Parse(placeCountParts[1]) : 0;
                        bool? shownUp = null;
                        var shownUpImage = dataRow.Cells[12].Children.FirstOrDefault(e => String.Equals(e.TagName, "img", StringComparison.OrdinalIgnoreCase));
                        if (shownUpImage != null)
                        {
                            string imageName = shownUpImage.GetAttribute("src").Split('/').Last();
                            if (imageName.StartsWith("ok"))
                                shownUp = true;
                            else if (imageName.StartsWith("no"))
                                shownUp = false;
                        }

                        string examResult = dataRow.Cells[14].GetFirstLineOfText();
                        string description = dataRow.Cells[15].GetFirstLineOfText();

                        examList.Add(new Exam(subject, course, type, attemptType, startTime, location, instructors, placesTaken, placesTotal, shownUp, examResult, description));
                    }

                    result.Add(semester, examList);
                }
                catch (Exception ex)
                {
                    // don't fail the whole sync if one semester load fails
                }
            }

            return result;
        }

        public async Task<IReadOnlyCollection<SemesterData>> RefreshSemestersAsnyc()
        {
            await LoginAsync();
            IDocument semestersPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=0205");
            IHtmlTableElement dataTable = (IHtmlTableElement) semestersPage.GetElementById("h_officialnote_average_gridAverages_bodytable");

            List<SemesterData> results = new List<SemesterData>();
            for (int i = 0; i < dataTable.Bodies[0].Rows.Length - 1; i += 2)
            {
                IHtmlTableRowElement headerRow = dataTable.Bodies[0].Rows[i];
                //IHtmlTableRowElement dataRow = dataTable.Bodies[0].Rows[i+1];

                Semester semester = Semester.Parse(headerRow.Cells[1].TextContent);
                string status = headerRow.Cells[2].TextContent;
                string financialStatus = headerRow.Cells[3].TextContent;
                int? creditsAccpomlished = !string.IsNullOrEmpty(headerRow.Cells[4].TextContent) ? Int32.Parse(headerRow.Cells[4].TextContent) : (int?) null;
                int? creditsTaken = !string.IsNullOrEmpty(headerRow.Cells[5].TextContent) ? Int32.Parse(headerRow.Cells[5].TextContent) : (int?) null;
                int? totalCreditsAccpomlished = !string.IsNullOrEmpty(headerRow.Cells[6].TextContent) ? Int32.Parse(headerRow.Cells[6].TextContent) : (int?) null;
                int? totalCreditsTaken = !string.IsNullOrEmpty(headerRow.Cells[7].TextContent) ? Int32.Parse(headerRow.Cells[7].TextContent) : (int?) null;
                double? average = !string.IsNullOrEmpty(headerRow.Cells[8].GetFirstLineOfText()) ? Double.Parse(headerRow.Cells[8].GetFirstLineOfText(), new CultureInfo("hu-HU")) : (double?) null;
                double? cumAverage = !string.IsNullOrEmpty(headerRow.Cells[9].GetFirstLineOfText()) ? Double.Parse(headerRow.Cells[9].GetFirstLineOfText(), new CultureInfo("hu-HU")) : (double?) null;
                results.Add(new SemesterData(semester, status, financialStatus, creditsAccpomlished, creditsTaken, totalCreditsAccpomlished, totalCreditsTaken, average, cumAverage));
            }
            return results;
        }

        public async Task<IReadOnlyCollection<Period>> RefreshPeriodsAsnyc()
        {
            await LoginAsync();
            List<Period> result = new List<Period>();

            IDocument periodsPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=1301");
            IEnumerable<IElement> semesterOptions = periodsPage.GetElementById("upFilter_cmbTerms").Children.Where(opt => opt.GetAttribute("value") != "-1");
            foreach (IElement option in semesterOptions.Take(8))
                try
                {
                    Semester semester = Semester.Parse(option.TextContent);
                    if (semester.IsFarFuture)
                        continue;

                    // load period data in semester
                    await Task.Delay(200);
                    periodsPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=1301");
                    await Task.Delay(100);
                    IDocument semesterExamData = await _client.PostFormAsnyc("main.aspx?ismenuclick=true&ctrl=1301", periodsPage, new[] {new KeyValuePair<string, string>("upFilter$cmbTerms", option.GetAttribute("value"))});
                    IHtmlTableElement subjectDataTable = (IHtmlTableElement) semesterExamData.GetElementById("h_seasons_gridSeasons_bodytable");

                    foreach (IHtmlTableRowElement dataRow in subjectDataTable.Bodies[0].Rows)
                    {
                        if (dataRow.ClassList.Contains("NoMatch"))
                            continue;
                        DateTime startTime = DateTime.ParseExact(dataRow.Cells[1].TextContent, "yyyy.MM.dd. H:mm:ss", DateTimeFormatInfo.InvariantInfo);
                        DateTime endTime = DateTime.ParseExact(dataRow.Cells[2].TextContent, "yyyy.MM.dd. H:mm:ss", DateTimeFormatInfo.InvariantInfo);
                        string type = dataRow.Cells[3].TextContent;
                        string name = dataRow.Cells[4].TextContent;

                        result.Add(new Period(startTime, endTime, type, name));
                    }
                }
                catch (FormatException)
                {
                    // e.g. "Kérem válasszon" semester
                }
                catch (Exception ex)
                {
                    throw new NetworkException("Error loading periods", ex);
                }

            return result;
        }

        private static IList<MailHeader> ParseMailHeaderTable(IHtmlTableElement table)
        {
            List<MailHeader> result = new List<MailHeader>();
            foreach (IHtmlTableRowElement row in table.Bodies[0].Rows)
                try
                {
                    DateTime receiveTime = DateTime.ParseExact(row.Cells[7].TextContent, "yyyy.MM.dd. H:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    string sender = row.Cells[4].TextContent;
                    string title = row.Cells[6].TextContent;
                    long trid = Int64.Parse(row.Id.Substring(4));
                    result.Add(new MailHeader(receiveTime, sender, title) {TrId = trid});
                }
                catch (Exception)
                {
                    // skip unparsable stuff
                }
            return result;
        }
    }
}