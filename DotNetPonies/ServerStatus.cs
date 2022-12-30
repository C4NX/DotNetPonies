using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetPonies
{
    public class ServerStatus
    {
        public string? Id { get; set; }
        public int? Count { get; set; }

        public override string ToString()
            => $"[{Id ?? "?"}] {Count?.ToString() ?? "?"} players";
    }
}
