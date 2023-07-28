using UiPath.CustomProxy.Contracts;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace UiPath.CustomProxy.Services
{
    internal class DependencyInjectionContainer : IServiceResolver, IServiceRegister
    {
        private readonly ConcurrentDictionary<Type, Func<IServiceResolver, object>> _registry = new ConcurrentDictionary<Type, Func<IServiceResolver, object>>();
        private readonly ConcurrentDictionary<Type, Lazy<object>> _singletonRegistry = new ConcurrentDictionary<Type, Lazy<object>>();

        T IServiceResolver.Get<T>() => ((IServiceResolver)this).Get(typeof(T)) as T;

        object IServiceResolver.Get(Type type)
        {
            if (_registry.TryGetValue(type, out var func))
                return func(this);

            if (_singletonRegistry.TryGetValue(type, out var lazy))
                return lazy.Value;

#if DEBUG
            Console.WriteLine($"Unregistered service requested: {type})");
            Debug.WriteLine($"Unregistered service requested: {type})");
#endif

            return null;
        }

        void IServiceRegister.Register<TContract>(Func<IServiceResolver, TContract> func)
        {
            _registry[typeof(TContract)] = func;
        }

        void IServiceRegister.RegisterSingleton<TContract>(Func<IServiceResolver, TContract> func)
        {
#if DEBUG
            if (typeof(IDisposable).IsAssignableFrom(typeof(TContract)))
                throw new InvalidOperationException("IDisposable should not be registered as Singleton");
#endif

            _singletonRegistry[typeof(TContract)] = new Lazy<object>(() => func(this));
        }
    }
}
