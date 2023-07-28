using System.Collections.Generic;
using UiPath.CustomProxy.Models;
using UiPath.CustomProxy.ViewModels;

namespace UiPath.CustomProxy.Contracts
{
    internal interface IConfigService
    {
        void Init(MainWindowViewModel viewModel);

        void ResetConfig();

        void LoadConfig();

        void ExportConfig();

        void UpdateResponses(IEnumerable<TabDetails> tabs);

        bool TryGetValue(string endpoint, out string content);
    }
}
