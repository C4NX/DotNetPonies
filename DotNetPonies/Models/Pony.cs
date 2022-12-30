using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetPonies.Models
{
    public class Pony
    {
        [JsonProperty("id")]
        public string? Id { get; private set; }

        [JsonProperty("name")]
        public string? Name { get; private set; }

        [JsonProperty("info")]
        public string? Data { get; private set; }

        [JsonProperty("site")]
        public string? Site { get; private set; }

        [JsonProperty("specialTag")]
        public int SpecialTag { get; private set; }

        [JsonProperty("lastUsed")]
        public DateTime LastUsed { get; private set; }

        [JsonProperty("supporterTag")]
        public int SupporterTag { get; private set; }

        public override string ToString()
            => $"{Id ?? "No Id"} {Name ?? "No Name"}";
    }
}
