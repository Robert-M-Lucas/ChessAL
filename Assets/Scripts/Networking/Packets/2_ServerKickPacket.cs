using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerKickPacket {
        public const int UID = 2;
        public string Reason;
        public ServerKickPacket(Packet packet){
            Reason = ASCIIEncoding.ASCII.GetString(packet.Contents[0]);
        }

       public static byte[] Build(string _Reason) {
           List<byte[]> contents = new List<byte[]>();
           contents.Add(ASCIIEncoding.ASCII.GetBytes(_Reason));
           return PacketBuilder.Build(UID, contents);
    }
    }
}