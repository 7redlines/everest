using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace Se7enRedLines.UI.MVVM
{
    public class WindowViewModel : ViewModel
    {
        //======================================================
        #region _Constructors_

        public WindowViewModel()
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

        public WindowBase Window { get; set; }

        private bool _isMaximized;
        public bool IsMaximized
        {
            get { return _isMaximized; }
            set { Set("IsMaximized", ref _isMaximized, value); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { Set("IsActive", ref _isActive, value); }
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        protected override void Initialize()
        {
            base.Initialize();

            IsMaximized = Window.WindowState == WindowState.Maximized;

            Window.StateChanged +=
                (sender, args) =>
                {
                    IsMaximized = Window.WindowState == WindowState.Maximized;
                };

            Window.Loaded += (sender, args) =>
            {
                IsMaximized = Window.WindowState == WindowState.Maximized;
            };

            Window.Activated += (sender, args) => IsActive = true;
            Window.Deactivated += (sender, args) => IsActive = false;

            IsActive = Window.IsActive;
        }

        [Conditional("DEBUG")]
        protected virtual void InitializeDesignTime()
        {
            IsMaximized = false;
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