using System;
using System.Collections.Generic;

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
            { 1, (Packet p) => ServerAccept(p) },
            { 2, (Packet p) => ServerKick(p) },
            { 3, (Packet p) => PlayerInformationUpdate(p) },
            { 6, (Packet p) => PlayerDisconnect(p) },
            { 5, (Packet p) => PingResponse(p) },
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
        // ServerConnectAcceptPacket acceptPacket = new ServerConnectAcceptPacket(packet);
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
        client.OnPlayersChange();
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
}