using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerOtherClientInfoPacket {
    public const int UID = 3;
    public int ClientUID;
    public string ClientName;
    public ServerOtherClientInfoPacket(Packet packet){
        ClientUID = BitConverter.ToInt32(packet.Contents[0]);
        ClientName = ASCIIEncoding.ASCII.GetString(packet.Contents[1]);
    }

    public static byte[] Build(int _ClientUID, string _ClientName="") {
            List<byte[]> contents = new List<byte[]>();
            contents.Add(BitConverter.GetBytes(_ClientUID));
            contents.Add(ASCIIEncoding.ASCII.GetBytes(_ClientName));
            return PacketBuilder.Build(UID, contents);
    }
}