using System;

namespace Se7enRedLines
{
    public class IoC
    {
        private static class IoCSingleton
        {
            public static readonly IoC Instance = new IoC();
        }

        //======================================================
        #region _Constructors_

        public IoC()
        {
            _resolver = new DependencyResolver();
        }

        #endregion

        //======================================================
        #region _Public properties_

        public static IoC Current
        {
            get { return IoCSingleton.Instance; }
        }

        #endregion

        //======================================================
        #region _Public methods_
        
        //[DebuggerStepThrough]
        public void Register<T>(T instance) where T : class
        {
            _resolver.Register(instance);
        }

        public void Register<T>(T instance, string name) where T : class
        {
            _resolver.Register(instance, name);
        }

        public void Register<T>(Lifetime lifetime) where T : class
        {
            _resolver.Register<T>(lifetime);
        }

        public void Register<T>(Lifetime lifetime, string name) where T : class
        {
            _resolver.Register<T>(lifetime, name);
        }

        //[DebuggerStepThrough]
        /// <summary>
        /// Register with <see cref="Lifetime.Singleton"/> lifetime.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="instanceType"></param>
        public void Register(Type interfaceType, Type instanceType)
        {
            _resolver.Register(interfaceType, instanceType, Lifetime.Singleton);
        }

        public void Register(Type interfaceType, Type instanceType, Lifetime lifetime)
        {
            _resolver.Register(interfaceType, instanceType, lifetime);
        }

        public void Register(Type interfaceType, Type instanceType, Lifetime lifetime, string name)
        {
            _resolver.Register(interfaceType, instanceType, lifetime, name);
        }

        //[DebuggerStepThrough]
        /// <summary>
        /// Registers with <see cref="Lifetime.Singleton"/> lifetime.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        public void Register<TInterface, TInstance>()
            where TInstance : class, TInterface
        {
            _resolver.Register<TInterface, TInstance>(Lifetime.Singleton);
        }

        //[DebuggerStepThrough]
        public void Register<TInterface, TInstance>(Lifetime lifetime)
            where TInstance : class, TInterface
        {
            _resolver.Register<TInterface, TInstance>(lifetime);
        }

        public void Register<TInterface, TInstance>(Lifetime lifetime, string name)
            where TInstance : class, TInterface
        {
            _resolver.Register<TInterface, TInstance>(lifetime, name);
        }

        //[DebuggerStepThrough]
        public void Inject<T>(T existing)
               where T : class
        {
            _resolver.Inject(existing);
        }

        //[DebuggerStepThrough]
        public T Resolve<T>() where T : class
        {
            return _resolver.Resolve<T>();
        }

        public T Resolve<T>(string name) where T : class
        {
            return _resolver.Resolve<T>(name);
        }

        //[DebuggerStepThrough]
        public object Resolve(Type type)
        {
            return _resolver.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return _resolver.Resolve(type, name);
        }

        #endregion

        //======================================================
        #region _Private, protected, internal fields_

        private readonly IDependencyResolver _resolver;

        #endregion
    }
}