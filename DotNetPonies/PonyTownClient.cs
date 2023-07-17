using DotNetPonies.Exceptions;
using DotNetPonies.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetPonies
{
    /// <summary>
    /// The non-official <c>PonyTown API Client</c> for .NET<br />
    /// <code>
    ///     var client = new PonyTownClient();
    ///     await client.ResolveApiVersionAsync();
    ///     var gameStatus = await client.GetStatusAsync();
    /// </code>
    /// </summary>
    public class PonyTownClient
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        private readonly PonyTownScope _scope;
        
        /// <summary>
        /// The Api Version Header Name (api-version)
        /// </summary>
        public const string ApiVersionHeaderName = "api-version";

        /// <summary>
        /// The PonyTown Base Url (https://pony.town/)
        /// </summary>
        public const string PonyTownBaseUrl = "https://pony.town/";

        /// <summary>
        /// ApiV2 Endpoint (https://pony.town/api2/)
        /// </summary>
        public const string Api2Endpoint = "https://pony.town/api2/";

        /// <summary>
        /// ApiV1 Endpoint (https://pony.town/api/)
        /// </summary>
        public const string Api1Endpoint = "https://pony.town/api/";

        /// <summary>
        /// Get or Set the Api Version Header (required to successfully call the API)<br/>To resolve the ApiVersion, use <see cref="ResolveApiVersionAsync"/>
        /// </summary>
        public string ApiVersionHeader
        {
            get => string.Join(",", _httpClient.DefaultRequestHeaders.GetValues(ApiVersionHeaderName));
            set
            {
                _httpClient.DefaultRequestHeaders.Remove(ApiVersionHeaderName);
                _httpClient.DefaultRequestHeaders.Add(ApiVersionHeaderName, value);
            }
        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="apiVersion"></param>
        public PonyTownClient(PonyTownScope? ponyTownScope = null) : this(new StandardHttpClient(), ponyTownScope)
        {
        }

        /// <summary>
        /// Create a new PonyTown client with the specified <see cref="IHttpClient"/> and apiVersion
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> to use or null</param>
        /// <param name="apiVersion">The apiVersion to use</param>
        public PonyTownClient(IHttpClient httpClient, PonyTownScope? ponyTownScope = null)
        {
            _httpClient = httpClient.GetClient();
            _cookieContainer = httpClient.GetCookieContainer();
            _scope = ponyTownScope ?? new PonyTownScope();
            if (_scope.DefaultApiVersion != null) ApiVersionHeader = _scope.DefaultApiVersion;
        }

        /// <summary>
        /// Login to pony town with a cookie, be careful, it will probably not last long until it expires.
        /// </summary>
        /// <param name="connect_sid">The 'connect_sid' cookie</param>
        /// <param name="remember_me">The 'remember_me' cookie (not required)</param>
        /// <returns>This <see cref="PonyTownClient"/> instance</returns>
        public PonyTownClient LoginWithCookie(string connect_sid, string? remember_me = null)
        {
            Uri hostUri = new Uri(PonyTownBaseUrl);
            _cookieContainer.Add(hostUri, new Cookie("connect.sid", connect_sid));
            if (remember_me != null) _cookieContainer.Add(hostUri, new Cookie("remember_me", remember_me));
            return this;
        }

        /// <summary>
        /// Get a <see cref="OfflineGameStatus"/> object that represents the game status with servers, messages, etc...
        /// </summary>
        /// <exception cref="PonyTownException">Response is not successful</exception>
        /// <returns>The loaded <see cref="OfflineGameStatus"/> object</returns>
        public async Task<OfflineGameStatus> GetStatusAsync()
        {
            using var response = await _httpClient.GetAsync($"{Api2Endpoint}game/status");
            return OfflineGameStatus.FromData(Convert.FromBase64String(await GetResponseOrThrowException(response)), _scope);
        }

        /// <summary>
        /// Get a <see cref="IReadOnlyCollection{T}"/> of <see cref="Pony"/> owned by a player.
        /// </summary>
        /// <param name="accountId">The player account id</param>
        /// <param name="accountName">The player account name</param>
        /// <returns>A <see cref="IReadOnlyCollection{T}"/> of <see cref="Pony"/></returns>
        /// <exception cref="PonyTownForbiddenException">Response is forbidden</exception>
        public async Task<IReadOnlyCollection<Pony>?> GetCharactersAsync(string accountId, string accountName)
        {
            using var resp = await _httpClient.PostAsync($"{Api1Endpoint}account-characters",
                new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "accountId", accountId }, { "accountName", accountName }
                }));
            if (resp.StatusCode == HttpStatusCode.Forbidden) throw new PonyTownForbiddenException();
            resp.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<ReadOnlyCollection<Pony>>(await resp.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Try to resolve the Api Version Header by getting the bootstrap javascript file and searching for the api version.
        /// </summary>
        /// <exception cref="PonyTownException">Could not resolve bootstrap javascript file or api version in it</exception>
        public async Task ResolveApiVersionAsync()
        {
            const string regexBootstrap = "/assets/scripts/bootstrap-([a-z0-9]*)\\.js";
            using var mainResponseMessage = await _httpClient.GetAsync(PonyTownBaseUrl);
            var mainResponseString = await mainResponseMessage.Content.ReadAsStringAsync();
            var bootstrapMatch = Regex.Match(mainResponseString, regexBootstrap);
            if (!bootstrapMatch.Success) throw new PonyTownException("Could not resolve bootstrap javascript file");
            using var responseMessage = await _httpClient.GetAsync(PonyTownBaseUrl + bootstrapMatch);
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            var match = Regex.Match(responseString, _scope.RegexApiVersion);
            if (match.Success) ApiVersionHeader = match.ToString()[(match.ToString().IndexOf('"') + 1)..];
            else throw new PonyTownException("Could not resolve api version");
        }

        private async Task<string> GetResponseOrThrowException(HttpResponseMessage response)
        {
            var stringData = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new PonyTownException(JsonConvert.DeserializeObject<ErrorModel>(stringData)?.Error ??
                                            "No error message provided");
            return stringData;
        }
    }
}