using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveUI;

namespace NeptunLight.Models
{
    public class NeptunData : ReactiveObject
    {
        [NotNull]
        private IReadOnlyCollection<CalendarEvent> _calendar = new List<CalendarEvent>();

        [NotNull]
        private IReadOnlyDictionary<Semester, IReadOnlyCollection<Exam>> _examsPerSemester = new Dictionary<Semester, IReadOnlyCollection<Exam>>();

        [NotNull]
        private IReadOnlyCollection<Mail> _messages = new List<Mail>();

        [NotNull]
        private IReadOnlyCollection<Period> _periods = new List<Period>();

        [NotNull]
        private IReadOnlyCollection<SemesterData> _semesterInfo = new List<SemesterData>();

        [NotNull]
        private IReadOnlyDictionary<Semester, IReadOnlyCollection<Subject>> _subjectsPerSemester = new Dictionary<Semester, IReadOnlyCollection<Subject>>();

        [NotNull]
        public IReadOnlyCollection<Mail> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value);
        }

        [NotNull]
        public IReadOnlyCollection<CalendarEvent> Calendar
        {
            get => _calendar;
            set => this.RaiseAndSetIfChanged(ref _calendar, value);
        }

        [NotNull]
        public IReadOnlyDictionary<Semester, IReadOnlyCollection<Subject>> SubjectsPerSemester
        {
            get => _subjectsPerSemester;
            set => this.RaiseAndSetIfChanged(ref _subjectsPerSemester, value);
        }

        [NotNull]
        public IReadOnlyDictionary<Semester, IReadOnlyCollection<Exam>> ExamsPerSemester
        {
            get => _examsPerSemester;
            set => this.RaiseAndSetIfChanged(ref _examsPerSemester, value);
        }

        [NotNull]
        public IReadOnlyCollection<SemesterData> SemesterInfo
        {
            get => _semesterInfo;
            set => this.RaiseAndSetIfChanged(ref _semesterInfo, value);
        }

        [NotNull]
        public IReadOnlyCollection<Period> Periods
        {
            get => _periods;
            set => this.RaiseAndSetIfChanged(ref _periods, value);
        }
    }
}