using System;
using System.Collections.Generic;
using System.Linq;
using UiPath.CustomProxy.Extensions;

namespace UiPath.CustomProxy.Services
{
    internal class CommandLineParser
    {
        private readonly IDictionary<string, Action<string>> _commands;

        public CommandLineParser(IDictionary<string, Action<string>> commands)
        {
            _commands = new Dictionary<string, Action<string>>(commands);
        }

        public bool Parse(params string[] args)
        {
            if (args.Length == 0)
                return true;

            if (RunProtocolHandler(args))
                return true;

            // handle command line args
            return HandleCommandLineArgs(args);
        }

        private bool RunProtocolHandler(IReadOnlyList<string> args)
        {
            if (args.Count != 1)
                return false;

            var url = args[0];
            if (!url.Contains("://"))
                return false;

            var protocolHandler = _commands.SingleOrDefault(x => x.Key.EndsWith("://") && url.StartsWith(x.Key));
            if (protocolHandler.Value == null)
                return false;

            protocolHandler.Value.Invoke(url);
            return true;
        }

        private bool HandleCommandLineArgs(IReadOnlyList<string> args)
        {
            var actions = new List<(Action<string> Handler, string Argument)>();
            for (var i = 0; i < args.Count; i += 2)
            {
                if (args.Count <= i + 1)
                    return false;

                var command = args[i].GetCommandLineParameter();
                if (!_commands.TryGetValue(command, out var handler))
                    return false;

                actions.Add((handler, args[i + 1]));
            }

            actions.ForEach(a => a.Handler(a.Argument));

            return true;
        }
    }
}