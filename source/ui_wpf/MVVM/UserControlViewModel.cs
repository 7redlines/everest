using System.Diagnostics;
using GalaSoft.MvvmLight.Threading;

namespace Se7enRedLines.UI.MVVM
{
    public abstract class UserControlViewModel : ViewModel
    {
        //======================================================
        #region _Constructors_

        public UserControlViewModel()
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

        public UserControlBase Control { get; set; }

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