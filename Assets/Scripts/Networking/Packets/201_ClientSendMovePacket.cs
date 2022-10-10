using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ClientSendMovePacket {
    public const int UID = 201;
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;
    public ClientSendMovePacket(Packet packet){
        fromY = BitConverter.ToInt32(packet.contents[0]);
        toX = BitConverter.ToInt32(packet.contents[1]);
        toY = BitConverter.ToInt32(packet.contents[2]);
    }

    public static byte[] Build(int _fromX, int _fromY, int _toX, int _toY) {
            List<byte[]> contents = new List<byte[]>();
            contents.Add(BitConverter.GetBytes(_fromY));
            contents.Add(BitConverter.GetBytes(_toX));
            contents.Add(BitConverter.GetBytes(_toY));
            return PacketBuilder.Build(UID, contents);
    }
}