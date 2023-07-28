using System.Collections.Generic;
using UiPath.CustomProxy.Models;

namespace UiPath.CustomProxy.Contracts
{
    internal interface ICommandLineService
    {
        void Parse(IEnumerable<string> args);

        ICommandLineData Data { get; }
    }
}
