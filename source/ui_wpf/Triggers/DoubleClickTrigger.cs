using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Se7enRedLines.UI
{
    public class DoubleClickTrigger : EventTriggerBase<FrameworkElement>
    {
        //======================================================
        #region _Private, protected, internal methods_

        protected override string GetEventName()
        {
            return "DoubleClick";
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            Source.MouseLeftButtonDown += OnDoubleClick;
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                OnEvent(e);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            Source.MouseLeftButtonDown -= OnDoubleClick;
        }

        #endregion
    }
}