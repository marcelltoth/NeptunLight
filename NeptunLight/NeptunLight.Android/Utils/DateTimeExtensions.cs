using System;
using Java.Util;

namespace NeptunLight.Droid.Utils
{
    public static class DateTimeExtensions
    {
        public static Calendar ToNativeCalendar(this DateTime dateTime)
        {
            Calendar c = Calendar.Instance;
            //c.Set(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            c.Set(CalendarField.Year, dateTime.Year);
            c.Set(CalendarField.Month, dateTime.Month);
            c.Set(CalendarField.DayOfMonth, dateTime.Day);
            c.Set(CalendarField.HourOfDay, dateTime.Hour);
            c.Set(CalendarField.Minute, dateTime.Minute);
            c.Set(CalendarField.Second, dateTime.Second);
            return c;
        }
    }
}