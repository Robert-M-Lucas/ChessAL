using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class StartGamePacket {
    public const int UID = 202;
    public StartGamePacket(Packet packet){
    }

    public static byte[] Build() {
            List<byte[]> contents = new List<byte[]>();
            return PacketBuilder.Build(UID, contents);
    }
}