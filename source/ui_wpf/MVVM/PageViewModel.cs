using System.Diagnostics;
using System.Windows.Markup;
using GalaSoft.MvvmLight.Threading;

namespace Se7enRedLines.UI.MVVM
{
    public class PageViewModel : ViewModel
    {
        //======================================================
        #region _Constructors_

        public PageViewModel()
        {
#if DEBUG
            if (IsInDesignMode)
            {
                DispatcherHelper.Initialize();
                InitializeDesignTime();
            }
#endif
        }

        #endregion

        //======================================================
        #region _Public properties_

        public PageBase Page { get; set; }

        #endregion

        //======================================================
        #region _Public methods_

        public virtual void OnNavigatedTo(object extraData = null)
        {

        }

        public virtual void OnNavigatingFrom(object extraData = null)
        {

        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        [Conditional("DEBUG")]
        protected virtual void InitializeDesignTime()
        {
            
        }

#if DEBUG
        protected bool Set<T>(string propertyName, ref T field, T value)
        {
            if (IsInDesignMode)
            {
                if (Equals(field, null))
                {
                    field = value;
                }

                return true;
            }
            
            return base.Set(propertyName, ref field, value);
        }
#endif

        #endregion
    }
}