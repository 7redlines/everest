using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace Se7enRedLines.UI.MVVM
{
    public class PageBase : Page, ICleanup
    {
        //======================================================
        #region _Public properties_

        public PageViewModel Model
        {
            get { return (PageViewModel)DataContext; }
            set
            {
                if (DataContext != value)
                {
                    if (value != null)
                    {
                        value.Page = this;
                        value.Dispatcher = Dispatcher;
                        if (!UIEnvironment.IsInDesignMode)
                            value.InitializeInternal();
                    }
                    DataContext = value;
                }
            }
        }

        #endregion

        //======================================================
        #region _Public methods_

        public virtual void Cleanup()
        {
            if (Model != null)
                Model.Cleanup();
        }

        #endregion
    }
}