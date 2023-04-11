using System;
using System.Collections.Generic;

namespace Networking.Packets.Unused
{
    public class MoveUpdatePacket {
        public const int UID = 204;
        public int NextPlayerTurn;
        public byte[] TileUpdates;
        public MoveUpdatePacket(Packet packet){
            NextPlayerTurn = BitConverter.ToInt32(packet.Contents[0]);
            TileUpdates = (packet.Contents[1]);
        }

       public static byte[] Build(int nextPlayerTurn, byte[] tileUpdates) {
           var contents = new List<byte[]>
           {
               BitConverter.GetBytes(nextPlayerTurn),
               tileUpdates
           };
           return PacketBuilder.Build(UID, contents);
    }
    }
}