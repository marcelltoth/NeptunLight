using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using Xunit;

namespace NeptunLight.Tests
{
    public class WebNeptunInterfaceTests
    {
        private static WebNeptunInterface CreateInterface()
        {
            WebNeptunInterface iface = new WebNeptunInterface(null, null)
            {
                BaseUri = new Uri("https://neptun3r.web.uni-corvinus.hu/hallgatoi_2/"),
                Username = Environment.GetEnvironmentVariable("NEPTUN_USERNAME", EnvironmentVariableTarget.User),
                Password = Environment.GetEnvironmentVariable("NEPTUN_PASSWORD", EnvironmentVariableTarget.User)
            };
            return iface;
        }

        [Fact]
        public async void Login_WrongCredentials()
        {
            WebNeptunInterface client = new WebNeptunInterface(null, null)
            {
                BaseUri = new Uri("https://neptun3r.web.uni-corvinus.hu/hallgatoi_2/"),
                Username = "asd",
                Password = "123"
            };
            await Assert.ThrowsAnyAsync<UnauthorizedAccessException>(() => client.LoginAsync());
        }

        [Fact]
        public async void Login_CorrectCredentials()
        {
            INeptunInterface client = CreateInterface();
            await client.LoginAsync();
        }

        [Fact]
        public async void Messages_MoreThan200()
        {
            INeptunInterface client = CreateInterface();
            IList<Mail> messages = await client.RefreshMessages().ToList();
            Assert.InRange(messages.Count, 201, Int32.MaxValue);
        }

        [Fact]
        public async void Subjects_AtLeastTwoSemesters()
        {
            INeptunInterface client = CreateInterface();
            IReadOnlyDictionary<Semester, IReadOnlyCollection<Subject>> subjectData = await client.RefreshSubjectsAsnyc();
            Assert.InRange(subjectData.Count(kvp => kvp.Value.Any()), 2, Int32.MaxValue);
        }

        [Fact]
        public async void Exams_AtLeastTwoSemesters()
        {
            INeptunInterface client = CreateInterface();
            IReadOnlyDictionary<Semester, IReadOnlyCollection<Exam>> examData = await client.RefreshExamsAsnyc();
            Assert.InRange(examData.Count(kvp => kvp.Value.Any()), 2, Int32.MaxValue);
        }

        [Fact]
        public async void Calendar_HasEvents()
        {
            INeptunInterface client = CreateInterface();
            IReadOnlyCollection<CalendarEvent> events = await client.RefreshCalendarAsnyc();
            Assert.InRange(events.Count, 40, Int32.MaxValue);
        }

        [Fact]
        public async void Periods_HasData()
        {
            WebNeptunInterface client = CreateInterface();
            IReadOnlyCollection<Period> periods = await client.RefreshPeriodsAsnyc();
            Assert.InRange(periods.Count, 3, Int32.MaxValue);
        }

        [Fact]
        public async void Semesters_HasData()
        {
            WebNeptunInterface client = CreateInterface();
            IReadOnlyCollection<SemesterData> semesters = await client.RefreshSemestersAsnyc();
            Assert.InRange(semesters.Count, 1, 20);
        }
    }
}