using System;
using System.Collections.Generic;
using System.Linq;
using NeptunLight.Models;
using NeptunLight.Services;

namespace NeptunLight.ViewModels
{
    public class CoursesTabViewModel : ViewModelBase
    {
        public IReadOnlyList<SubjectViewModel> Subjects { get; }

        public string SemesterName { get; }

        public CoursesTabViewModel(Semester semester, IDataStorage data, Func<Subject, SubjectViewModel> subjectVmFac)
        {
            Subjects = data.CurrentData.SubjectsPerSemester[semester].Select(subjectVmFac).ToList();
            SemesterName = semester.Name;
        }
    }
}