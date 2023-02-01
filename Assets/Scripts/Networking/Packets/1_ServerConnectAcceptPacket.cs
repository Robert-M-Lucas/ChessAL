using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerConnectAcceptPacket {
        public const int UID = 1;
        public int PlayerID;
        public ServerConnectAcceptPacket(Packet packet){
            PlayerID = BitConverter.ToInt32(packet.Contents[0]);
        }

       public static byte[] Build(int _PlayerID) {
           List<byte[]> contents = new List<byte[]>();
           contents.Add(BitConverter.GetBytes(_PlayerID));
           return PacketBuilder.Build(UID, contents);
        }
    }
}