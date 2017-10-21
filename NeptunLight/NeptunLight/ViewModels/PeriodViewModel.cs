using System;
using NeptunLight.Models;

namespace NeptunLight.ViewModels
{
    public class PeriodViewModel : ViewModelBase
    {
        public PeriodGroup Group
        {
            get
            {
                if (Type.ToLower().Contains("beiratkoz") || Type.ToLower().Contains("beiratkoz"))
                    return PeriodGroup.Registration;
                if (Type.ToLower().Contains("kurzusjelentkez") || Type.ToLower().Contains("tárgyjelentkez"))
                    return PeriodGroup.CourseSelection;

                return PeriodGroup.Other;
            }
        }

        public string Type { get; }

        public string Name { get; }

        public DateTime StartTime { get; }

        public DateTime EndTime { get; }

        public PeriodViewModel(Period model)
        {
            Type = model.Type;
            Name = model.Name;
            StartTime = model.StartTime;
            EndTime = model.EndTime;
        }

        public enum PeriodGroup
        {
            Registration,
            CourseSelection,
            Other
        }
    }

}