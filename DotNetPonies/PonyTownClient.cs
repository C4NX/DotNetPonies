using DotNetPonies.Exceptions;
using DotNetPonies.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
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

        /// <summary>
        /// The api-version header, last updated 30/12/2022
        /// </summary>
        public const string ApiVersion = "1gL2q7BKYG";
        /// <summary>
        /// ApiV2 Endpoint 
        /// </summary>
        public const string Api2Endpoint = "https://pony.town/api2/";


        /// <summary>
        /// Create a new PonyTown Client with the specified apiVersion and httpClient
        /// </summary>
        /// <param name="apiVersion">The apiVersion to use</param>
        /// <param name="httpClient">The httpClient to use or null</param>
        public PonyTownClient(string apiVersion = ApiVersion, HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("api-version", apiVersion);
        }

        public async Task<GameStatus> GetStatusAsync()
        {
            using (var response = await _httpClient.GetAsync($"{Api2Endpoint}game/status"))
            {
                string stringData = await response.Content.ReadAsStringAsync();
                CheckForJsonError(stringData);
                return GameStatus.FromData(Convert.FromBase64String(stringData));
            }
        }

        public void CheckForJsonError(string data)
        {
            if (data[0] == '{')
                throw new PonyTownException(JsonConvert.DeserializeObject<ErrorModel>(data)?.Error ?? "No error message provided");
        }
    }
}
