using System;
using System.Globalization;
using System.Windows.Data;

namespace Se7enRedLines.UI.Converters
{
    public abstract class BaseTwoBindingConverter : IMultiValueConverter
    {
        //======================================================
        #region _Public methods_

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(values[0], values[1], targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetTypes[0], targetTypes[1], parameter, culture);
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        protected abstract object Convert(object value1, object value2, Type targetType, object parameter,
            CultureInfo culture);

        protected virtual object[] ConvertBack(object value, Type targetType1, Type targetType2, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}