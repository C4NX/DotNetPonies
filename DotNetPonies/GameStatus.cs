using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetPonies
{
    /// <summary>
    /// A class about the current game informations, it contains servers list, event message, version, ect...
    /// </summary>
    public class GameStatus
    {
        /// <summary>
        /// Get the game version
        /// </summary>
        public string? Version { get; private set; }
        /// <summary>
        /// Get a readonly collection of <see cref="ServerStatus"/>
        /// </summary>
        public IReadOnlyCollection<ServerStatus> Servers { get; private set; }

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

        internal GameStatus() 
        { 
            Servers = new List<ServerStatus>();
        }

        /// <summary>
        /// Read a <see cref="GameStatus"/> from an array of <see cref="byte"/>
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="EndOfStreamException">An error has occurred while reading one of the values, or the data provided is not valid.</exception>
        /// <returns>A <see cref="GameStatus"/> object.</returns>
        public static GameStatus FromData(byte[] data)
        {
            GameStatus gameStatus = new GameStatus();
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    List<ServerStatus> serverList = new List<ServerStatus>();
                    string versionString = reader.ReadString();
                    var gameBits = 179 ^ reader.ReadUInt16();

                    // event
                    if((8 & gameBits) > 0)
                        gameStatus.Event = reader.ReadString();
                    // infoMessage
                    if ((32 & gameBits) > 0)
                        gameStatus.InfoMessage = reader.ReadString();

                    // skip inAppRollout
                    if ((64 & gameBits) > 0)
                        memoryStream.Position++;

                    // infoMessageDismissable
                    if ((128 & gameBits) > 0)
                    {
                        gameStatus.InfoMessageDismissableMessage = reader.ReadString();
                        gameStatus.InfoMessageDismissableTime = reader.ReadUInt32();
                    }

                    gameStatus.MaintenanceMode = AndBit(gameBits, 2);
                    gameStatus.Twitter = AndBit(gameBits, 4);
                    gameStatus.UpdateMode = AndBit(gameBits, 1);
                    gameStatus.VisitPt = AndBit(gameBits, 16);
                    gameStatus.EnableBoosty = AndBit(gameBits, 256);

                    byte serverCount = reader.ReadByte();
                    for (int i = 0; i < serverCount; i++)
                    {
                        string id = reader.ReadString();
                        string? percistantNotif = null;

                        // read the offline, restrict byte
                        int serverBits = 54 ^ reader.ReadByte();

                        // untested, 4 & serverBits && (o = FQn(t))
                        if ((serverBits & 4) > 0) // hasPercistantNotificationText
                            percistantNotif = reader.ReadString();

                        int onlineCount = 52340 ^ reader.ReadUInt16();
                        serverList.Add(new ServerStatus(id, onlineCount, AndBit(serverBits, 1), AndBit(serverBits, 2), percistantNotif));
                    }

                    gameStatus.Servers = serverList.AsReadOnly();
                    gameStatus.Version = versionString;
                }
            }
            return gameStatus;
        }

        private static bool AndBit(int t, int n)
            => (t & n) == n;
    }
}
