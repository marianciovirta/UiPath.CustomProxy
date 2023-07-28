using UiPath.CustomProxy.ViewModels;

namespace UiPath.CustomProxy.Contracts
{
    internal interface IWindowFactory
    {
        MainWindowViewModel GetMainWindowViewModel();
    }
}
