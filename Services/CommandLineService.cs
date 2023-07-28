using System;
using System.Collections.Generic;
using System.Linq;
using UiPath.CustomProxy.Contracts;
using UiPath.CustomProxy.Models;

namespace UiPath.CustomProxy.Services
{
    internal class CommandLineService : ICommandLineService
    {
        private readonly CommandLineData _data = new();
        private readonly CommandLineParser _commandLineParser;

        public CommandLineService()
        {
            _commandLineParser = new CommandLineParser(new Dictionary<string, Action<string>>
            {
                {"config", OnConfig}
            });
        }

        public ICommandLineData Data => _data;

        public void Parse(IEnumerable<string> args) => _commandLineParser.Parse(args.ToArray());

        private void OnConfig(string configSource) => _data.ConfigJsonPath = configSource;
    }
}
