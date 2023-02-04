using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class MoveUpdatePacket {
        /// <summary> Unique packet type identifier for MoveUpdate </summary>
        public const int UID = 205;
        public int NextPlayer;
        public int FromX;
        public int FromY;
        public int ToX;
        public int ToY;
        
        /// <summary>
        /// Decodes generic packet data into a MoveUpdatePacket
        /// </summary>
        public MoveUpdatePacket(Packet packet){
            NextPlayer = BitConverter.ToInt32(packet.Contents[0]);
            FromX = BitConverter.ToInt32(packet.Contents[1]);
            FromY = BitConverter.ToInt32(packet.Contents[2]);
            ToX = BitConverter.ToInt32(packet.Contents[3]);
            ToY = BitConverter.ToInt32(packet.Contents[4]);
        }

        /// <summary>
        /// Creates an encoded MoveUpdatePacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build(int _NextPlayer, int _FromX, int _FromY, int _ToX, int _ToY) {
            List<byte[]> contents = new List<byte[]>
            {
                BitConverter.GetBytes(_NextPlayer),
                BitConverter.GetBytes(_FromX),
                BitConverter.GetBytes(_FromY),
                BitConverter.GetBytes(_ToX),
                BitConverter.GetBytes(_ToY),
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}