using System;
using System.Collections.Generic;

namespace NeptunLight.Models
{
    public class Exam
    {
        public Exam(string subject, string course, string type, string attemptType, DateTime startTime, string location, IEnumerable<string> instructors, int placesTaken, int placesTotal, bool? shownUp, string result, string description)
        {
            Subject = subject;
            Course = course;
            Type = type;
            AttemptType = attemptType;
            StartTime = startTime;
            Location = location;
            Instructors = instructors;
            PlacesTaken = placesTaken;
            PlacesTotal = placesTotal;
            ShownUp = shownUp;
            Result = result;
            Description = description;
        }

        public string Subject { get; }

        public string Course { get; }

        public string Type { get; }

        public string AttemptType { get; }

        public DateTime StartTime { get; }

        public string Location { get; }

        public IEnumerable<string> Instructors { get; }

        public int PlacesTaken { get; }

        public int PlacesTotal { get; }

        public bool? ShownUp { get; }

        public string Result { get; }

        public string Description { get; }
    }
}