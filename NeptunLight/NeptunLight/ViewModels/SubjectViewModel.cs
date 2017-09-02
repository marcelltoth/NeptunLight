using System;
using System.Collections.Generic;
using System.Linq;
using NeptunLight.Models;

namespace NeptunLight.ViewModels
{
    public class SubjectViewModel : ViewModelBase
    {
        public SubjectViewModel(Subject model, Func<Course, CourseViewModel> courseVmFac)
        {
            Name = model.Name;
            CreditCount = model.CreditCount;
            Courses = model.Courses.Select(courseVmFac).ToList();
        }
        public string Name { get; }

        public int CreditCount { get; }

        public IReadOnlyList<CourseViewModel> Courses { get; }
    }
}