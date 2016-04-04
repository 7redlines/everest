using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Helpers;

namespace Se7enRedLines.UI
{
    /// <summary>
    /// Wrapper around HotkeyHook. Use HotkeyHook for low level hotkey operations.
    /// </summary>
    public static class HotkeyManager
    {
        //======================================================
        #region _Constructors_

        static HotkeyManager()
        {
            HotkeyHook.HotkeyPressed += OnHotKeyPressed;
        }

        #endregion

        //======================================================
        #region _Public properties_

        public static bool IsPaused { get; private set; }

        #endregion

        //======================================================
        #region _Public methods_

        /// <summary>
        /// Unsubscribe everything and re-register new hotkeys.
        /// </summary>
        /// <param name="hotkeys"></param>
        /// <returns>whether all hotkey have been registed.</returns>
        public static bool ReInitHotkey(params HotkeyInfo[] hotkeys)
        {
            Contract.Requires(hotkeys != null);

            Unsubscribe();
            HotkeyHook.Unregister();

            var result = true;
            foreach (var hotkey in hotkeys)
            {
                result = result && HotkeyHook.Register(hotkey);
            }

            return result;
        }

        public static void CleanupAll()
        {
            Unsubscribe();
            HotkeyHook.Unregister();
        }

        public static bool AddHotkey(HotkeyInfo hotkey)
        {
            return HotkeyHook.Register(hotkey);
        }

        public static bool RemoveHotkey(HotkeyInfo hotkey)
        {
            return HotkeyHook.Unregister(hotkey);
        }

        public static void Pause()
        {
            HotkeyHook.HotkeyPressed -= OnHotKeyPressed;
            IsPaused = true;
        }

        public static void Resume()
        {
            HotkeyHook.HotkeyPressed += OnHotKeyPressed;
            IsPaused = false;
        }

        public static HotkeyInfo GetHotkey(KeyGesture gesture)
        {
            return HotkeyHook.Registered.FirstOrDefault(hotkey => hotkey.KeyGesture.Key == gesture.Key && hotkey.KeyGesture.Modifiers == gesture.Modifiers);
        }

        public static bool Subscribe(object recipient, KeyGesture gesture, Action<HotkeyEventArgs> action)
        {
            Contract.Requires(recipient != null);
            Contract.Requires(gesture != null);
            Contract.Requires(action != null);

            var hotkey = GetHotkey(gesture);
            if (hotkey == null)
                return false;

            var list = GetSubscribtions(hotkey.Id);
            list.Add(new HotkeyManagerItem(hotkey.Id, new WeakAction<HotkeyEventArgs>(recipient, action)));

            Cleanup();
            return true;
        }

        public static void Unsubscribe(object recipient, KeyGesture gesture)
        {
            Contract.Requires(recipient != null);
            Contract.Requires(gesture != null);

            var hotkey = GetHotkey(gesture);
            if (hotkey == null)
                return;

            var lists = _subscribtions;
            if (lists.Count == 0 || !lists.ContainsKey(hotkey.Id))
                return;

            lock (lists)
            {
                foreach (var item in lists[hotkey.Id])
                {
                    var weakAction = item.Action;

                    if (weakAction != null && recipient == weakAction.Target)
                        weakAction.MarkForDeletion();
                }
            }

            Cleanup();
        }

        public static void Unsubscribe(object recipient)
        {
            Contract.Requires(recipient != null);

            var lists = _subscribtions;
            if (lists.Count == 0)
                return;

            lock (lists)
            {
                foreach (var hotkeyId in lists.Keys)
                {
                    foreach (var item in lists[hotkeyId])
                    {
                        var weakAction = item.Action;

                        if (weakAction != null && recipient == weakAction.Target)
                            weakAction.MarkForDeletion();
                    }
                }
            }

            Cleanup();
        }

        public static void Unsubscribe()
        {
            var lists = _subscribtions;
            if (lists.Count == 0)
                return;

            lock (lists)
            {
                foreach (var hotkeyId in lists.Keys)
                {
                    foreach (var item in lists[hotkeyId])
                    {
                        var weakAction = item.Action;

                        if (weakAction != null)
                            weakAction.MarkForDeletion();
                    }
                }
            }

            Cleanup();
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        private static void OnHotKeyPressed(HotkeyEventArgs args)
        {
            var lists = _subscribtions;
            if (lists.Count == 0 || !lists.ContainsKey(args.Hotkey.Id))
                return;

            lock (lists)
            {
                foreach (var item in lists[args.Hotkey.Id])
                {
                    var weakAction = item.Action;

                    if (weakAction != null && weakAction.IsAlive)
                    {
                        weakAction.Execute(args);
                        args.Handled = true;
                    }
                }
            }
        }

        private static void Cleanup()
        {
            var lists = _subscribtions;

            var listsToRemove = new List<ushort>();
            foreach (var list in lists)
            {
                var toRemove = list.Value.Where(item => item.Action == null || !item.Action.IsAlive).ToList();

                foreach (var recipient in toRemove)
                {
                    list.Value.Remove(recipient);
                }

                if (list.Value.Count == 0)
                {
                    listsToRemove.Add(list.Key);
                }
            }

            foreach (var key in listsToRemove)
            {
                lists.Remove(key);
            }
        }
        
        private static List<HotkeyManagerItem> GetSubscribtions(ushort key)
        {
            List<HotkeyManagerItem> subscribtions;
            if (!_subscribtions.TryGetValue(key, out subscribtions))
            {
                subscribtions = new List<HotkeyManagerItem>();
                _subscribtions.Add(key, subscribtions);
            }

            return subscribtions;
        }

        #endregion
        
        //======================================================
        #region _Fields_

        private static Dictionary<ushort, List<HotkeyManagerItem>> _subscribtions = new Dictionary<ushort, List<HotkeyManagerItem>>();

        #endregion
    }
}