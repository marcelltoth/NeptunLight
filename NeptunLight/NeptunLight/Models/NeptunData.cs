using System.Collections.Generic;
using ReactiveUI;

namespace NeptunLight.Models
{
    public class NeptunData : ReactiveObject
    {
        private IReadOnlyCollection<Mail> _messages;

        public IReadOnlyCollection<Mail> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value);
        }

        private IReadOnlyCollection<CalendarEvent> _calendar;

        public IReadOnlyCollection<CalendarEvent> Calendar
        {
            get => _calendar;
            set => this.RaiseAndSetIfChanged(ref _calendar, value);
        }

        private IReadOnlyDictionary<Semester, IReadOnlyCollection<Subject>> _subjectsPerSemester;

        public IReadOnlyDictionary<Semester, IReadOnlyCollection<Subject>> SubjectsPerSemester
        {
            get => _subjectsPerSemester;
            set => this.RaiseAndSetIfChanged(ref _subjectsPerSemester, value);
        }

        private IReadOnlyDictionary<Semester, IReadOnlyCollection<Exam>> _examsPerSemester;

        public IReadOnlyDictionary<Semester, IReadOnlyCollection<Exam>> ExamsPerSemester
        {
            get => _examsPerSemester;
            set => this.RaiseAndSetIfChanged(ref _examsPerSemester, value);
        }

        private IReadOnlyCollection<SemesterData> _semesterInfo;

        public IReadOnlyCollection<SemesterData> SemesterInfo
        {
            get => _semesterInfo;
            set => this.RaiseAndSetIfChanged(ref _semesterInfo, value);
        }

        private IReadOnlyCollection<Period> _periods;

        public IReadOnlyCollection<Period> Periods
        {
            get => _periods;
            set => this.RaiseAndSetIfChanged(ref _periods, value);
        }
    }
}