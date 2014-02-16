using System;
using System.Globalization;
using System.Windows.Data;

namespace Se7enRedLines.UI.Converters
{
    public class TimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime))
            {
                throw new InvalidOperationException("Only DateTime value supported");
            }

            var time = (DateTime) value;

            return ToRelativeDateString(time, true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compares a supplied date to the current date and generates a friendly English 
        /// comparison ("5 days ago", "5 days from now")
        /// </summary>
        /// <param name="date">The date to convert</param>
        /// <param name="approximate">When off, calculate timespan down to the second.
        /// When on, approximate to the largest round unit of time.</param>
        /// <returns></returns>
        public static string ToRelativeDateString(DateTime value, bool approximate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - value.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * MINUTE)
            {
                return "a minute ago";
            }
            if (delta < 45 * MINUTE)
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 90 * MINUTE)
            {
                return "an hour ago";
            }
            if (delta < 24 * HOUR)
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * HOUR)
            {
                return "yesterday";
            }
            if (delta < 30 * DAY)
            {
                return ts.Days + " days ago";
            }
            if (delta < 12 * MONTH)
            {
                int months = (int) Math.Floor((double)ts.Days / 30);
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = (int) Math.Floor((double)ts.Days / 365);
                return years <= 1 ? "one year ago" : years + " years ago";
            }

        }
    }
}