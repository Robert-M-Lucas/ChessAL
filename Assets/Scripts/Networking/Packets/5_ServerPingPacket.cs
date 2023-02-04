using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerPingPacket {
        /// <summary> Unique packet type identifier for ServerPing </summary>
        public const int UID = 5;
        
        /// <summary>
        /// Decodes generic packet data into a ServerPingPacket
        /// </summary>
        public ServerPingPacket(Packet packet){
        }

        /// <summary>
        /// Creates an encoded ServerPingPacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build() {
            List<byte[]> contents = new List<byte[]>
            {
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}