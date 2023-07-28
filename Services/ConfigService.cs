using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UiPath.CustomProxy.Contracts;
using UiPath.CustomProxy.Models;
using UiPath.CustomProxy.ViewModels;

namespace UiPath.CustomProxy.Services
{
    internal class ConfigService : IConfigService
    {
        private readonly ICommandLineService _commandLineService;
        private readonly IFileSystemService _fileSystemService;
        private readonly ILoggingService _loggingService;

        private readonly Dictionary<string, string> _responses = new()
        {
            { "processCV", "" },
            { "processOCR", "" },
            { "processDU", "" },
            { "processAWS", "" },
            { "processGCP", "" },
            { "processGPT", "" },
            { "transformations", "" },
            { "valueMatching", "" },
            { "schemaMatching", "" },
            { "serviceVersion", "" },
            { "clientVersion", "" },
            { "feedback", "" },
            { "newsfeed", "" },
            { "telemetry", "" },
            { "exception", "" },
            { "dataCollection", "" },
            { "domRelations", "" },
            { "processQA", "" },
            { "sourceType", "" },
            { "configuration", "" },
            { "consumption", "" },
            { "consumption/display", "" },
            { "registration", "" },
            { "registration/loginNotification", "" }
        };

        private MainWindowViewModel _windowViewModel;

        public ConfigService(IServiceResolver resolver)
        {
            _fileSystemService = resolver.Get<IFileSystemService>();
            _commandLineService = resolver.Get<ICommandLineService>();
            _loggingService = resolver.Get<ILoggingService>();
        }

        public void Init(MainWindowViewModel viewModel)
        {
            _windowViewModel = viewModel;

            LoadConfig(_commandLineService.Data.ConfigJsonPath);
        }

        public void ResetConfig()
        {
            foreach (var j in _responses)
                _responses[j.Key] = "";
            UpdateTabs();
        }

        public void LoadConfig()
        {
            if (!_fileSystemService.TrySelectDirectory(out string path))
                return;

            LoadConfig(path);
        }

        public void ExportConfig()
        {
            if (!_fileSystemService.TrySelectDirectory(out string path))
                return;

            if (!_fileSystemService.DirectoryExists(path))
                _fileSystemService.CreateDirectory(path);

            _loggingService.Log($"Exporting config to {path}");

            // create endpoint configs
            foreach (var c in _responses)
                if (!string.IsNullOrEmpty(c.Value))
                {
                    var configPath = Path.Combine(path, c.Key + ".json");
                    if (_fileSystemService.TryWriteAllText(configPath, c.Value))
                        _loggingService.Log($"Exported config for {c.Key}: {configPath}");
                }
        }

        public void UpdateResponses(IEnumerable<TabDetails> tabs)
        {
            foreach (var t in tabs)
                _responses[t.Name] = t.Content;
        }

        public bool TryGetValue(string endpoint, out string content) => _responses.TryGetValue(endpoint, out content);

        public static JsonSerializerSettings GetSerializerSettings()
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            jsonSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return jsonSerializerSettings;
        }

        private void LoadConfig(string configPath)
        {
            if (!string.IsNullOrEmpty(configPath) && _fileSystemService.DirectoryExists(configPath))
            {
                if (!_fileSystemService.TryGetFiles(configPath, "*.json", out string[] files))
                    return;

                _loggingService.Log($"Loaded config: {configPath}");
                foreach (var filePath in files)
                {
                    if (!_fileSystemService.TryGetFileNameWithoutExtension(filePath, out string endpoint))
                        continue;

                    if (_fileSystemService.TryReadAllText(filePath, out var fileContent))
                    {
                        _responses[endpoint] = fileContent;
                        _loggingService.Log($"Loaded config for {endpoint}: {filePath}");
                    }
                }
            }

            UpdateTabs();
        }

        private void UpdateTabs()
           => _windowViewModel.UpdateTabs(_responses.Select(r => new TabDetails() { Name = r.Key, Content = r.Value }));
    }
}
