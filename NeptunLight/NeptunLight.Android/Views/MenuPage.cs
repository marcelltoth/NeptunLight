using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;
using ReactiveUI.AndroidSupport;

namespace NeptunLight.Droid.Views
{
    public class MenuPage : ReactiveUI.AndroidSupport.ReactiveFragment<MenuPageViewModel>, IActionBarProvider
    {
        public SwipeRefreshLayout SwipeRefresh { get; set; }

        public TextView NameLabel { get; set; }
        public TextView InfoLabel { get; set; }
        public TextView LastRefreshLabel { get; set; }

        public MenuButton MessagesButton { get; set; }
        public MenuButton CalendarButton { get; set; }
        public MenuButton CoursesButton { get; set; }
        public MenuButton ExamsButton { get; set; }
        public MenuButton SemestersButton { get; set; }
        public MenuButton PeriodsButton { get; set; }

        public ImageButton SettingsButton { get; set; }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.MenuPage, container, false);

            Activated.InvokeCommand(this, x=> x.ViewModel.EnsureDataAccessible);
            this.MyWireUpControls(layout);

            this.OneWayBind(ViewModel, x => x.IsRefreshing, x => x.SwipeRefresh.Refreshing);
            this.WhenActivated(() =>
            {
                return new List<IDisposable>()
                {
                    ViewModel.Refresh.ThrownExceptions.Subscribe(ex =>
                    {
                        if (Context != null)
                            Toast.MakeText(Activity, "Hiba a neptun-adatok frissítése közben", ToastLength.Short);

                        Crashes.TrackError(ex, new Dictionary<string, string>
                        {
                            {"Category", "Forced sync error"},
                            {"Source", "Swipe"}
                        });
                    }),
                    Observable.FromEventPattern(h => SwipeRefresh.Refresh += h, h => SwipeRefresh.Refresh -= h)
                              .Select(_ => Unit.Default)
                              .Do((_) => Analytics.TrackEvent("Forced sync started", new Dictionary<string, string>()
                              {
                                  {"Source", "Swipe"}
                              }))
                              .InvokeCommand(ViewModel.Refresh)
                };
            });
            
            this.BindCommand(ViewModel, x => x.GoToMessages, x => x.MessagesButton);
            this.BindCommand(ViewModel, x => x.GoToCalendar, x => x.CalendarButton);
            this.BindCommand(ViewModel, x => x.GoToCourses, x => x.CoursesButton);
            this.BindCommand(ViewModel, x => x.GoToExams, x => x.ExamsButton);
            this.BindCommand(ViewModel, x => x.GoToSemesters, x => x.SemestersButton);
            this.BindCommand(ViewModel, x => x.GoToPeriods, x => x.PeriodsButton);

            this.OneWayBind(ViewModel, x => x.Name, x => x.NameLabel.Text);
            this.OneWayBind(ViewModel, x => x.InfoLine, x => x.InfoLabel.Text);

            SettingsButton.Click += (sender, args) =>
            {
                StartActivity(new Intent(Application.Context, typeof(SettingsActivity)));
            };

            return layout;
        }
    }
}