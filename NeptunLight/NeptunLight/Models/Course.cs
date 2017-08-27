using System.Collections.Generic;

namespace NeptunLight.Models
{
    public class Course
    {
        public Course(string code, string type, int periodCount, string scheduleInfo, IEnumerable<string> instructors)
        {
            Code = code;
            Type = type;
            PeriodCount = periodCount;
            ScheduleInfo = scheduleInfo;
            Instructors = instructors;
        }

        public string Code { get; }

        public string Type { get; }

        public int PeriodCount { get; }

        public string ScheduleInfo { get; }

        public IEnumerable<string> Instructors { get; }
    }
}