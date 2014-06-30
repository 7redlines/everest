using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Se7enRedLines.Collections;
using Se7enRedLines.UI.Command;

namespace Se7enRedLines.UI.MVVM
{
    public class ViewModel : ViewModelBase
    {
        public class BusyValue : ObservableObject
        {
            //======================================================
            #region _Constructors_

            public BusyValue(string text, bool isIntermidiate, double value)
            {
                _text = text;
                _isIntermidiate = isIntermidiate;
                _value = value;
            }

            #endregion

            //======================================================
            #region _Public properties_

            private string _text;
            public string Text
            {
                get { return _text; }
                set { Set("Text", ref _text, value); }
            }

            private bool _isIntermidiate;
            public bool IsIntermidiate
            {
                get { return _isIntermidiate; }
                set { Set("IsIntermidiate", ref _isIntermidiate, value); }
            }

            private double _value;
            public double Value
            {
                get { return _value; }
                set { Set("Value", ref _value, value); }
            }

            #endregion
        }

        public class DataValue : ObservableObject
        {
            public DataValue(object value)
            {
                _value = value;
            }

            private object _value;
            public object Value
            {
                get { return _value; }
                set { Set("Value", ref _value, value); }
            }
        }

        //======================================================
        #region _Constructors_

        public ViewModel()
        {
            Commands = new ObservableDataValueDictionary<CommandBase>();
            Events = new ObservableDataValueDictionary<CommandBase>();
            Values = new ObservableDataValueDictionary<DataValue>();

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

        private bool _isBusy;
        public virtual bool IsBusy
        {
            get { return _isBusy; }
        }

        private List<BusyValue> _busyStack = new List<BusyValue>();
        private BusyValue _busy;
        public BusyValue Busy
        {
            get { return _busy; }
            private set
            {
                Set("Busy", ref _busy, value);
                Set("IsBusy", ref _isBusy, _busy != null);
            }
        }

        public ObservableDataValueDictionary<CommandBase> Commands { get; protected set; }
        public ObservableDataValueDictionary<CommandBase> Events { get; protected set; }
        public ObservableDataValueDictionary<DataValue> Values { get; protected set; }

        #endregion

        //======================================================
        #region _Private, protected, internal properties_

        public Dispatcher Dispatcher { get; set; }

        #endregion

        //======================================================
        #region _Publi methods_
        
        public void InitializeInternal()
        {
            if (Dispatcher == null)
                throw new InvalidOperationException("Dispatcher should be assigned before calling Initialize method.");

            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        protected virtual void Initialize()
        {
        }

        [Conditional("DEBUG")]
        protected virtual void InitializeDesignTime()
        {

        }

        // ------------------------------------------------- Commands ------------------------------------------------------- //

        protected virtual CommandBase AddCommand(string name, Action command)
        {
            return AddCommand(name, command, true, true);
        }

        protected virtual CommandBase AddCommand<T>(string name, Action<T> command)
        {
            return AddCommand(name, command, true, true);
        }

        protected virtual CommandBase AddCommand(string name, Action command, bool enable, bool visible)
        {
            var cmd = new RelayCommand(name, command, enable, visible);
            Commands.Add(name, cmd);
            return cmd;
        }

        protected virtual CommandBase AddCommand<T>(string name, Action<T> command, bool enable, bool visible)
        {
            var cmd = new RelayCommand<T>(name, command, enable, visible);
            Commands.Add(name, cmd);
            return cmd;
        }

        /// <summary>
        /// Uses SmartDispatcher.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enable"></param>
        /// <param name="visible"></param>
        protected virtual void ChangeCommandState(string name, bool? enable = null, bool? visible = null)
        {
            if ((enable.HasValue || visible.HasValue) && Commands.ContainsKey(name))
            {
                var command = Commands[name];

                Dispatcher.Invoke(new Action(() =>
                {
                    if (enable.HasValue)
                        command.IsEnabled = enable.Value;
                    if (visible.HasValue)
                        command.IsVisible = visible.Value;
                }));
            }
        }

        protected virtual void RemoveCommand(string name)
        {
            Commands.Remove(name);
        }

        // --------------------------------------------------- events ------------------------------------------------------- //

        protected virtual CommandBase AddEvent(string name, Action handler)
        {
            var h = new RelayCommand(name, handler, true, true);
            Events.Add(name, h);
            return h;
        }

        protected virtual CommandBase AddEvent<T>(string name, Action<T> handler)
        {
            var h = new RelayCommand<T>(name, handler, true, true);
            Events.Add(name, h);
            return h;
        }

        protected virtual void ChangeEventState(string name, bool enable)
        {
            if (Events.ContainsKey(name))
            {
                var command = Events[name];

                Dispatcher.Invoke(new Action(() =>
                {
                    command.IsEnabled = enable;
                }));
            }
        }

        protected virtual bool RemoveEvent(string name)
        {
            return Events.Remove(name);
        }

        // --------------------------------------------------- values ------------------------------------------------------- //

        protected virtual void NewOrClearValue(string key)
        {
            SetValue(key, (object)null);
        }

        protected virtual void SetValue<T>(string key, T value)
        {
#if DEBUG
            if (IsInDesignMode)
            {
                DataValue v;
                if (!Values.TryGetValue(key, out v))
                    Values[key] = new DataValue(value);
                else
                    v.Value = value;
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    DataValue v;
                    if (!Values.TryGetValue(key, out v))
                        Values[key] = new DataValue(value);
                    else
                        v.Value = value;
                }));
            }
#else
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DataValue v;
                if (!Values.TryGetValue(key, out v))
                    Values[key] = new DataValue(value);
                else
                    v.Value = value;
            }));
#endif
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

        protected virtual T GetValue<T>(string key)
        {
            if (!Values.ContainsKey(key))
                return default(T);

            return (T)Values[key].Value;
        }

        protected virtual DataValue GetValue(string key)
        {
            return !Values.ContainsKey(key) ? null : Values[key];
        }

        // --------------------------------------------------- busy ------------------------------------------------------- //

        protected void PushBusy(string status, bool isIntermidiate = true, double value = 0)
        {
            var busyStatus = new BusyValue(status, isIntermidiate, value);
            if (Busy != null)
            {
                _busyStack.Insert(0, Busy);
            }

            Busy = busyStatus;
        }

        protected void ChangeBusy(string status, double value, bool? isIntermidiate)
        {
            var busy = Busy.Text == status ? Busy : _busyStack.FirstOrDefault(b => b.Text == status);

            if (busy != null)
            {
                busy.Value = value;
                if (isIntermidiate.HasValue)
                {
                    busy.IsIntermidiate = isIntermidiate.Value;
                }
            }
        }

        protected void PopBusy(string status)
        {
            if (Busy != null && Busy.Text == status)
            {
                Busy = _busyStack.FirstOrDefault();
                if (Busy != null)
                    _busyStack.RemoveAt(0);
            }
            else
            {
                _busyStack.RemoveAll(b => b.Text == status);
            }
        }

        protected void ClearBusy()
        {
            _busyStack.Clear();
            Busy = null;
        }

        #endregion

        //======================================================
        #region _Fields_

        private bool _isInitialized;

        #endregion
    }
}