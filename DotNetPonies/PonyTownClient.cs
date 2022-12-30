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
    public class PonyTownClient
    {
        private HttpClient _httpClient;

        public const string ApiVersion = "1gL2q7BKYG";
        public const string Api2Endpoint = "https://pony.town/api2/";


        public PonyTownClient(string apiVersion = ApiVersion, HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("api-version", apiVersion);
        }

        public async Task<GameStatus> GetStatus()
        {
            using (var response = await _httpClient.GetAsync($"{Api2Endpoint}game/status"))
            {
                string stringData = await response.Content.ReadAsStringAsync();
                CheckForJsonError(stringData);
                return ReadGameStatusData(Convert.FromBase64String(stringData));
            }
        }

        public GameStatus ReadGameStatusData(byte[] statusData)
        {
            GameStatus gameStatus = new GameStatus();

            using (MemoryStream memoryStream = new MemoryStream(statusData))
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    List<ServerStatus> serverList = new List<ServerStatus>();
                    string versionString = reader.ReadString();

                    // skip 3 bytes for now.
                    memoryStream.Position += 3;
                    //var tmpUInt16 = 179 ^ reader.ReadByte();
                    //var tmpByte = reader.ReadUInt16();

                    while (memoryStream.Position != memoryStream.Length)
                    {
                        ServerStatus server = new ServerStatus();
                        server.Id = reader.ReadString();

                        // read the offline, restrict byte
                        byte readedByte = reader.ReadByte();

                        int n = 54 ^ readedByte;
                        //offline: hC(n, 1),
                        //restrict: hC(n, 2)

                        // untested
                        bool offline = (n & 1) == 2;
                        bool restrict = (n & 2) == 2;
                        bool hasPercistantNotificationText = (n & 4) == 4;

                        server.Count = 52340 ^ reader.ReadUInt16();
                        serverList.Add(server);
                    }

                    gameStatus.SetStatus(versionString, serverList);
                }
            }
            return gameStatus;
        }

        public void CheckForJsonError(string data)
        {
            if (data[0] == '{')
                throw new PonyTownException(JsonConvert.DeserializeObject<ErrorModel>(data)?.Error ?? "No error message provided");
        }
    }
}
