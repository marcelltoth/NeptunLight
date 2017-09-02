using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class ExamViewModel : ViewModelBase
    {
        public ExamViewModel(Exam model)
        {
            Subject = model.Subject;
            StartTime = model.StartTime;
            Location = model.Location;
            Result = model.Result;
        }

        public string Subject { get; }

        public DateTime StartTime { get; }

        public string Location { get; }

        public string Result { get; }
    }
}