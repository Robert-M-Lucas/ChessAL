using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ClientPingPacket {
        public const int UID = 4;
        public ClientPingPacket(Packet packet){
        }

       public static byte[] Build() {
           List<byte[]> contents = new List<byte[]>();
           return PacketBuilder.Build(UID, contents);
    }
    }
}