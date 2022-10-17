using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class MoveUpdatePacket {
        public const int UID = 204;
        public int NextPlayerTurn;
        public byte[] TileUpdates;
        public MoveUpdatePacket(Packet packet){
            NextPlayerTurn = BitConverter.ToInt32(packet.Contents[0]);
            TileUpdates = (packet.Contents[1]);
        }

       public static byte[] Build(int _NextPlayerTurn, byte[] _TileUpdates) {
           List<byte[]> contents = new List<byte[]>();
           contents.Add(BitConverter.GetBytes(_NextPlayerTurn));
           contents.Add(_TileUpdates);
           return PacketBuilder.Build(UID, contents);
    }
    }
}