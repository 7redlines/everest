using System;
using System.Globalization;

namespace Se7enRedLines.UI.Converters
{
    public class BooleanTwoBindingConverter : BaseTwoBindingConverter
    {
        //======================================================
        #region _Public properties_

        public bool Inverse { get; set; }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        protected override object Convert(object value1, object value2, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool) && targetType != typeof(bool?))
                throw new ArgumentOutOfRangeException("targetType", "BooleanValueConverter can only convert to bool");

            var result = Equals(value1, value2);

            return !Inverse ? result : !result;
        }

        protected override object[] ConvertBack(object value, Type targetType1, Type targetType2, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion 
    }
}