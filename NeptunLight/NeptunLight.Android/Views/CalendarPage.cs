using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Alamkanak.Weekview;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using NeptunLight.Droid.Utils;
using NeptunLight.Models;
using NeptunLight.ViewModels;
using ReactiveUI.AndroidSupport;

namespace NeptunLight.Droid.Views
{
    public class CalendarPage : ReactiveUI.AndroidSupport.ReactiveFragment<CalendarPageViewModel>, MonthLoader.IMonthChangeListener, IActionBarProvider
    {
        public CalendarPage()
        {
            _colorPoolInitTask = Task.Run(async () => { _colorPool = await CalendarColorPool.LoadAsync(); });
        }

        private readonly Task _colorPoolInitTask;
        private CalendarColorPool _colorPool;

        private readonly Dictionary<long, CalendarEvent> _eventIdMap = new Dictionary<long, CalendarEvent>();
        private long _eventIdMapPointer;
        private LayoutInflater _layoutInflater;
        private View _layoutRoot;

        public WeekView WeekView { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Analytics.TrackEvent("Calendar page shown");
        }

        public override View OnCreateView([NotNull] LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            _layoutInflater = inflater;
            _layoutRoot = inflater.Inflate(Resource.Layout.CalendarPage, container, false);

            this.WireUpControls(_layoutRoot);

            // Apply a dummy event listener because of a bug in the library. It will try to call the listener even if it is not set, resulting in a NullPointerException -> crash
            WeekView.SetZoomEndListener(new DummyZoomEndListener());
            WeekView.MonthChangeListener = this;
            WeekView.EventClick += WeekView_EventClick;
            WeekView.SetLimitTime(6,22);
            DateTime now = DateTime.Now;
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                now = now.AddDays(7);
            WeekView.GoToDate(now.StartOfWeek().ToNativeCalendar());
            return _layoutRoot;
        }

        private void WeekView_EventClick(object sender, WeekView.EventClickEventArgs e)
        {
            CalendarEvent ev = _eventIdMap[e.P0.Id];

            View popupView = _layoutInflater.Inflate(Resource.Layout.CalendarEventPopup, null);
            popupView.FindViewById<TextView>(Resource.Id.eventTitle).Text = ev.Title;
            popupView.FindViewById<TextView>(Resource.Id.eventInstructor).Text = ev.Instructor;
            popupView.FindViewById<TextView>(Resource.Id.eventGroup).Text = ev.Group;
            popupView.FindViewById<TextView>(Resource.Id.eventStartTime).Text = ev.StartDate.ToString("g");
            popupView.FindViewById<TextView>(Resource.Id.eventEndTime).Text = ev.EndDate.ToString("g");
            popupView.FindViewById<TextView>(Resource.Id.eventLocation).Text = ev.Location;
            PopupWindow popupWindow = new PopupWindow(popupView, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent, true);
            popupWindow.ShowAtLocation(_layoutRoot, GravityFlags.Center, 0, 0);
            popupView.Touch += (o, args) =>
            {
                popupWindow.Dismiss();
            };
        }

        // p0 is year
        // p1 is month
        public IList<WeekViewEvent> OnMonthChange(int p0, int p1)
        {
            _colorPoolInitTask.Wait(300);
            IEnumerable<CalendarEvent> eventList = ViewModel.Events.Where(ce => ce.StartDate.Year == p0 && ce.StartDate.Month == p1);
            List<WeekViewEvent> nativeList = new List<WeekViewEvent>();
            foreach (CalendarEvent calendarEvent in eventList)
            {
                _eventIdMap[_eventIdMapPointer] = calendarEvent;
                nativeList.Add(new WeekViewEvent(_eventIdMapPointer, calendarEvent.Title, 
                    calendarEvent.StartDate.Year, calendarEvent.StartDate.Month, calendarEvent.StartDate.Day, calendarEvent.StartDate.Hour, calendarEvent.StartDate.Minute,
                                                 calendarEvent.EndDate.Year, calendarEvent.EndDate.Month, calendarEvent.EndDate.Day, calendarEvent.EndDate.Hour, calendarEvent.EndDate.Minute)
                {
                    Color = _colorPool[calendarEvent.Title, calendarEvent.Group],
                    Location = calendarEvent.Location,
                    Name = $"{calendarEvent.Title} ({calendarEvent.Group})"
                });

                _eventIdMapPointer++;
            }
            return nativeList;
        }

        private class DummyZoomEndListener : Java.Lang.Object, WeekView.IZoomEndListener
        {
            public void OnZoomEnd(int p0)
            {
            }
        }
    }
}