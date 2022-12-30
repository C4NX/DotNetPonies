using Newtonsoft.Json;

namespace DotNetPonies.Models
{
    internal class ErrorModel
    {
        [JsonProperty("error")]
        public string? Error { get; set; }
    }
}
