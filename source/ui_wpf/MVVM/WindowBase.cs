using System.Windows;

namespace Se7enRedLines.UI.MVVM
{
    public class WindowBase : Window
    {
        //======================================================
        #region _Constructors_

        public WindowBase()
        {
            base.Activated += (sender, args) =>
            {
                var newEventArgs = new RoutedEventArgs(ActivatedEvent);
                RaiseEvent(newEventArgs);
            };

            base.Deactivated += (sender, args) =>
            {
                var newEventArgs = new RoutedEventArgs(DeactivatedEvent);
                RaiseEvent(newEventArgs);
            };

            CanBeClosed = true;
        }

        #endregion

        //======================================================
        #region _Public properties_

        public WindowViewModel Model
        {
            get { return (WindowViewModel)DataContext; }
            set
            {
                if (DataContext != value)
                {
                    if (value != null)
                    {
                        value.Window = this;
                        value.InitializeInternal();
                    }
                    DataContext = value;
                }
            }
        }

        // Provide CLR accessors for the event 
        public new event RoutedEventHandler Activated
        {
            add { AddHandler(ActivatedEvent, value); }
            remove { RemoveHandler(ActivatedEvent, value); }
        }

        public new event RoutedEventHandler Deactivated
        {
            add { AddHandler(DeactivatedEvent, value); }
            remove { RemoveHandler(DeactivatedEvent, value); }
        }

        public bool CanBeClosed { get; set; }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!CanBeClosed)
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        #endregion

        //======================================================
        #region _Fields_

        public static readonly RoutedEvent ActivatedEvent = EventManager.RegisterRoutedEvent("Activated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WindowBase));
        public static readonly RoutedEvent DeactivatedEvent = EventManager.RegisterRoutedEvent("Deactivated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WindowBase));

        #endregion
    }
}