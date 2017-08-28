namespace NeptunLight.Models
{
    public class SemesterData
    {
        public SemesterData(Semester semester, string status, string financialStatus, int? creditsAccomplished, int? creditsTaken, int? totalCreditsAccomplished, int? totalCreditsTaken, double? average, double? cumulativeAverage)
        {
            Semester = semester;
            Status = status;
            FinancialStatus = financialStatus;
            CreditsAccomplished = creditsAccomplished;
            CreditsTaken = creditsTaken;
            TotalCreditsAccomplished = totalCreditsAccomplished;
            TotalCreditsTaken = totalCreditsTaken;
            Average = average;
            CumulativeAverage = cumulativeAverage;
        }

        public Semester Semester { get; }

        public string Status { get; }

        public string FinancialStatus { get; }

        public int? CreditsAccomplished { get; }

        public int? CreditsTaken { get; }

        public int? TotalCreditsAccomplished { get; }

        public int? TotalCreditsTaken { get; }

        public double? Average { get; }

        public double? CumulativeAverage { get; }
    }
}