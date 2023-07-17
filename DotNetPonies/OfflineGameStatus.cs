using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetPonies
{
    /// <summary>
    /// A class about the current game information, it contains servers list, event message, version, ect...
    /// </summary>
    public class OfflineGameStatus
    {
        /// <summary>
        /// Get a readonly collection of <see cref="ServerStatus"/>
        /// </summary>
        public IReadOnlyCollection<ServerStatus> Servers { get; private set; } = new List<ServerStatus>();

        /// <summary>
        /// Get the current event name or null if no event is currently taking place.
        /// </summary>
        public string? Event { get; private set; }

        /// <summary>
        /// Get the 'infoMessage' string value.
        /// </summary>
        public string? InfoMessage { get; private set; }

        /// <summary>
        /// Get the 'infoMessageDismissable.message' value
        /// </summary>
        public string? InfoMessageDismissableMessage { get; private set; }

        /// <summary>
        /// Get the 'infoMessageDismissable.time' value
        /// </summary>
        public uint? InfoMessageDismissableTime { get; private set; }

        /// <summary>
        /// Get the 'twitter' value
        /// </summary>
        public bool Twitter { get; private set; }

        /// <summary>
        /// Get the 'visitPt' value
        /// </summary>
        public bool VisitPt { get; private set; }

        /// <summary>
        /// Get the 'enableBoosty' value
        /// </summary>
        public bool EnableBoosty { get; private set; }

        /// <summary>
        /// Get the 'inAppRollout' value
        /// </summary>
        public bool InAppRollout { get; private set; }

        /// <summary>
        /// Get the 'criticalTwitterWarning' value
        /// </summary>
        public bool CriticalTwitterWarning { get; private set; }

        public string? RestartNotificationJson { get; private set; }

        /// <summary>
        /// Read a <see cref="OfflineGameStatus"/> from an array of <see cref="byte"/>
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="EndOfStreamException">An error has occurred while reading one of the values, or the data provided is not valid.</exception>
        /// <returns>A <see cref="OfflineGameStatus"/> object.</returns>
        public static OfflineGameStatus FromData(byte[] data, PonyTownScope scope)
        {
            OfflineGameStatus offlineGameStatus;
            using MemoryStream memoryStream = new MemoryStream(data);
            using BinaryReader reader = new BinaryReader(memoryStream);
            var serverList = new List<ServerStatus>();
            var statusBit = reader.ReadUInt16() ^ scope.MagicStatusBit;
            var serversCount = reader.ReadByte();
            for (var i = 0; i < serversCount; i++)
            {
                var serverId = reader.ReadString();
                var offlineAndRestrictAndNotification = reader.ReadByte() ^ scope.MagicOfflineAndRestrictAndNotificationBit;
                var offline = AndBit(offlineAndRestrictAndNotification, 1);
                var restrict = AndBit(offlineAndRestrictAndNotification, 2);
                var online = reader.ReadUInt16() ^ scope.MagicOnlineBit;
                string? notification = null;
                if (AndBit(offlineAndRestrictAndNotification, 4)) notification = reader.ReadString();
                serverList.Add(new ServerStatus(serverId, online, offline, restrict, notification));
            }

            // read meta data
            
            string? _event = null;
            string? infoMessage = null;
            bool inAppRollout = false;
            string? infoMessageDismissableMessage = null;
            uint? infoMessageDismissableTime = null;
            string? restartNotificationJson = null;
            if (AndBit(statusBit, 1)) restartNotificationJson = reader.ReadString();
            if (AndBit(statusBit, 4)) _event = reader.ReadString();
            if (AndBit(statusBit, 16)) infoMessage = reader.ReadString();
            if (AndBit(statusBit, 32)) inAppRollout = reader.ReadBoolean();
            if (AndBit(statusBit, 64))
            {
                infoMessageDismissableMessage = reader.ReadString();
                infoMessageDismissableTime = reader.ReadUInt32();
            }

            // read boolean values
            
            bool twitter = AndBit(statusBit, 2);
            bool visitPt = AndBit(statusBit, 8);
            bool enableBoosty = AndBit(statusBit, 128);
            bool criticalTwitterWarning = AndBit(statusBit, 256);
            offlineGameStatus = new OfflineGameStatus
            {
                RestartNotificationJson = restartNotificationJson,
                Twitter = twitter,
                VisitPt = visitPt,
                EnableBoosty = enableBoosty,
                CriticalTwitterWarning = criticalTwitterWarning,
                InfoMessage = infoMessage,
                InfoMessageDismissableMessage = infoMessageDismissableMessage,
                InfoMessageDismissableTime = infoMessageDismissableTime,
                Event = _event,
                Servers = serverList,
                InAppRollout = inAppRollout
            };
            return offlineGameStatus;
        }

        private static bool AndBit(int t, int n) => (t & n) == n;
    }
}