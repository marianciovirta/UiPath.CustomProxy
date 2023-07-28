using UiPath.CustomProxy.Contracts;
using System;

namespace UiPath.CustomProxy.Services
{
    internal class ServiceResolver
    {
        private static readonly Lazy<ServiceResolver> s_lazyServiceFactory = new Lazy<ServiceResolver>(() => new ServiceResolver());

        public static IServiceResolver Instance => s_lazyServiceFactory.Value._dependencyInjectionContainer;

        public static IServiceRegister Register => s_lazyServiceFactory.Value._dependencyInjectionContainer;

        public static T Get<T>() where T : class => Instance.Get<T>();

        private readonly DependencyInjectionContainer _dependencyInjectionContainer;

        private ServiceResolver()
        {
            _dependencyInjectionContainer = new DependencyInjectionContainer();
            ServiceRegistry.Register(_dependencyInjectionContainer);
        }
    }
}
