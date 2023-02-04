using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerConnectAcceptPacket {
        /// <summary> Unique packet type identifier for ServerConnectAccept </summary>
        public const int UID = 1;
        public int PlayerID;
        
        /// <summary>
        /// Decodes generic packet data into a ServerConnectAcceptPacket
        /// </summary>
        public ServerConnectAcceptPacket(Packet packet){
            PlayerID = BitConverter.ToInt32(packet.Contents[0]);
        }

        /// <summary>
        /// Creates an encoded ServerConnectAcceptPacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build(int _PlayerID) {
            List<byte[]> contents = new List<byte[]>
            {
                BitConverter.GetBytes(_PlayerID),
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}