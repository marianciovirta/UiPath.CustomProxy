using UiPath.CustomProxy.ViewModels;
using System.Windows;
using UiPath.CustomProxy.Services;
using UiPath.CustomProxy.Contracts;
using System.Windows.Controls;
using System.Windows.Interop;
using System;

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

            Loaded += (sender, e) => RegisterHook(); ;
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

        private void RegisterHook()
        {
            var helper = new WindowInteropHelper(this);
            var hwnd = helper.EnsureHandle();
            var source = HwndSource.FromHwnd(hwnd);
            source?.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
            {
                ServiceResolver.Get<ILoggingService>().Log($"{msg}");
#pragma warning disable CS1522 // Empty switch block
                switch (msg)
                {
                }
#pragma warning restore CS1522 // Empty switch block

                return IntPtr.Zero;
            });
        }
    }
}
