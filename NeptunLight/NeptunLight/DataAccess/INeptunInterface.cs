using System.Collections.Generic;
using System.Threading.Tasks;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public interface INeptunInterface
    {
        Task LoginAsync();

        Task<IReadOnlyCollection<MailHeader>> RefreshMessagesAsnyc();
        Task<IReadOnlyCollection<CalendarEvent>> RefreshCalendarAsnyc();
        Task<IReadOnlyDictionary<Semester, Subject>> RefreshSubjectsAsnyc();
        Task<IReadOnlyDictionary<Semester, Exam>> RefreshExamsAsnyc();
        Task<IReadOnlyCollection<SemesterData>> RefreshSemestersAsnyc();
        Task<IReadOnlyCollection<SemesterData>> RefreshPeriodsAsnyc();
    }
}