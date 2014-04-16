using System.Windows;
using System.Windows.Interactivity;

namespace Se7enRedLines.UI.Actions
{
    public class SetFocusAction : TargetedTriggerAction<FrameworkElement>
    {
        //======================================================
        #region _Public methods_

        protected override void Invoke(object parameter)
        {
            var target = Target;
            if (target != null)
                target.Focus();
        }

        #endregion
    }
}