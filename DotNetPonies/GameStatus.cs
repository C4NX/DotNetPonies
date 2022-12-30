using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetPonies
{
    public class GameStatus
    {
        public string? Version { get; internal set; }
        public IReadOnlyCollection<ServerStatus> Servers { get; private set; }

        public GameStatus() 
        { 
            Servers = new List<ServerStatus>();
        }

        internal void SetStatus(string versionString, List<ServerStatus> serverStatuses)
        {
            Servers = serverStatuses
                .AsReadOnly();
            Version = versionString;
        }
    }
}
