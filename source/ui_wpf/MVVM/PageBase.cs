using System.Windows.Controls;

namespace Se7enRedLines.UI.MVVM
{
    public class PageBase : Page
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
                        value.InitializeInternal();
                    }
                    DataContext = value;
                }
            }
        }

        #endregion
    }
}