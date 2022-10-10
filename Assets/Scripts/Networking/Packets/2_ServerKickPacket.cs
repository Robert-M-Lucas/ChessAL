using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerKickPacket {
    public const int UID = 2;
    public string Reason;
    public ServerKickPacket(Packet packet){
    }

    public static byte[] Build(string _Reason) {
            List<byte[]> contents = new List<byte[]>();
            return PacketBuilder.Build(UID, contents);
    }
}