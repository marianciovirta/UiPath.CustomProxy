using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using UiPath.CustomProxy.Contracts;
using UiPath.CustomProxy.Models;

namespace UiPath.CustomProxy.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IHttpServerService _httpServerService;
        private readonly IConfigService _configService;

        public MainWindowViewModel(IServiceResolver resolver)
        {
            _httpServerService = resolver.Get<IHttpServerService>();
            _configService = resolver.Get<IConfigService>();
        }

        public ObservableCollection<TabDetails> TabDetails { get; set; } = new();

        public MainWindow Window { get; set; }

        public bool IsWindowReady => Window != null;

        public bool IsServerStopped => !_httpServerService.IsServerRunning;

        public string ToggleServerTitle => IsServerStopped ? "Start server" : "Stop server";

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Log(string message) => Window?.Log(message);

        public void UpdateTabs(IEnumerable<TabDetails> tabs)
        {
            TabDetails.Clear();
            foreach (var tab in tabs)
                TabDetails.Add(tab);

            Window?.Dispatcher.InvokeAsync(() => NotifyUi());
        }

        public void ToggleServer()
        {
            if (IsServerStopped)
                _httpServerService.Start();
            else
                _httpServerService.Stop();
            NotifyUi();
        }

        public void ResetConfig() => _configService.ResetConfig();

        public void ExportConfig() => _configService.ExportConfig();

        public void UpdateResponses() => _configService.UpdateResponses(TabDetails);

        public void LoadConfig() => _configService.LoadConfig();

        private void NotifyUi()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsServerStopped)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TabDetails)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToggleServerTitle)));
        }
    }
}
