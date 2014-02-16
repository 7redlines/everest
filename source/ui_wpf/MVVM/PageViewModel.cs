namespace Se7enRedLines.UI.MVVM
{
    public class PageViewModel : ViewModel
    {
        //======================================================
        #region _Public properties_

        public PageBase Page { get; set; }

        #endregion

        //======================================================
        #region _Public methods_

        public virtual void OnNavigatedTo()
        {

        }

        public virtual void OnNavigatingFrom()
        {

        }

        #endregion
    }
}