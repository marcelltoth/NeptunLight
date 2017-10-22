using System;
using System.Collections.Generic;
using System.Linq;
using NeptunLight.Models;
using NeptunLight.Services;

namespace NeptunLight.ViewModels
{
    public class ExamsTabViewModel : ViewModelBase
    {
        public ExamsTabViewModel(Semester semester, IDataStorage dataStorage, Func<Exam, ExamViewModel> examVmFac)
        {
            Exams = dataStorage.CurrentData.ExamsPerSemester[semester].Select(examVmFac).OrderBy(eVm => eVm.StartTime).ToList();
            SemesterName = semester.Name;
        }

        public IReadOnlyList<ExamViewModel> Exams { get; }

        public string SemesterName { get; }
    }
}