using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NeptunLight.ViewModels;
using ReactiveUI;
using ReactiveUI.AndroidSupport;
using GridLayout = Android.Support.V7.Widget.GridLayout;

namespace NeptunLight.Droid.Views
{
    public class InitialSyncPage : ReactiveUI.AndroidSupport.ReactiveFragment<InitialSyncPageViewModel>
    {
        public ViewGroup LoginPanel { get; set; }

        public GridLayout StatusPanel { get; set; }

        public Button StartButton { get; set; }

        public ImageView BasicDataCompleted { get; set; }
        public TextView BasicDataFetching { get; set; }

        public ImageView SemesterDataCompleted { get; set; }
        public TextView SemesterDataFetching { get; set; }

        public ImageView SubjectsCompleted { get; set; }
        public TextView SubjectsFetching { get; set; }

        public ImageView ExamsCompleted { get; set; }
        public TextView ExamsFetching { get; set; }

        public ImageView CalendarCompleted { get; set; }
        public TextView CalendarFetching { get; set; }

        public ImageView PeriodsCompleted { get; set; }
        public TextView PeriodsFetching { get; set; }

        public ImageView MessagesCompleted { get; set; }
        public TextView MessagesFetching { get; set; }

        private Color StatusToColor(InitialSyncPageViewModel.RefreshStepState status)
        {
            switch (status)
            {
                case InitialSyncPageViewModel.RefreshStepState.Waiting:
                    return Color.LightGray;
                case InitialSyncPageViewModel.RefreshStepState.Refreshing:
                    return Color.DarkGray;
                case InitialSyncPageViewModel.RefreshStepState.Done:
                    return Color.Black;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.InitialSyncPage, container, false);

            this.WireUpControls(layout);

            this.WhenAnyObservable(x => x.ViewModel.IsSyncing).Subscribe(syncing =>
            {
                StatusPanel.Alpha = syncing ? 1 : 0;
                StartButton.Alpha = syncing ? 0 : 1;
            });

            this.BindCommand(ViewModel, x => x.PerformSync, x => x.StartButton);

            StartButton.Click += (sender, args) =>
            {
                Analytics.TrackEvent("Initial sync started");
            };

            this.WhenAnyObservable(x => x.ViewModel.EnsureCredentials.ThrownExceptions)
                .Merge(this.WhenAnyObservable(x => x.ViewModel.PerformSync.ThrownExceptions))
                .Subscribe(ex =>
                {
                    if (Context != null)
                        Toast.MakeText(Activity, "Kommunikációs hiba, ellenőrizd az internetkapcsolatodat.", ToastLength.Short).Show();
                    Crashes.TrackError(ex, new Dictionary<string, string>{
                        {"Category", "Initial sync error"}
                    });
                });

            this.WhenAnyObservable(x => x.ViewModel.PerformSync).Subscribe(_ =>
            {
                Analytics.TrackEvent("Initial sync successful", new Dictionary<string, string>{ {"Name", ViewModel.Name} });
            });

            this.WhenAnyValue(x => x.ViewModel.LoadBasicDataStatus).Select(s => s == InitialSyncPageViewModel.RefreshStepState.Done ? 1 : 0)
                .BindTo(this, x => x.BasicDataCompleted.Alpha);
            this.WhenAnyValue(x => x.ViewModel.LoadBasicDataStatus).Select(StatusToColor)
                .Subscribe(color => BasicDataFetching.SetTextColor(color));

            this.WhenAnyValue(x => x.ViewModel.LoadSemesterDataStatus).Select(s => s == InitialSyncPageViewModel.RefreshStepState.Done ? 1 : 0)
                .BindTo(this, x => x.SemesterDataCompleted.Alpha);
            this.WhenAnyValue(x => x.ViewModel.LoadSemesterDataStatus).Select(StatusToColor)
                .Subscribe(color => SemesterDataFetching.SetTextColor(color));

            this.WhenAnyValue(x => x.ViewModel.LoadCoursesStatus).Select(s => s == InitialSyncPageViewModel.RefreshStepState.Done ? 1 : 0)
                .BindTo(this, x => x.SubjectsCompleted.Alpha);
            this.WhenAnyValue(x => x.ViewModel.LoadCoursesStatus).Select(StatusToColor)
                .Subscribe(color => SubjectsFetching.SetTextColor(color));

            this.WhenAnyValue(x => x.ViewModel.LoadExamsStatus).Select(s => s == InitialSyncPageViewModel.RefreshStepState.Done ? 1 : 0)
                .BindTo(this, x => x.ExamsCompleted.Alpha);
            this.WhenAnyValue(x => x.ViewModel.LoadExamsStatus).Select(StatusToColor)
                .Subscribe(color => ExamsFetching.SetTextColor(color));

            this.WhenAnyValue(x => x.ViewModel.LoadCalendarStatus).Select(s => s == InitialSyncPageViewModel.RefreshStepState.Done ? 1 : 0)
                .BindTo(this, x => x.CalendarCompleted.Alpha);
            this.WhenAnyValue(x => x.ViewModel.LoadCalendarStatus).Select(StatusToColor)
                .Subscribe(color => CalendarFetching.SetTextColor(color));

            this.WhenAnyValue(x => x.ViewModel.LoadPeriodsStatus).Select(s => s == InitialSyncPageViewModel.RefreshStepState.Done ? 1 : 0)
                .BindTo(this, x => x.PeriodsCompleted.Alpha);
            this.WhenAnyValue(x => x.ViewModel.LoadPeriodsStatus).Select(StatusToColor)
                .Subscribe(color => PeriodsFetching.SetTextColor(color));

            this.WhenAnyValue(x => x.ViewModel.LoadMessagesStatus).Select(s => s == InitialSyncPageViewModel.RefreshStepState.Done ? 1 : 0)
                .BindTo(this, x => x.MessagesCompleted.Alpha);
            this.WhenAnyValue(x => x.ViewModel.LoadMessagesStatus).Select(StatusToColor)
                .Subscribe(color => MessagesFetching.SetTextColor(color));
            this.WhenAnyValue(x => x.ViewModel.MessageSyncProgress, x => x.ViewModel.MessagesTotal)
                .Subscribe(t =>
                {
                    MessagesFetching.SetText(
                        t.Item1 != -1
                            ? $"Üzenetek szinkronizálása... ({t.Item1}/{t.Item2})"
                            : "Üzenetek szinkronizálása...",
                        TextView.BufferType.Normal);
                });

            return layout;
        }
        
        public override void OnResume()
        {
            base.OnResume();
            Observable.Return(Unit.Default).InvokeCommand(ViewModel.EnsureCredentials);

            bool resized = false;
            LoginPanel.ViewTreeObserver.GlobalLayout += (o, args) =>
            {
                if (resized)
                    return;
                resized = true;
                // only do this once, otherwise it gets recursive

                ViewGroup.LayoutParams layout = LoginPanel.LayoutParameters;
                int originalHeight = LoginPanel.Height;
                int originalWidth = LoginPanel.Width;
                double finalHeight = originalHeight;
                const double ratio = 1193 / 842d;
                if (originalWidth > originalHeight / ratio) // too wide
                    layout.Width = (int)(originalHeight / ratio);
                else // too tall
                {
                    layout.Height = (int)(originalWidth * ratio);
                    finalHeight = layout.Height;
                }
                LoginPanel.LayoutParameters = layout;
                LoginPanel.SetPadding(LoginPanel.PaddingLeft, (int)(finalHeight * 0.22), LoginPanel.PaddingRight, LoginPanel.PaddingBottom);
            };
        }
    }
}