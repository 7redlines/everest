﻿using System;
using System.Globalization;
using System.Windows.Data;
using Humanizer;

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

            if (parameter != null)
            {
                return time.ToLocalTime().ToString(parameter.ToString());
            }

            try
            {
                return time.Humanize(culture: CultureInfo.GetCultureInfo("en"));
            }
            catch (Exception ex)
            {
                return time.ToString("g");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}