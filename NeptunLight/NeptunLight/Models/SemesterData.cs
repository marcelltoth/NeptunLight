namespace NeptunLight.Models
{
    public class SemesterData
    {
        public Semester Semester { get; }

        public string Status { get; }

        public string FinancialStatus { get; }

        public int CreditsAccomplished { get; }

        public int CreditsTaken { get; }

        public int TotalCreditsAccomplished { get; }

        public int TotalCreditsTaken { get; }

        public int Average { get; }

        public int CumulativeAverage { get; }
    }
}