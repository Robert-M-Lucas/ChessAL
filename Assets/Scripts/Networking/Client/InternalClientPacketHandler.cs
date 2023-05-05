using System;
using System.Collections.Generic;
using Networking.Packets;
using Networking.Packets.Generated;

namespace Networking.Client
{
    /// <summary>
    /// Controls actions when a packet is recieved by the client
    /// </summary>
    public class InternalClientPacketHandler
    {
        private Client client;

        public Dictionary<int, Action<Packet>> UIDtoAction { get; }

        public InternalClientPacketHandler(Client client)
        {
            this.client = client;

            UIDtoAction = new Dictionary<int, Action<Packet>>
            {
                { 1,  ServerAccept },
                { 2, ServerKick },
                { 3, PlayerInformationUpdate },
                { 6, PlayerDisconnect },
                { 5, PingResponse },
                { 201, GamemodeDataRecieve},
                { 202, OnGameStart },
                { 205, OnMoveUpdate },
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

        // Server accepted connection
        public void ServerAccept(Packet packet)
        {
            var accept_packet = new ServerConnectAcceptPacket(packet);
            client.PlayerID = accept_packet.PlayerID;
        }

        // Server kicked player
        public void ServerKick(Packet packet)
        {
            var kick_packet = new ServerKickPacket(packet);
            client.Disconnect(kick_packet.Reason);
        }

        // Server has new information about players
        public void PlayerInformationUpdate(Packet packet)
        {
            var info_packet = new ServerOtherClientInfoPacket(packet);
            client.AddOrUpdatePlayer(info_packet.ClientUID, info_packet.ClientName, info_packet.ClientTeam, info_packet.ClientPlayerInTeam);
            client.NetworkManager.OnPlayersChange();
        }

        // A player disconnected
        public void PlayerDisconnect(Packet packet)
        {
            var disconnect_packet =
                new ServerInformOfClientDisconnectPacket(packet);
            client.TryRemovePlayer(disconnect_packet.ClientUID);
        }

        // The server has responded to a ping
        public void PingResponse(Packet p)
        {
            var ping = client.PingTimer.Elapsed.Milliseconds;
            client.PingResponseAction!(ping);
            client.PingTimer.Reset();
            client.PingResponseAction = null;
        }

        // Processes gamemode data recieved from the server
        public void GamemodeDataRecieve(Packet p)
        {
            var gamemode_data_packet = new GamemodeDataPacket(p);
            client.NetworkManager.OnGamemodeRecieve(gamemode_data_packet.Gamemode, gamemode_data_packet.SaveData);
        }

        // Starts game when game start packet is recieved
        public void OnGameStart(Packet p)
        {
            client.NetworkManager.OnGameStart();
        }

        // Updates game when a move update is recieved
        public void OnMoveUpdate(Packet p)
        {
            var move_update_packet = new MoveUpdatePacket(p);

            client.NetworkManager.OnForeignMove(move_update_packet.NextPlayer, new V2(move_update_packet.FromX, move_update_packet.FromY), new V2(move_update_packet.ToX, move_update_packet.ToY));
        }
    }
}