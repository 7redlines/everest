﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Se7enRedLines.UI.Converters
{
    public class InverseConverter : IValueConverter
    {
        //======================================================
        #region _Public methods_

#if WinRT
		public virtual object Convert(object value, Type targetType, object parameter, string language)
#else
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            if (value == null)
                return new object();

            if (value is bool)
                return !(bool)value;

            throw new NotSupportedException(string.Format("Type {0} not suported by InverseConverter", targetType.Name));
        }

#if WinRT
		public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            throw new NotSupportedException("ConvertBack not supported by InverseConverter");
        }

        #endregion
    }
}