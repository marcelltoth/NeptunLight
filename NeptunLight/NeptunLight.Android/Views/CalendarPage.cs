using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Com.Alamkanak.Weekview;
using NeptunLight.Models;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class CalendarPage : ReactiveFragment<CalendarPageViewModel>, MonthLoader.IMonthChangeListener, IActionBarContentProvider
    {
        public CalendarPage()
        {
            _colorPoolInitTask = Task.Run(async () => { _colorPool = await CalendarColorPool.LoadAsync(); });
        }

        private readonly Task _colorPoolInitTask;
        private CalendarColorPool _colorPool;

        public WeekView WeekView { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.CalendarPage, container, false);

            this.WireUpControls(layout);


            WeekView.MonthChangeListener = this;

            return layout;
        }

        // p0 is year
        // p1 is month
        public IList<WeekViewEvent> OnMonthChange(int p0, int p1)
        {
            _colorPoolInitTask.Wait(300);
            IEnumerable<CalendarEvent> eventList = ViewModel.Events.Where(ce => ce.StartDate.Year == p0 && ce.StartDate.Month == p1);
            List<WeekViewEvent> nativeList = new List<WeekViewEvent>();
            int i = 0;
            foreach (CalendarEvent calendarEvent in eventList)
            {
                nativeList.Add(new WeekViewEvent(i, calendarEvent.Title, 
                    calendarEvent.StartDate.Year, calendarEvent.StartDate.Month, calendarEvent.StartDate.Day, calendarEvent.StartDate.Hour, calendarEvent.StartDate.Minute,
                                                 calendarEvent.EndDate.Year, calendarEvent.EndDate.Month, calendarEvent.EndDate.Day, calendarEvent.EndDate.Hour, calendarEvent.EndDate.Minute)
                {
                    Color = _colorPool[calendarEvent.Title, calendarEvent.Group],
                    Location = calendarEvent.Location,
                    Name = $"{calendarEvent.Title} ({calendarEvent.Group})"
                });
            }
            return nativeList;
        }
    }
}