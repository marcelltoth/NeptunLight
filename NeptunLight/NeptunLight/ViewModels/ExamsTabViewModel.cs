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
            DataStorage = dataStorage;
            Exams = dataStorage.CurrentData.ExamsPerSemester[semester].Select(examVmFac);
            SemesterName = semester.Name;
        }

        public IEnumerable<ExamViewModel> Exams { get; }

        public string SemesterName { get; }

        private IDataStorage DataStorage { get; }
    }
}