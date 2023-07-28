using System;

namespace UiPath.CustomProxy.Contracts
{
    internal interface IServiceRegister
    {
        void Register<TContract>(Func<IServiceResolver, TContract> func) where TContract : class;

        void RegisterSingleton<TContract>(Func<IServiceResolver, TContract> func) where TContract : class;
    }
}
