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
        public PonyTownClient(string? apiVersion = null) : this(new StandardHttpClient(), apiVersion)
        {
        }

        /// <summary>
        /// Create a new PonyTown client with the specified <see cref="IHttpClient"/> and apiVersion
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> to use or null</param>
        /// <param name="apiVersion">The apiVersion to use</param>
        public PonyTownClient(IHttpClient httpClient, string? apiVersion = null)
        {
            _httpClient = httpClient.GetClient();
            _cookieContainer = httpClient.GetCookieContainer();
            if (apiVersion != null) ApiVersionHeader = apiVersion;
        }

        /// <summary>
        /// Get a <see cref="GameStatus"/> object that represents the game status with servers, messages, etc...
        /// </summary>
        /// <exception cref="PonyTownException">Response is not successful</exception>
        /// <returns>The loaded <see cref="GameStatus"/> object</returns>
        public async Task<GameStatus> GetStatusAsync()
        {
            using var response = await _httpClient.GetAsync($"{Api2Endpoint}game/status");
            return GameStatus.FromData(Convert.FromBase64String(await GetResponseOrThrowException(response)));
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

        public async Task ResolveApiVersionAsync()
        {
            const string regexQuery = "const Ew=\"([^\"]*)";
            using var responseMessage =
                await _httpClient.GetAsync($"{PonyTownBaseUrl}assets/scripts/bootstrap-7052b2bb32.js");
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            var match = Regex.Match(responseString, regexQuery);
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