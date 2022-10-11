using System;
using System.Collections.Generic;

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

    public bool TryHandlePacket(Packet packet)
    {
        if (!UIDtoAction.ContainsKey(packet.UID)) return false;

        UIDtoAction[packet.UID](packet);
        return true;
    }

    public void ServerAccept(Packet packet)
    {
        // ServerConnectAcceptPacket acceptPacket = new ServerConnectAcceptPacket(packet);
    }

    public void ServerKick(Packet packet)
    {
        ServerKickPacket kickPacket = new ServerKickPacket(packet);
        client.Disconnect(kickPacket.Reason);
    }

    public void PlayerInformationUpdate(Packet packet)
    {
        ServerOtherClientInfoPacket infoPacket = new ServerOtherClientInfoPacket(packet);
        client.AddOrUpdatePlayer(infoPacket.ClientUID, infoPacket.ClientName, infoPacket.ClientTeam, infoPacket.ClientPlayerInTeam);
        client.OnPlayersChange();
    }

    public void PlayerDisconnect(Packet packet)
    {
        ServerInformOfClientDisconnectPacket disconnectPacket =
            new ServerInformOfClientDisconnectPacket(packet);
        client.TryRemovePlayer(disconnectPacket.ClientUID);
    }

    public void PingResponse(Packet p)
    {
        int ping = client.PingTimer.Elapsed.Milliseconds;
        client.pingResponseAction(ping);
        client.PingTimer.Reset();
        client.pingResponseAction = null;
    }
}