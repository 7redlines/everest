using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Se7enRedLines.UI.Behaviors
{
    public class RadioButtonAsToggleBehavior : Behavior<RadioButton>
    {
        //======================================================
        #region _Private, protected, internal methods_

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseUp += OnPreviewClick;
            AssociatedObject.Click += OnClick;
        }

        private void OnPreviewClick(object sender, MouseButtonEventArgs e)
        {
            _checkedStateBeforeClick = AssociatedObject.IsChecked;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (_checkedStateBeforeClick == true)
            {
                AssociatedObject.IsChecked = false;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseUp -= OnPreviewClick;
            AssociatedObject.Click -= OnClick;
        }

        #endregion

        //======================================================
        #region _Fields_

        private bool? _checkedStateBeforeClick;

        #endregion
    }
}