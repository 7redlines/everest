using System;
using System.Globalization;
using Se7enRedLines.UI.MVVM;

namespace Se7enRedLines.UI.Converters
{
    public class CommandPathConverter : BaseTwoBindingConverter
    {
        //======================================================
        #region _Private, protected, internal methods_

        protected override object Convert(object value1, object value2, Type targetType, object parameter, CultureInfo culture)
        {
            if (value1 == null || string.IsNullOrEmpty(value1.ToString()) || !(value2 is ViewModel))
                return null;

            var viewModel = value2 as ViewModel;
            return viewModel.Commands[value1.ToString()];
        }

        #endregion
    }
}