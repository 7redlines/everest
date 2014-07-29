using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Se7enRedLines.UI
{
    public class SingletonAppStartupEventArgs : EventArgs
    {
        //======================================================
        #region _Constructors_

        public SingletonAppStartupEventArgs(string[] args)
        {
            Args = args;
        }

        #endregion

        //======================================================
        #region _Public properties_

        public string[] Args { get; protected set; }

        #endregion
    }

    public class SingletonApplication : Application
    {
        //======================================================
        #region _Public properties_

        public event EventHandler<SingletonAppStartupEventArgs> SingletonStartup;

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Make();
        }

        /// <summary>
        /// Processing single instance in <see cref="SingletonAppMode"/> <see cref="SingletonAppMode.ForEveryUser"/> mode.
        /// </summary>
        public static void Make()
        {
            Make(SingletonAppMode.ForEveryUser);
        }

        /// <summary>
        /// Processing single instance.
        /// </summary>
        /// <param name="singletonAppMode"></param>
        public static void Make(SingletonAppMode singletonAppMode)
        {
            var appName = Application.Current.GetType().Assembly.ManifestModule.ScopeName;

            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();

            var keyUserName = windowsIdentity != null ? windowsIdentity.User.ToString() : string.Empty;

            // Be careful! Max 260 chars!
            var eventWaitHandleName = string.Format(
                "{0}{1}",
                appName,
                singletonAppMode == SingletonAppMode.ForEveryUser ? keyUserName : string.Empty);

            try
            {
                using (var eventWaitHandle = EventWaitHandle.OpenExisting(eventWaitHandleName))
                {
                    SaveArgs();

                    // It informs first instance about other startup attempting.
                    eventWaitHandle.Set();
                }

                // Let's terminate this posterior startup.
                // For that exit no interceptions.
                Environment.Exit(0);
            }
            catch
            {
                // It's first instance.
                SaveArgs();

                // Register EventWaitHandle.
                using (var eventWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, eventWaitHandleName))
                {
                    ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, OtherInstanceAttemptedToStart,
                        null, Timeout.Infinite, false);
                }

                RemoveApplicationsStartupDeadlockForStartupCrushedWindows();
            }
        }

        private static void OtherInstanceAttemptedToStart(object state, bool timedOut)
        {
            RemoveApplicationsStartupDeadlockForStartupCrushedWindows();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var app = (SingletonApplication) Application.Current;

                    string[] args;
                    if (TryGetArgs(out args))
                    {
                        if (app.SingletonStartup != null)
                            app.SingletonStartup(app, new SingletonAppStartupEventArgs(args));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }));
        }

        internal static DispatcherTimer AutoExitAplicationIfStartupDeadlock;

        /// <summary>
        /// Sometimes app crash on startup.
        /// </summary>
        private static void RemoveApplicationsStartupDeadlockForStartupCrushedWindows()
        {
            var app = Application.Current;

            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                AutoExitAplicationIfStartupDeadlock =
                    new DispatcherTimer(
                        TimeSpan.FromSeconds(6),
                        DispatcherPriority.ApplicationIdle,
                        (o, args) =>
                        {
                            if (app.Windows.OfType<Window>().Count(window => !double.IsNaN(window.Left)) == 0)
                            {
                                // For that exit no interceptions.
                                Environment.Exit(0);
                            }

                        }, app.Dispatcher);
            }), DispatcherPriority.ApplicationIdle);
        }

        private static void SaveArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (1 < args.Length)
            {
                using (var store = IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Assembly,
                    null, null))
                {

                    using (var storageStream = new IsolatedStorageFileStream(
                        _appStorageFileName, FileMode.Create, store))
                    {
                        using (var writer = new StreamWriter(storageStream))
                            writer.Write(string.Join(" ", args));
                    }
                }
            }
        }

        private static bool TryGetArgs(out string[] args)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Assembly,
                    null, null))
                {

                    using (var storageStream = new IsolatedStorageFileStream(
                        _appStorageFileName, FileMode.OpenOrCreate, store))
                    {
                        using (var writer = new StreamReader(storageStream))
                        {
                            var str = writer.ReadToEnd();
                            args = str.Split(' ');
                        }
                    }

                    store.DeleteFile(_appStorageFileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            args = null;
            return false;
        }

        #endregion

        //======================================================
        #region _Fields_

        private static readonly String _appStorageFileName = "SomeFileInTheRoot.txt";

        #endregion
    }

    public enum SingletonAppMode
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        NotInited = 0,

        /// <summary>
        /// Every user can have own single instance.
        /// </summary>
        ForEveryUser,
    }
}