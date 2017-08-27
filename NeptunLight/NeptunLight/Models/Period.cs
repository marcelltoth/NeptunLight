using System;

namespace NeptunLight.Models
{
    public class Period
    {
        public Period(DateTime startTime, DateTime endTime, string type, string name)
        {
            StartTime = startTime;
            EndTime = endTime;
            Type = type;
            Name = name;
        }

        public DateTime StartTime { get; }
        
        public DateTime EndTime { get; }

        public string Type { get; }

        public string Name { get; }
    }
}