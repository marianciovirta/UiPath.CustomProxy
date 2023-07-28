using UiPath.CustomProxy.Contracts;
using UiPath.CustomProxy.Services;
using System.Windows;

namespace UiPath.CustomProxy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            HandleCommandLine(e.Args);

            Init();
            StartServer();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            StopServer();
        }

        private static void Init()
        {
            var mainWindowViewModel = ServiceResolver.Instance.Get<IWindowFactory>().GetMainWindowViewModel();

            var logging = ServiceResolver.Instance.Get<ILoggingService>();
            logging.Init(mainWindowViewModel);

            var config = ServiceResolver.Instance.Get<IConfigService>();
            config.Init(mainWindowViewModel);
        }

        private static void HandleCommandLine(string[] args)
        {
            var cls = ServiceResolver.Instance.Get<ICommandLineService>();
            cls.Parse(args);
        }

        private static void StartServer() => ServiceResolver.Instance.Get<IHttpServerService>().Start();

        private static void StopServer() => ServiceResolver.Instance.Get<IHttpServerService>().Stop();
    }
}
