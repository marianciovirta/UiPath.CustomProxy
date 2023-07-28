using UiPath.CustomProxy.Contracts;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Linq;
using UiPath.CustomProxy.Extensions;
using System.Net.Http;

namespace UiPath.CustomProxy.Services
{
    internal class HttpServerService : IHttpServerService
    {
        private const string _defaultHtml = "<html><body>Server running</body></html>";
        private const string _proxyEndpoint = "https://alpha.uipath.com/semanticproxy_/api/";

        private readonly IConfigService _configService;
        private readonly ILoggingService _loggingService;

        private readonly int _port;
        private readonly string _serverUrl;
        private readonly string _root;

        private CancellationTokenSource _cancellationTokenSource;
        private HttpListener _listener;
        private bool _serverRunning;
        

        public HttpServerService(IServiceResolver resolver)
        {
            _root = "/api";
            _port = 7071;
            _serverUrl = $"http://localhost:{_port}{_root}/";
            _loggingService = resolver.Get<ILoggingService>();
            _configService = resolver.Get<IConfigService>();
        }

        public bool IsServerRunning => _serverRunning;

        public void Start()
        {
            // port is already used, do not start the server
            if (_serverRunning || PortInUse(_port))
                return;
            
            _serverRunning = true;

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            Task.Run(() => {
                Listen(cancellationToken);
            }, cancellationToken);
        }

        public void Stop()
        {
            if (!_serverRunning)
                return;

            _loggingService.Log($"Server stopped");

            _serverRunning = false;
            _listener?.Stop();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void Listen(CancellationToken token)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_serverUrl);
            _listener.Start();

            _loggingService.Log("Server started");

            while (true)
            {
                if (token.IsCancellationRequested || !_serverRunning)
                    break;

                try
                {
                    var context = _listener.GetContext();
                    Process(context);
                }
                catch
                {
                }
            }
        }

        private async Task Process(HttpListenerContext context)
        {
            try
            {
                var absolutePath = context.Request.Url.AbsolutePath;
                var endpoint = absolutePath.RemovePrefix(_root).RemovePrefix("/");

                var responseContent = "";
                if (endpoint.Equals(""))
                {
                    var buffer = Encoding.UTF8.GetBytes(_defaultHtml);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);

                    _loggingService.Log($"Request: [{context.Request.HttpMethod} {absolutePath}]");
                }
                else if (_configService.TryGetValue(endpoint, out var content) && !string.IsNullOrEmpty(content))
                {
                    var buffer = Encoding.UTF8.GetBytes(content);
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);

                    responseContent = content;

                    _loggingService.Log($"Request: [{context.Request.HttpMethod} {absolutePath}]");
                }
                else
                {
                    var client = new HttpClient
                    {
                        BaseAddress = new Uri(_proxyEndpoint)
                    };
                    var httpRequest = context.Request.ToHttpRequestMessage(_proxyEndpoint, endpoint);

                    var requestContent = "";
                    if (httpRequest.Content != null)
                        requestContent = await httpRequest.Content.ReadAsStringAsync();

                    _loggingService.Log($"Request: [{context.Request.HttpMethod} {absolutePath}]" + (!string.IsNullOrEmpty(requestContent) ? " : " + requestContent : ""));

                    using var response = await client.SendAsync(httpRequest);
                    await context.Response.CopyFrom(response);

                    if (response.Content != null)
                        responseContent = await response.Content.ReadAsStringAsync();
                }

                _loggingService.Log($"Response: [{context.Request.HttpMethod} {endpoint}] {context.Response.StatusCode}" + (!string.IsNullOrEmpty(responseContent) ? " : " + responseContent : ""));
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                _loggingService.Log($"Process: exception {ex.Message}");
            }
        }

        private static bool PortInUse(int port)
            => IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(x => x.Port == port);
    }
}
