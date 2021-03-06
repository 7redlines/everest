﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace Se7enRedLines.UI.Command
{
    public abstract class CommandBase : ObservableObject, ICommand
    {
        //======================================================
        #region _Constructors_

        protected CommandBase(string name, bool isEnabled = true)
        {
            Name = name;
            IsEnabled = isEnabled;
            IsVisible = true;
        }

        #endregion

        //======================================================
        #region _Public properties_

        /// <summary>
        /// Gets command name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets command text.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        protected bool _isEnabled;
        /// <summary>
        /// Gets or sets enable parameter of command.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    RaisePropertyChanged("IsEnabled");
                    RaiseCanExecuteChanged();
                }
            }
        }

        protected bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    RaisePropertyChanged("IsVisible");
                }
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { Set("IsChecked", ref _isChecked, value); }
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        //======================================================
        #region _Public methods_

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked. 
        /// </summary>
        public abstract void Execute(object parameter);

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "The this keyword is used in the Silverlight version")]
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        protected void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        #endregion

        //======================================================
        #region _Private, protected, internal fields_

        protected string _text;

        #endregion
    }
}