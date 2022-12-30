using System.Net;
using System.Net.Http;

namespace DotNetPonies
{
    public interface IHttpClient
    {
        CookieContainer GetCookieContainer();
        HttpClient GetClient();
    }

    /// <summary>
    /// A simple implementation of <see cref="IHttpClient"/> for .NET Standard
    /// </summary>
    public class StandardHttpClient : IHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        public StandardHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            _cookieContainer = new CookieContainer();
            handler.CookieContainer = _cookieContainer;
            _httpClient = new HttpClient(handler);
        }

        public HttpClient GetClient()
            => _httpClient;

        public CookieContainer GetCookieContainer()
            => _cookieContainer;
    }
}