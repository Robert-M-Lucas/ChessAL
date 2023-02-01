using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class MoveUpdatePacket {
        public const int UID = 205;
        public int NextPlayer;
        public int FromX;
        public int FromY;
        public int ToX;
        public int ToY;
        public MoveUpdatePacket(Packet packet){
            NextPlayer = BitConverter.ToInt32(packet.Contents[0]);
            FromX = BitConverter.ToInt32(packet.Contents[1]);
            FromY = BitConverter.ToInt32(packet.Contents[2]);
            ToX = BitConverter.ToInt32(packet.Contents[3]);
            ToY = BitConverter.ToInt32(packet.Contents[4]);
        }

       public static byte[] Build(int _NextPlayer, int _FromX, int _FromY, int _ToX, int _ToY) {
           List<byte[]> contents = new List<byte[]>();
           contents.Add(BitConverter.GetBytes(_NextPlayer));
           contents.Add(BitConverter.GetBytes(_FromX));
           contents.Add(BitConverter.GetBytes(_FromY));
           contents.Add(BitConverter.GetBytes(_ToX));
           contents.Add(BitConverter.GetBytes(_ToY));
           return PacketBuilder.Build(UID, contents);
        }
    }
}