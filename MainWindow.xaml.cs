using UiPath.CustomProxy.ViewModels;
using System.Windows;
using UiPath.CustomProxy.Services;
using UiPath.CustomProxy.Contracts;
using System.Windows.Controls;

namespace UiPath.CustomProxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var windowFactory = ServiceResolver.Get<IWindowFactory>();
            DataContext = ViewModel = windowFactory.GetMainWindowViewModel();
            ViewModel.Window = this;

            InitializeComponent();

            TabControl.ItemsSource = ViewModel.TabDetails;
            TabControl.SelectedIndex = 0;

            WindowState = WindowState.Minimized;
        }

        internal MainWindowViewModel ViewModel { get; private set; }

        public void Log(string message) => Dispatcher.InvokeAsync(() => {
            OutputConsole.Inlines.Add(message);
            ScrollViewer.ScrollToBottom();
        });

        public void ToggleServerCommand(object sender, RoutedEventArgs e) => ViewModel.ToggleServer();

        public void LoadConfigCommand(object sender, RoutedEventArgs e) => ViewModel.LoadConfig();

        public void ResetConfigCommand(object sender, RoutedEventArgs e) => ViewModel.ResetConfig();

        public void ExportConfigCommand(object sender, RoutedEventArgs e) => ViewModel.ExportConfig();

        private void TextChangedEventHandler(object sender, TextChangedEventArgs args) => ViewModel.UpdateResponses();
    }
}
