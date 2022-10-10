using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerOtherClientInfoPacket {
    public const int UID = 3;
    public int ClientUID;
    public string ClientName;
    public ServerOtherClientInfoPacket(Packet packet){
        ClientName = ASCIIEncoding.ASCII.GetString(packet.contents[0]);
    }

    public static byte[] Build(int _ClientUID, string _ClientName="") {
            List<byte[]> contents = new List<byte[]>();
            contents.Add(ASCIIEncoding.ASCII.GetBytes(_ClientName));
            return PacketBuilder.Build(UID, contents);
    }
}