using System;
using NeptunLight.Models;

namespace NeptunLight.ViewModels
{
    public class CourseViewModel : ViewModelBase
    {
        public CourseViewModel(Course model)
        {
            Code = model.Code;
            Type = model.Type;
            ScheduleInfo = model.ScheduleInfo;
            Instructors = String.Join(", ", model.Instructors);
        }

        public string Code { get; }

        public string Type { get; }

        public string ScheduleInfo { get; }

        public string Instructors { get; }
    }
}