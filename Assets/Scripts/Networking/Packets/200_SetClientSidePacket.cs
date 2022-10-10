using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SetClientSidePacket {
    public const int UID = 200;
    public int side;
    public SetClientSidePacket(Packet packet){
    }

    public static byte[] Build(int _side) {
            List<byte[]> contents = new List<byte[]>();
            return PacketBuilder.Build(UID, contents);
    }
}