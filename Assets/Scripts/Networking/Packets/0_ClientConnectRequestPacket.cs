using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ClientConnectRequestPacket {
    public const int UID = 0;
    public string Name;
    public string Version;
    public string Password;
    public ClientConnectRequestPacket(Packet packet){
        Version = ASCIIEncoding.ASCII.GetString(packet.Contents[0]);
        Password = ASCIIEncoding.ASCII.GetString(packet.Contents[1]);
    }

    public static byte[] Build(string _Name, string _Version, string _Password="") {
            List<byte[]> contents = new List<byte[]>();
            contents.Add(ASCIIEncoding.ASCII.GetBytes(_Version));
            contents.Add(ASCIIEncoding.ASCII.GetBytes(_Password));
            return PacketBuilder.Build(UID, contents);
    }
}