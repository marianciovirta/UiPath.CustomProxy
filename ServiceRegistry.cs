using UiPath.CustomProxy.Contracts;
using UiPath.CustomProxy.Services;

namespace UiPath.CustomProxy
{
    internal static class ServiceRegistry
    {
        public static void Register(IServiceRegister register)
        {
            register.RegisterSingleton<ILoggingService>(r => new LoggingService());
            register.RegisterSingleton<ICommandLineService>(r => new CommandLineService());
            register.RegisterSingleton<IFileSystemService>(r => new FileSystemService());
            register.RegisterSingleton<IConfigService>(r => new ConfigService(r));
            register.RegisterSingleton<IHttpServerService>(r => new HttpServerService(r));
            register.RegisterSingleton<IWindowFactory>(r => new WindowFactory(r));
        }
    }
}