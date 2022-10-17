using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class GamemodeDataRequestPacket {
        public const int UID = 200;
        public GamemodeDataRequestPacket(Packet packet){
        }

       public static byte[] Build() {
           List<byte[]> contents = new List<byte[]>();
           return PacketBuilder.Build(UID, contents);
    }
    }
}