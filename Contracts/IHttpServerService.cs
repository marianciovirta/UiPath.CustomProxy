namespace UiPath.CustomProxy.Contracts
{
    internal interface IHttpServerService
    {
        void Start();

        void Stop();

        bool IsServerRunning { get; }
    }
}
