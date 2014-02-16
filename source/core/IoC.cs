using GalaSoft.MvvmLight.Ioc;

namespace Se7enRedLines
{
    public static class IoC
    {
        //======================================================
        #region _Constructors_

        static IoC()
        {
            _ioc = new SimpleIoc();
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        public static void Register<TInterface, TInstance>(TInstance instance)
            where TInstance : class, TInterface
            where TInterface : class
        {
            _ioc.Register<TInterface>(() => instance, true);
        }

        public static void Register<TInterface, TInstance>() 
            where TInstance : class, TInterface
            where TInterface : class
        {
            _ioc.Register<TInterface, TInstance>();
        }

        public static void Register<TInterface, TInstance>(bool singleton)
            where TInstance : class, TInterface
            where TInterface : class
        {
            _ioc.Register<TInterface, TInstance>(singleton);
        }

        public static TInterface Resolve<TInterface>()
        {
            return _ioc.GetInstance<TInterface>();
        }

        #endregion

        //======================================================
        #region _Private, protected, internal fields_

        private static readonly SimpleIoc _ioc;

        #endregion
    }
}