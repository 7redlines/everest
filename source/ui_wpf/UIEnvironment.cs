using GalaSoft.MvvmLight;

namespace Se7enRedLines.UI
{
    public static class UIEnvironment
    {
        public static bool IsInDesignMode
        {
            get { return ViewModelBase.IsInDesignModeStatic; }
        }
    }
}