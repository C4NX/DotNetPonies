using System.Net;
using System.Net.Http;

namespace DotNetPonies
{
    /// <summary>
    /// An interface to use all kinds of <see cref="HttpMessageHandler"/>, and for DotNetPonies it gives the <see cref="CookieContainer"/> to use.
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// Get the <see cref="CookieContainer"/> to use.
        /// </summary>
        /// <returns></returns>
        CookieContainer GetCookieContainer();

        /// <summary>
        /// Get the <see cref="HttpClient"/> instance to use.
        /// </summary>
        /// <returns></returns>
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