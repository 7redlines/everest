using System;
using System.Globalization;
using System.Windows.Data;

namespace Se7enRedLines.UI.Converters
{
    public class TimeSpanToNumberConverter : IValueConverter
    {
        //======================================================
        #region _Public methods_

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                var time = (TimeSpan) value;
                if (parameter == null)
                    return time.TotalMilliseconds;

                throw new NotSupportedException("Currently only empty parameter supported");
            }

            throw new InvalidOperationException("Value is not of TimeSpan type");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double time;
            if (!double.TryParse(value.ToString(), out time))
                throw new InvalidOperationException("Value is not a number.");

            if (parameter == null)
                return TimeSpan.FromMilliseconds(time);

            throw new NotSupportedException("Currently only empty parameter supported");
        }

        #endregion
    }
}