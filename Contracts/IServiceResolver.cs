using System;

namespace UiPath.CustomProxy.Contracts
{
    internal interface IServiceResolver
    {
        T Get<T>() where T : class;

        object Get(Type serviceType);
    }
}
