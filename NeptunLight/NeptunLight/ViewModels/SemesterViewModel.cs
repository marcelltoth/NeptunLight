using NeptunLight.Models;

namespace NeptunLight.ViewModels
{
    public class SemesterViewModel : ViewModelBase
    {
        public SemesterViewModel(SemesterData model)
        {
            SemesterName = model.Semester.Name;
            Status = model.Status;
            if (!string.IsNullOrEmpty(model.FinancialStatus))
            {
                Status += " - " + model.FinancialStatus;
            }
            CreditInfo = $"{(model.CreditsAccomplished.HasValue ? model.CreditsAccomplished.ToString() : "-")} / {(model.CreditsTaken.HasValue ? model.CreditsTaken.ToString() : "-")}";
            TotalCreditInfo = $"{(model.TotalCreditsAccomplished.HasValue ? model.TotalCreditsAccomplished.ToString() : "-")} / {(model.TotalCreditsTaken.HasValue ? model.TotalCreditsTaken.ToString() : "-")}";
            Average = model.Average.HasValue ? model.Average.Value.ToString("0.00") : " - ";
            CumulativeAverage = model.CumulativeAverage.HasValue ? model.CumulativeAverage.Value.ToString("0.00") : " - ";
        }

        public string SemesterName { get; }

        public string Status { get; }

        public string CreditInfo { get; }

        public string TotalCreditInfo { get; }

        public string Average { get; }

        public string CumulativeAverage { get; }
    }
}