using System;
using NeptunLight.Models;

namespace NeptunLight.ViewModels
{
    public class PeriodViewModel : ViewModelBase
    {
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
    }
}