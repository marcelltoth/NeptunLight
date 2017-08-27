using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
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
                    DateTime receiveTime = DateTime.ParseExact(row.Cells[7].TextContent, "yyyy.MM.dd. HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
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
            throw new NotImplementedException();
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

        public async Task<IReadOnlyDictionary<Semester, IReadOnlyCollection<Subject>>> RefreshExamsAsnyc()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<SemesterData>> RefreshSemestersAsnyc()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<SemesterData>> RefreshPeriodsAsnyc()
        {
            throw new NotImplementedException();
        }
    }
}