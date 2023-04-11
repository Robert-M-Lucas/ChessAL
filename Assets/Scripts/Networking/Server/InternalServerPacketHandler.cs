using System;
using System.Collections.Generic;
using Networking.Packets;
using Networking.Packets.Generated;


namespace Networking.Server
{
    /// <summary>
    /// Controls actions when a packet is recieved by the server
    /// </summary>
    public class InternalServerPacketHandler
    {
        private Server server;

        public Dictionary<int, Action<Packet>> UIDtoAction { get; }

        public InternalServerPacketHandler(Server server)
        {
            this.server = server;

            UIDtoAction = new Dictionary<int, Action<Packet>> {
            { 4, PingRespond },
            { 7, RemoveClient },
            { 200, SendGamemodeData },
            {205, OnMoveUpdate }
        };
        }

        /// <summary>
        /// Attempts to process a packet
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>Whether the packet was successfully processed</returns>
        public bool TryHandlePacket(Packet packet)
        {
            if (!UIDtoAction.ContainsKey(packet.UID)) return false;

            UIDtoAction[packet.UID](packet);
            return true;
        }

        // Respond to a client's ping
        public void PingRespond(Packet packet)
        {
            server.SendMessage(packet.From, ServerPingPacket.Build());
        }

        // Player disconnected
        public void RemoveClient(Packet packet)
        {
            server.TryRemovePlayer(packet.From, "Disconnected");
        }

        // Send data about current gamemode
        public void SendGamemodeData(Packet packet)
        {
            server.SendMessage(packet.From, GamemodeDataPacket.Build(server.GameData.GameModeID, server.GameData.SaveData));
        }
        
        // Broadcasts a move sent in by a player to all players
        public void OnMoveUpdate(Packet packet)
        {
            var p = new MoveUpdatePacket(packet);

            server.SendToAll(MoveUpdatePacket.Build(p.NextPlayer, p.FromX, p.FromY, p.ToX, p.ToY));
        }
    }
}