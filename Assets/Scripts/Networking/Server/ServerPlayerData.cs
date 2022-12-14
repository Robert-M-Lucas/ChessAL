using System.Net.Sockets;
using System.Text;

namespace Networking.Server
{
    /// <summary>
    /// Data server holds about player
    /// </summary>
    public class ServerPlayerData
    {
        public int PlayerID;
        public string Name;

        public int Team;

        /// <summary>
        /// E.g. Player *1* in team x
        /// </summary>
        public int PlayerInTeam;

        public ServerPlayerData(Socket handler, int playerID, string name, int team, int playerInTeam)
        {
            Handler = handler;
            PlayerID = playerID;
            Name = name;
            Team = team;
            PlayerInTeam = playerInTeam;
        }

        /// <summary>
        /// Returns a unique string that represents the player
        /// </summary>
        /// <returns></returns>
        public string GetUniqueString() => $"[{PlayerID} {Name}";
        
        // Networking
        public Socket Handler;

        // Buffers
        public byte[] Buffer = new byte[1024];
        public byte[] LongBuffer = new byte[1024];
        public int CurrentPacketLength = -1;
        public int LongBufferSize = 0;

        /// <summary>
        /// Resets buffers
        /// </summary>
        public void Reset() => Buffer = new byte[1024];

        /// <summary>
        /// Shuts down the connection between the client and the server
        /// </summary>
        public void ShutdownSocket()
        {
            try { Handler.Disconnect(false); } catch (SocketException) { }
            try { Handler.Shutdown(SocketShutdown.Both); } catch (SocketException) { }
            try { Handler.Close(0); } catch (SocketException) { }
            try { Handler.Dispose(); } catch (SocketException) { }
        }
    }
}