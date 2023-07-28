namespace UiPath.CustomProxy.Models
{
    internal interface ICommandLineData
    {
        string ConfigJsonPath { get; }
    }

    internal class CommandLineData : ICommandLineData
    {
        public string ConfigJsonPath { get; set; }
    }
}