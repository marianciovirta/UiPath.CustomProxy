using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace UiPath.CustomProxy.Extensions
{
    internal static class HttpExtensions
    {
        public static HttpRequestMessage ToHttpRequestMessage(this HttpListenerRequest listenerRequest, string baseUrl, string path)
        {
            var requestMessage = new HttpRequestMessage();

            CopyRequestContent(listenerRequest, requestMessage);
            CopyRequestHeaders(listenerRequest, requestMessage);
            var uri = new Uri(baseUrl + path);
            requestMessage.RequestUri = uri;
            requestMessage.Headers.Host = uri.Host;
            requestMessage.Method = new HttpMethod(listenerRequest.HttpMethod);

            return requestMessage;
        }

        public static async Task CopyFrom(this HttpListenerResponse listenerResponse, HttpResponseMessage responseMessage)
        {
            listenerResponse.StatusCode = (int)responseMessage.StatusCode;
            CopyResponseHeaders(listenerResponse, responseMessage);
            await CopyResponseContent(listenerResponse, responseMessage);
        }

        private static void CopyRequestContent(HttpListenerRequest listenerRequest, HttpRequestMessage requestMessage)
        {
            var requestMethod = new HttpMethod(listenerRequest.HttpMethod);
            if (requestMethod != HttpMethod.Get && requestMethod != HttpMethod.Head && requestMethod != HttpMethod.Delete && requestMethod != HttpMethod.Trace)
            {
                var streamContent = new StreamContent(listenerRequest.InputStream);
                requestMessage.Content = streamContent;
            }
        }

        private static void CopyRequestHeaders(HttpListenerRequest listenerRequest, HttpRequestMessage requestMessage)
        {
            foreach (string key in listenerRequest.Headers)
                if (!requestMessage.Headers.TryAddWithoutValidation(key, listenerRequest.Headers[key].Split(",").ToArray()))
                    requestMessage.Content?.Headers.TryAddWithoutValidation(key, listenerRequest.Headers[key].Split(",").ToArray());
        }

        private static void CopyResponseHeaders(HttpListenerResponse listenerResponse, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
                listenerResponse.Headers[header.Key] = string.Join(",", header.Value.ToArray());

            foreach (var header in responseMessage.Content.Headers)
                listenerResponse.Headers[header.Key] = string.Join(",", header.Value.ToArray());

            listenerResponse.Headers.Remove("transfer-encoding");
        }

        private static async Task CopyResponseContent(HttpListenerResponse listenerResponse, HttpResponseMessage responseMessage)
        {
            listenerResponse.ContentType = responseMessage.Content?.Headers?.ContentType?.MediaType;
            listenerResponse.ContentLength64 = responseMessage.Content?.Headers?.ContentLength ?? 0;

            if (responseMessage.Content != null)
                await responseMessage.Content.CopyToAsync(listenerResponse.OutputStream);
        }
    }
}
