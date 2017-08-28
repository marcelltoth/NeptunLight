using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using iCal.PCL.DataModel;
using iCal.PCL.Serialization;
using NeptunLight.Models;
using NeptunLight.Services;
using Newtonsoft.Json.Linq;

namespace NeptunLight.DataAccess
{
    public class WebNeptunInterface : INeptunInterface
    {
        private WebScraperClient _client;

        public WebNeptunInterface(string username, string password, Uri baseUri)
        {
            Username = username;
            Password = password;
            BaseUri = baseUri;
        }

        public string Username { get; }

        public string Password { get; }

        public Uri BaseUri { get; }

        public async Task LoginAsync()
        {
            _client = new WebScraperClient
            {
                BaseUri = BaseUri
            };

            try
            {
                JObject r1 = await _client.PostJsonObjectAsnyc("Login.aspx/GetMaxTryNumber", "");
                JObject r2 = await _client.PostJsonObjectAsnyc("Login.aspx/CheckLoginEnable", $"{{user: \"{Username}\", pwd: \"{Password}\", UserLogin: null, GUID: null, captcha: \"\"}}");
                JObject loginResult = JObject.Parse(r2.Value<string>("d"));
                if (!string.Equals(loginResult.Value<string>("success"), "True", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException();
                }
                JObject r3 = await _client.PostJsonObjectAsnyc("Login.aspx/SavePopupState", "{state: \"hidden\", PopupID: \"upLoginWait_popupLoginWait\"}");
            }
            catch (Exception exc) when (!(exc is UnauthorizedAccessException))
            {
                throw new NetworkException("Error loading neptun", exc);
            }
        }

        public async Task<IReadOnlyCollection<Mail>> RefreshMessagesAsnyc(IMailContentCache contentCache = null)
        {
            await LoginAsync();
            IDocument inboxPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=inbox");
            string rawMessages = await _client.GetRawAsnyc("HandleRequest.ashx?RequestType=GetData&GridID=c_messages_gridMessages&pageindex=1&pagesize=500&sort1=SendDate%20DESC&sort2=&fixedheader=false&searchcol=&searchtext=&searchexpanded=false&allsubrowsexpanded=False&selectedid=undefined&functionname=&level=");
            HtmlParser parser = new HtmlParser();
            IDocument rawMessagesDocument = await parser.ParseAsync(rawMessages);
            IHtmlTableElement messageHeaderTable = (IHtmlTableElement)rawMessagesDocument.GetElementById("c_messages_gridMessages_bodytable");

            List<Mail> result = new List<Mail>();
            foreach (MailHeader mailHeader in ParseMailHeaderTable(messageHeaderTable))
            {
                Mail ret;
                if (contentCache != null && await contentCache.TryRetrieveAsync(mailHeader, out ret))
                {
                    result.Add(ret);
                    continue;
                }

                // load the mail itself
                await Task.Delay(50);
                IDocument popupDocument = await _client.PostFormAsnyc(
                    "main.aspx?ismenuclick=true&ctrl=inbox",
                    inboxPage,
                    new[]
                    {
                        new KeyValuePair<string, string>("__EVENTTARGET", "upFunction$c_messages$upMain$upGrid$gridMessages"),
                        new KeyValuePair<string, string>("__EVENTARGUMENT", $"commandname=Subject;commandsource=select;id={mailHeader.TrId};level=1"),
                    },
                    false);
                
                string content = popupDocument.GetElementById("Readmessage1_lblMessage").InnerHtml;
                ret = new Mail(mailHeader, content);

                if (contentCache != null)
                {
                    await contentCache.StoreAsync(mailHeader, ret);
                }

                result.Add(ret);

            }

            return result;
        }

        private IEnumerable<MailHeader> ParseMailHeaderTable(IHtmlTableElement table)
        {
            List<MailHeader> result = new List<MailHeader>();
            foreach (IHtmlTableRowElement row in table.Bodies[0].Rows)
            {
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
            }
            return result;
        }

        public async Task<IReadOnlyCollection<CalendarEvent>> RefreshCalendarAsnyc()
        {
            await LoginAsync();
            IDocument exportPage = await _client.GetDocumentAsnyc("main.aspx?ctrl=0104");
            string majorId = exportPage.GetElementById("calexport_cmbTraining").Children[0].GetAttribute("value");
            await _client.PostJsonObjectAsnyc("main.aspx/GetICS", $"{{\"ID\":\"1_1_0_1_0_0_1\",\"fromDate\":\"{DateTime.Today.AddYears(-1):yyyy.MM.dd}\",\"toDate\":\"{DateTime.Today.AddYears(1):yyyy.MM.dd}\",\"trainingId\":\"{majorId}\"}}");
            string ics = await _client.GetRawAsnyc($"CommonControls/SaveFileDialog.aspx?id=1_1_0_1_0_0_1&Func=exportcalendar&from={DateTime.Today.AddYears(-1):yyyy.MM.dd}&to={DateTime.Today.AddYears(1):yyyy.MM.dd}&trainingid={majorId}");
            IEnumerable<iCalVEvent> events = iCalSerializer.Deserialize(ics.Split('\n').Select(line => line.TrimEnd('\r'))).Cast<iCalVEvent>();
            return events.Select(ice =>
            {
                string[] summaryParts = ice.Summary.Split(new[] {" - "}, StringSplitOptions.None);
                string title = summaryParts[0].Substring(summaryParts[0].IndexOf('(')).Trim();
                string details = summaryParts[0].Replace(title, "").Trim();
                return new CalendarEvent(ice.DTStart, ice.DTEnd, ice.Location, summaryParts.Last(), title, summaryParts[1], details, summaryParts.Length > 3 ? summaryParts[2] : null);
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
                IDocument semesterSubjectData = await _client.PostFormAsnyc("main.aspx?ismenuclick=true&ctrl=0304", subjectsPage, new[]{new KeyValuePair<string, string>("upFilter$cmb$m_cmb", option.GetAttribute("value"))});
                IHtmlTableElement subjectDataTable = (IHtmlTableElement)semesterSubjectData.GetElementById("h_addedsubjects_gridAddedSubjects_bodytable");

                // load course data

                await Task.Delay(200);
                IDocument coursesPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=0302");
                await Task.Delay(100);
                IElement optionToSelect = coursesPage.GetElementById("cmb_cmb").Children.FirstOrDefault(e => e.TextContent == semester.Name);
                if (optionToSelect == null)
                {
                    continue;
                }
                IDocument semesterCourseData = await _client.PostFormAsnyc("main.aspx?ismenuclick=true&ctrl=0302", coursesPage, new[] { new KeyValuePair<string, string>("upFilter$cmb$m_cmb", optionToSelect.GetAttribute("value")) });
                IHtmlTableElement courseDataTable = (IHtmlTableElement)semesterCourseData.GetElementById("h_actual_courses_gridCourses_bodytable");

                foreach (IHtmlTableRowElement dataRow in subjectDataTable.Bodies[0].Rows)
                {
                    string subjectCode = dataRow.Cells[1].TextContent;
                    string subjectName = dataRow.Cells[2].TextContent;
                    int creditCount = Int32.Parse(dataRow.Cells[3].TextContent);
                    int attemptCount = Int32.Parse(dataRow.Cells[4].TextContent);
                    IEnumerable<Course> courses = courseDataTable.Bodies[0].Rows.Where(r => r.Cells[1].TextContent == subjectCode).Select(r =>
                    {
                        string courseCode = r.Cells[2].TextContent;
                        string courseType = r.Cells[3].TextContent;
                        int periodCount;
                        if (!Int32.TryParse(r.Cells[4].TextContent, out periodCount))
                            periodCount = 1;
                        string scheduleInfo = r.Cells[5].TextContent;
                        IEnumerable<string> instructors = r.Cells[6].TextContent.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
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
                Semester semester = Semester.Parse(option.TextContent);
                if (semester.IsFarFuture)
                    continue;

                List<Exam> examList = new List<Exam>();

                // load exam data in semester
                await Task.Delay(200);
                examsPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=0402");
                await Task.Delay(100);
                IDocument semesterExamData = await _client.PostFormAsnyc("main.aspx?ismenuclick=true&ctrl=0402", examsPage, new[] { new KeyValuePair<string, string>("upFilter$cmbTerms", option.GetAttribute("value")) });
                IHtmlTableElement subjectDataTable = (IHtmlTableElement)semesterExamData.GetElementById("h_signedexams_gridExamList_bodytable");
                
                foreach (IHtmlTableRowElement dataRow in subjectDataTable.Bodies[0].Rows)
                {
                    if (dataRow.ClassList.Contains("NoMatch"))
                        continue;
                    string subject = dataRow.Cells[1].TextContent;
                    string course = dataRow.Cells[3].TextContent;
                    string type = dataRow.Cells[4].TextContent;
                    string attemptType = dataRow.Cells[5].TextContent;
                    DateTime startTime = DateTime.ParseExact(dataRow.Cells[6].TextContent, "yyyy.MM.dd. H:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    string location = dataRow.Cells[7].TextContent;
                    IEnumerable<string> instructors = dataRow.Cells[8].TextContent.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
                    string[] placeCountParts = dataRow.Cells[9].TextContent.Trim().Split(' ')[0].Split('/');
                    int placesTaken = Int32.Parse(placeCountParts[0]);
                    int placesTotal = placeCountParts.Length > 1 ? Int32.Parse(placeCountParts[1]) : 0;
                    bool? shownUp = null;
                    if (dataRow.Cells[11].Children.Any(e => string.Equals(e.TagName, "img", StringComparison.OrdinalIgnoreCase)))
                    {
                        string imageName = dataRow.Cells[11].Children.First(e => string.Equals(e.TagName, "img", StringComparison.OrdinalIgnoreCase)).GetAttribute("src").Split('/').Last();
                        if (imageName.StartsWith("ok"))
                            shownUp = true;
                        else if (imageName.StartsWith("no"))
                            shownUp = false;
                    }

                    string examResult = dataRow.Cells[12].TextContent;
                    string description = dataRow.Cells[13].TextContent;

                    examList.Add(new Exam(subject, course, type, attemptType, startTime, location, instructors, placesTaken, placesTotal, shownUp, examResult, description));
                }

                result.Add(semester, examList);
            }

            return result;
        }

        public async Task<IReadOnlyCollection<SemesterData>> RefreshSemestersAsnyc()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<Period>> RefreshPeriodsAsnyc()
        {
            await LoginAsync();
            List<Period> result = new List<Period>();

            IDocument periodsPage = await _client.GetDocumentAsnyc("main.aspx?ismenuclick=true&ctrl=1301");
            IEnumerable<IElement> semesterOptions = periodsPage.GetElementById("upFilter_cmbTerms").Children.Where(opt => opt.GetAttribute("value") != "-1");
            foreach (IElement option in semesterOptions.Take(8))
            {
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
            }

            return result;
        }
    }
}