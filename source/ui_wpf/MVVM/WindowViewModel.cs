using System;
using System.Diagnostics;
using System.Windows;

namespace Se7enRedLines.UI.MVVM
{
    public class WindowViewModel : ViewModel
    {
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

        internal bool OnClosingInternal()
        {
            return OnClosing();
        }

        protected virtual bool OnClosing()
        {
            return true;
        }

        internal void OnClosedInternal()
        {
            try
            {
                Cleanup();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            try
            {
                OnClosed();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        protected virtual void OnClosed()
        {
        }

        protected override void InitializeDesignTime()
        {
            base.InitializeDesignTime();

            IsMaximized = false;
        }

        #endregion
    }
}