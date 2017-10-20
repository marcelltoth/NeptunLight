using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

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