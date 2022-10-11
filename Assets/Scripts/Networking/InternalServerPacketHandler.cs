using System;
using System.Collections.Generic;

public class InternalServerPacketHandler
{
    private Server server;

    public Dictionary<int, Action<Packet>> UIDtoAction { get; }

    public InternalServerPacketHandler(Server server)
    {
        this.server = server;

        UIDtoAction = new Dictionary<int, Action<Packet>> {
            { 4, PingRespond },
            { 7, RemoveClient }
        };
    }

    public bool TryHandlePacket(Packet packet)
    {
        if (!UIDtoAction.ContainsKey(packet.UID)) return false;

        UIDtoAction[packet.UID](packet);
        return true;
    }

    public void PingRespond(Packet packet)
    {
        server.SendMessage(packet.From, ServerPingPacket.Build());
    }

    public void RemoveClient(Packet packet)
    {
        server.TryRemovePlayer(packet.From, "Disconnected");
    }
}