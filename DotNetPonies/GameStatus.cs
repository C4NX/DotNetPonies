using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetPonies
{
    public class GameStatus
    {
        public string? Version { get; internal set; }
        public IReadOnlyCollection<ServerStatus> Servers { get; private set; }

        public string? Event { get;internal set; }
        public string? InfoMessage { get; internal set; }
        public string? InfoMessageDismissableMessage { get; internal set; }
        public uint? InfoMessageDismissableTime { get; internal set; }

        public bool MaintenanceMode { get; internal set; }
        public bool Twitter { get; internal set; }
        public bool UpdateMode { get; internal set; }
        public bool VisitPt { get; internal set; }
        public bool EnableBoosty { get; internal set; }

        public GameStatus() 
        { 
            Servers = new List<ServerStatus>();
        }

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

                    gameStatus.MaintenanceMode = IsBit(gameBits, 2);
                    gameStatus.Twitter = IsBit(gameBits, 4);
                    gameStatus.UpdateMode = IsBit(gameBits, 1);
                    gameStatus.VisitPt = IsBit(gameBits, 16);
                    gameStatus.EnableBoosty = IsBit(gameBits, 256);

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
                        serverList.Add(new ServerStatus(id, onlineCount, IsBit(serverBits, 1), IsBit(serverBits, 2), percistantNotif));
                    }

                    gameStatus.Servers = serverList.AsReadOnly();
                    gameStatus.Version = versionString;
                }
            }
            return gameStatus;
        }

        private static bool IsBit(int t, int n)
            => (t & n) == n;
    }
}
