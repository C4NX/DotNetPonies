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
using System.Threading.Tasks;

namespace DotNetPonies
{
    /// <summary>
    /// The non-official wrapper for Pony Town API.
    /// </summary>
    public class PonyTownClient
    {
        private HttpClient _httpClient;
        private CookieContainer _cookieContainer;

        /// <summary>
        /// The api-version header, last updated 31/01/2023
        /// </summary>
        public const string ApiVersion = "cHMqbcVtri";
        
        /// <summary>
        /// ApiV2 Endpoint 
        /// </summary>
        public const string Api2Endpoint = "https://pony.town/api2/";

        /// <summary>
        /// ApiV1 Endpoint 
        /// </summary>
        public const string Api1Endpoint = "https://pony.town/api/";

        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="apiVersion"></param>
        public PonyTownClient(string apiVersion = ApiVersion) : this(new StandardHttpClient(), apiVersion)
        {
        }

        /// <summary>
        /// Create a new PonyTown client with the specified <see cref="IHttpClient"/> and apiVersion
        /// </summary>
        /// <param name="httpClient">The <see cref="IHttpClient"/> to use or null</param>
        /// <param name="apiVersion">The apiVersion to use</param>
        public PonyTownClient(IHttpClient httpClient, string apiVersion = ApiVersion)
        {
            _httpClient = httpClient.GetClient();
            _cookieContainer = httpClient.GetCookieContainer();
            
            _httpClient.DefaultRequestHeaders.Add("api-version", apiVersion);
            _httpClient.DefaultRequestHeaders.Add("origin", "https://pony.town");
            _httpClient.DefaultRequestHeaders.Add("referer", "https://pony.town");
        }

        /// <summary>
        /// Get a <see cref="GameStatus"/> object that represents the status of the current game, it contains all the available servers, game version and others...
        /// </summary>
        /// <returns>The <see cref="GameStatus"/> object.</returns>
        public async Task<GameStatus> GetStatusAsync()
        {
            using (var response = await _httpClient.GetAsync($"{Api2Endpoint}game/status"))
            {
                string stringData = await response.Content.ReadAsStringAsync();
                CheckForJsonError(stringData);
                return GameStatus.FromData(Convert.FromBase64String(stringData));
            }
        }

        /// <summary>
        /// Login to pony town with a cookie, be careful, it will probably not last long until it expires.
        /// </summary>
        /// <param name="connect_sid">The 'connect_sid' cookie</param>
        /// <param name="remember_me">The 'remember_me' cookie (not required)</param>
        /// <returns>This <see cref="PonyTownClient"/> instance</returns>
        public PonyTownClient LoginWithCookie(string connect_sid, string? remember_me = null)
        {
            Uri hostUri = new Uri("https://pony.town");
            _cookieContainer.Add(hostUri, new Cookie("connect.sid", connect_sid));
            if(remember_me != null)
                _cookieContainer.Add(hostUri, new Cookie("remember_me", remember_me));
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
            using (var resp = await _httpClient.PostAsync($"{Api1Endpoint}account-characters",
                new FormUrlEncodedContent(new Dictionary<string, string>() { { "accountId", accountId }, { "accountName", accountName } })))
            {
                if (resp.StatusCode == HttpStatusCode.Forbidden)
                    throw new PonyTownForbiddenException();

                resp.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<ReadOnlyCollection<Pony>>(await resp.Content.ReadAsStringAsync());
            }
        }

        private void CheckForJsonError(string data)
        {
            if (data.Length > 0 && data[0] == '{')
                throw new PonyTownException(JsonConvert.DeserializeObject<ErrorModel>(data)?.Error ?? "No error message provided");
        }
    }
}
