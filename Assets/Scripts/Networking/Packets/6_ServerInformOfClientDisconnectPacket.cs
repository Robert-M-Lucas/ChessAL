using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerInformOfClientDisconnectPacket {
        public const int UID = 6;
        public int ClientUID;
        public ServerInformOfClientDisconnectPacket(Packet packet){
            ClientUID = BitConverter.ToInt32(packet.Contents[0]);
        }

       public static byte[] Build(int _ClientUID) {
           List<byte[]> contents = new List<byte[]>();
           contents.Add(BitConverter.GetBytes(_ClientUID));
           return PacketBuilder.Build(UID, contents);
    }
    }
}