using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
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
            { 205, OnMoveUpdate }
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
            ServerConnectAcceptPacket acceptPacket = new ServerConnectAcceptPacket(packet);
            client.PlayerID = acceptPacket.PlayerID;
        }

        // Server kicked player
        public void ServerKick(Packet packet)
        {
            ServerKickPacket kickPacket = new ServerKickPacket(packet);
            client.Disconnect(kickPacket.Reason);
        }

        // Server has new information about players
        public void PlayerInformationUpdate(Packet packet)
        {
            ServerOtherClientInfoPacket infoPacket = new ServerOtherClientInfoPacket(packet);
            client.AddOrUpdatePlayer(infoPacket.ClientUID, infoPacket.ClientName, infoPacket.ClientTeam, infoPacket.ClientPlayerInTeam);
            client.networkManager.OnPlayersChange();
        }

        // A player disconnected
        public void PlayerDisconnect(Packet packet)
        {
            ServerInformOfClientDisconnectPacket disconnectPacket =
                new ServerInformOfClientDisconnectPacket(packet);
            client.TryRemovePlayer(disconnectPacket.ClientUID);
        }

        // The server has responded to a ping
        public void PingResponse(Packet p)
        {
            int ping = client.PingTimer.Elapsed.Milliseconds;
            client.pingResponseAction(ping);
            client.PingTimer.Reset();
            client.pingResponseAction = null;
        }

        // Processes gamemode data recieved from the server
        public void GamemodeDataRecieve(Packet p)
        {
            GamemodeDataPacket gamemodeDataPacket = new GamemodeDataPacket(p);
            client.networkManager.OnGamemodeRecieve(gamemodeDataPacket.Gamemode, gamemodeDataPacket.SaveData);
        }

        // Starts game when game start packet is recieved
        public void OnGameStart(Packet p)
        {
            client.networkManager.OnGameStart();
        }

        // Updates game when a move update is recieved
        public void OnMoveUpdate(Packet p)
        {
            MoveUpdatePacket moveUpdatePacket = new MoveUpdatePacket(p);

            client.networkManager.OnForeignMove(moveUpdatePacket.NextPlayer, new V2(moveUpdatePacket.FromX, moveUpdatePacket.FromY), new V2(moveUpdatePacket.ToX, moveUpdatePacket.ToY));
        }
    }
}