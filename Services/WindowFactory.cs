using UiPath.CustomProxy.Contracts;
using UiPath.CustomProxy.ViewModels;

namespace UiPath.CustomProxy.Services
{
    internal class WindowFactory : IWindowFactory
    {
        private readonly IServiceResolver _serviceResolver;
        private MainWindowViewModel _mainWindowViewModel;

        public WindowFactory(IServiceResolver resolver)
        {
            _serviceResolver = resolver;
        }

        public MainWindowViewModel GetMainWindowViewModel()
        {
            _mainWindowViewModel ??= new MainWindowViewModel(_serviceResolver);

            return _mainWindowViewModel;
        }
    }
}
