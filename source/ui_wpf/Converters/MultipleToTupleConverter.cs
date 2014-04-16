using System;
using System.Globalization;
using System.Windows.Data;

namespace Se7enRedLines.UI.Converters
{
    public class MultipleToTupleConverter : IMultiValueConverter
    {
        //======================================================
        #region _Private, protected, internal methods_

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 1)
            {
                return new Tuple<object>(values[0]);
            }
            if (values.Length == 2)
            {
                return new Tuple<object, object>(values[0], values[1]);
            }
            if (values.Length == 3)
            {
                return new Tuple<object, object, object>(values[0], values[1], values[2]);
            }

            throw new NotSupportedException("Only 1,2 or 3 arguments are supported");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is Tuple<object>)
            {
                var tuple = value as Tuple<object>;
                return new[] { tuple.Item1 };
            }
            if (value is Tuple<object, object>)
            {
                var tuple = value as Tuple<object, object>;
                return new[] {tuple.Item1, tuple.Item2};
            }
            if (value is Tuple<object, object, object>)
            {
                var tuple = value as Tuple<object, object, object>;
                return new[] { tuple.Item1, tuple.Item2, tuple.Item3};
            }

            throw new NotSupportedException("Only 1,2 or 3 arguments are supported");
        }

        #endregion
    }
}