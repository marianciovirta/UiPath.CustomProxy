using UiPath.CustomProxy.ViewModels;

namespace UiPath.CustomProxy.Contracts
{
    internal interface ILoggingService
    {
        void Init(MainWindowViewModel viewModel);

        void Log(string message);
    }
}
