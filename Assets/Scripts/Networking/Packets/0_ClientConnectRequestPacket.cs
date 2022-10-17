using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ClientConnectRequestPacket {
        public const int UID = 0;
        public string Name;
        public string Version;
        public string Password;
        public ClientConnectRequestPacket(Packet packet){
            Name = ASCIIEncoding.ASCII.GetString(packet.Contents[0]);
            Version = ASCIIEncoding.ASCII.GetString(packet.Contents[1]);
            Password = ASCIIEncoding.ASCII.GetString(packet.Contents[2]);
        }

       public static byte[] Build(string _Name, string _Version, string _Password="") {
           List<byte[]> contents = new List<byte[]>();
           contents.Add(ASCIIEncoding.ASCII.GetBytes(_Name));
           contents.Add(ASCIIEncoding.ASCII.GetBytes(_Version));
           contents.Add(ASCIIEncoding.ASCII.GetBytes(_Password));
           return PacketBuilder.Build(UID, contents);
    }
    }
}