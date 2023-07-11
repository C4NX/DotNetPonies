using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetPonies
{
    /// <summary>
    /// A class about the current game information, it contains servers list, event message, version, ect...
    /// </summary>
    public class GameStatus
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
        /// Return a boolean that indicate if the game is under maintenance.
        /// </summary>
        public bool MaintenanceMode { get; private set; }

        /// <summary>
        /// Get the 'twitter' value
        /// </summary>
        public bool Twitter { get; private set; }

        /// <summary>
        /// Get the 'updateMode' value
        /// </summary>
        public bool UpdateMode { get; private set; }

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

        /// <summary>
        /// Read a <see cref="GameStatus"/> from an array of <see cref="byte"/>
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="EndOfStreamException">An error has occurred while reading one of the values, or the data provided is not valid.</exception>
        /// <returns>A <see cref="GameStatus"/> object.</returns>
        public static GameStatus FromData(byte[] data)
        {
            GameStatus gameStatus;
            using MemoryStream memoryStream = new MemoryStream(data);
            using BinaryReader reader = new BinaryReader(memoryStream);
            
            var serverList = new List<ServerStatus>();

            //_4n: readUInt16
            //n5n: readString
            //i5n: readByte
            //Q4n: readByte / ReadBoolean
                    
            var statusBit = reader.ReadUInt16() ^ 244;
            var serversCount = reader.ReadByte();
            for (var i = 0; i < serversCount; i++)
            {
                var serverId = reader.ReadString();
                var offlineAndRestrictAndNotification = reader.ReadByte() ^ 113;
                var offline = AndBit(offlineAndRestrictAndNotification, 1);
                var restrict = AndBit(offlineAndRestrictAndNotification, 2);
                var online = reader.ReadUInt16() ^ 34867;
                string? notification = null;
                if (AndBit(offlineAndRestrictAndNotification, 4)) notification = reader.ReadString();
                        
                serverList.Add(new ServerStatus(serverId, online, offline, restrict, notification));
            }

            string? _event = null;
            string? infoMessage = null;
            bool inAppRollout = false;
            string? infoMessageDismissableMessage = null;
            uint? infoMessageDismissableTime = null;
            if (AndBit(statusBit, 8)) _event = reader.ReadString();
            if (AndBit(statusBit, 32)) infoMessage = reader.ReadString();
            if (AndBit(statusBit, 64)) inAppRollout = reader.ReadBoolean();
            if (AndBit(statusBit, 128))
            {
                infoMessageDismissableMessage = reader.ReadString();
                infoMessageDismissableTime = reader.ReadUInt32();
            }

            bool maintenanceMode = AndBit(statusBit, 2);
            bool twitter = AndBit(statusBit, 4);
            bool updateMode = AndBit(statusBit, 1);
            bool visitPt = AndBit(statusBit, 16);
            bool enableBoosty = AndBit(statusBit, 256);
            bool criticalTwitterWarning = AndBit(statusBit, 512);

            gameStatus = new GameStatus
            {
                MaintenanceMode = maintenanceMode,
                Twitter = twitter,
                UpdateMode = updateMode,
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

            return gameStatus;
        }

        private static bool AndBit(int t, int n) => (t & n) == n;
    }
}