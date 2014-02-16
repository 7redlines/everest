using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Se7enRedLines.UI.Behaviors
{
    public class UpdateSourceAction : Behavior<Control>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject is TextBox)
            {
                var ctrl = AssociatedObject as TextBox;
                ctrl.TextChanged += OnTextChanged;
            }
            else if (AssociatedObject is PasswordBox)
            {
                var ctrl = AssociatedObject as PasswordBox;
                ctrl.PasswordChanged += OnPasswordChanged;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            AssociatedObject.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject is TextBox)
            {
                var ctrl = AssociatedObject as TextBox;
                ctrl.TextChanged -= OnTextChanged;
            }
            else if (AssociatedObject is PasswordBox)
            {
                var ctrl = AssociatedObject as PasswordBox;
                ctrl.PasswordChanged -= OnPasswordChanged;
            }
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException("UpdateSourceAction for PasswordBox not implemented");
            //AssociatedObject.GetBindingExpression(PasswordBox.PasswordProperty).UpdateSource();
        }
    }
}