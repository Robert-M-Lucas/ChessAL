using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerInformOfClientDisconnectPacket {
    public const int UID = 6;
    public int ClientUID;
    public ServerInformOfClientDisconnectPacket(Packet packet){
    }

    public static byte[] Build(int _ClientUID) {
            List<byte[]> contents = new List<byte[]>();
            return PacketBuilder.Build(UID, contents);
    }
}