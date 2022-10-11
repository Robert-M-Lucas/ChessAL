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
        fromX = BitConverter.ToInt32(packet.Contents[0]);
        fromY = BitConverter.ToInt32(packet.Contents[1]);
        toX = BitConverter.ToInt32(packet.Contents[2]);
        toY = BitConverter.ToInt32(packet.Contents[3]);
    }

    public static byte[] Build(int _fromX, int _fromY, int _toX, int _toY) {
            List<byte[]> contents = new List<byte[]>();
            contents.Add(BitConverter.GetBytes(_fromX));
            contents.Add(BitConverter.GetBytes(_fromY));
            contents.Add(BitConverter.GetBytes(_toX));
            contents.Add(BitConverter.GetBytes(_toY));
            return PacketBuilder.Build(UID, contents);
    }
}