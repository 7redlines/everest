﻿using System.Windows;
using System.Windows.Interactivity;

namespace Se7enRedLines.UI.Actions 
{
    public class UnfocusAction : TargetedTriggerAction<FrameworkElement>
    {
        //======================================================
        #region _Private, protected, internal methods_

        protected override void Invoke(object parameter)
        {
            if (Target != AssociatedObject)
                Target.Focus();
            else if (Application.Current.MainWindow.Focus())
            {
            }
        }

        #endregion
    }
}