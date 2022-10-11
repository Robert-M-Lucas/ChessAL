using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerConnectAcceptPacket {
    public const int UID = 1;
    public int PlayerID;
    public ServerConnectAcceptPacket(Packet packet){
    }

    public static byte[] Build(int _PlayerID) {
            List<byte[]> contents = new List<byte[]>();
            return PacketBuilder.Build(UID, contents);
    }
}