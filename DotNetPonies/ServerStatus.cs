using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetPonies
{
    /// <summary>
    /// Represents the status of a pony town server
    /// </summary>
    public class ServerStatus
    {
        /// <summary>
        /// Get the server Id, like main, safe...
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Get how many players are on the server
        /// </summary>
        public int OnlineCount { get; }

        /// <summary>
        /// Get if the server is offline
        /// </summary>
        public bool Offline { get; }
        /// <summary>
        /// Get if the server is restrict
        /// </summary>
        public bool Restrict { get; }

        /// <summary>
        /// Get the percistant notification message
        /// </summary>
        public string? PercistantNotificationText { get; }

        internal ServerStatus(string id, int online, bool offline, bool restrict, string? percistantNotificationText)
        {
            Id = id;
            OnlineCount = online;
            Offline = offline;
            Restrict = restrict;
            PercistantNotificationText = percistantNotificationText;
        }

        public override string ToString()
            => $"[{Id ?? "?"}] {OnlineCount.ToString() ?? "?"} players";
    }
}
