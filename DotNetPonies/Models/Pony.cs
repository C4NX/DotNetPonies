using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetPonies.Models
{
    /// <summary>
    /// A class that represents an instance of a pony from pony town.
    /// </summary>
    public class Pony
    {
        /// <summary>
        /// Get the <see cref="Pony"/> identifier.
        /// </summary>
        [JsonProperty("id")]
        public string? Id { get; private set; }


        /// <summary>
        /// Get the <see cref="Pony"/> name.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; private set; }

        /// <summary>
        /// Get the <see cref="Pony"/> data, data will be in base 64.
        /// </summary>
        [JsonProperty("info")]
        public string? Data { get; private set; }

        /// <summary>
        /// Get the Site value of this <see cref="Pony"/> instance.
        /// </summary>
        [JsonProperty("site")]
        public string? Site { get; private set; }

        /// <summary>
        /// Get the special tag value of this <see cref="Pony"/> instance.
        /// </summary>
        [JsonProperty("specialTag")]
        public int SpecialTag { get; private set; }

        /// <summary>
        /// Get the <see cref="DateTime"/> who are representing the last time the <see cref="Pony"/> was used by the player.
        /// </summary>
        [JsonProperty("lastUsed")]
        public DateTime LastUsed { get; private set; }

        /// <summary>
        /// Get the supporter tag value of this <see cref="Pony"/> instance.
        /// </summary>
        [JsonProperty("supporterTag")]
        public int SupporterTag { get; private set; }

        
        public override string ToString()
            => $"{Id ?? "No Id"} {Name ?? "No Name"}";
    }
}
