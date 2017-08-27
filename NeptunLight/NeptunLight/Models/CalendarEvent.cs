using System;

namespace NeptunLight.Models
{
    public class CalendarEvent
    {
        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public string Location { get; }

        public string Title { get; }

        public string Details { get; }

        public string Group { get; }

        public string Instructor { get; }

        public string Category { get; }

        public CalendarEvent(DateTime startDate, DateTime endDate, string location, string category, string title, string group, string details = null, string instructor = null)
        {
            StartDate = startDate;
            EndDate = endDate;
            Location = location;
            Category = category;
            Title = title;
            Group = group;
            Details = details;
            Instructor = instructor;
        }
    }
}