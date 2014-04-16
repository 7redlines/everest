using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace Se7enRedLines.UI.MVVM
{
    public class UserControlBase : UserControl, ICleanup
    {
        //======================================================
        #region _Public properties_

        public UserControlViewModel Model
        {
            get
            {
                return RootElement.DataContext as UserControlViewModel;
            }
            set
            {
                if (RootElement.DataContext != value)
                {
                    if (value != null)
                    {
                        value.Control = this;
                        value.Dispatcher = Dispatcher;
                        if (!UIEnvironment.IsInDesignMode)
                        {
                            value.InitializeInternal();
                        }
                    }
                    RootElement.DataContext = value;
                }
            }
        }

        public FrameworkElement RootElement
        {
            get { return Content as FrameworkElement; }
        }

        #endregion

        //======================================================
        #region _Public methods_

        public void Cleanup()
        {
            if (Model != null)
                Model.Cleanup();
        }

        #endregion
    }
}