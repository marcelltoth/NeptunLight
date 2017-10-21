namespace NeptunLight.ViewModels
{
    public class SemestersPageViewModel : PageViewModel
    {
        public override string Title { get; } = "Féléves adatok";

        public SemestersCreditsTabViewModel CreditData { get; }

        public SemestersAveragesTabViewModel AveragesData { get; }

        public SemestersPageViewModel(SemestersCreditsTabViewModel creditData, SemestersAveragesTabViewModel averagesData)
        {
            CreditData = creditData;
            AveragesData = averagesData;
        }
    }
}