using System;
using NeptunLight.Models;

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

        public int ResultLevel
        {
            get
            {
                switch (Result.ToLower())
                {
                    case "jeles":
                        return 5;
                    case "jó":
                        return 4;
                    case "közepes":
                        return 3;
                    case "elégséges":
                        return 2;
                    case "elégtelen":
                        return 1;
                    default:
                        return 0;
                }
            }
        }
    }
}