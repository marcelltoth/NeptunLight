using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public interface INeptunInterface
    {
        Uri BaseUri { get; set; }
        [NotNull]
        string Username { get; set; }

        [NotNull]
        string Password { get; set; }

        Task LoginAsync();

        Task<BasicNeptunData> RefreshBasicDataAsync();
        IObservable<Mail> RefreshMessages(IProgress<MessageLoadingProgress> progress = null);
        Task<IReadOnlyCollection<CalendarEvent>> RefreshCalendarAsnyc();
        Task<IReadOnlyDictionary<Semester, IReadOnlyCollection<Subject>>> RefreshSubjectsAsnyc();
        Task<IReadOnlyDictionary<Semester, IReadOnlyCollection<Exam>>> RefreshExamsAsnyc();
        Task<IReadOnlyCollection<SemesterData>> RefreshSemestersAsnyc();
        Task<IReadOnlyCollection<Period>> RefreshPeriodsAsnyc();
    }
}