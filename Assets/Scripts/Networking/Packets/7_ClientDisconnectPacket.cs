using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ClientDisconnectPacket {
        public const int UID = 7;
        public ClientDisconnectPacket(Packet packet){
        }

       public static byte[] Build() {
           List<byte[]> contents = new List<byte[]>();
           return PacketBuilder.Build(UID, contents);
        }
    }
}