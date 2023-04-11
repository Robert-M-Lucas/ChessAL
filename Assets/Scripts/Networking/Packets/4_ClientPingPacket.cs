// ReSharper disable All
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ClientPingPacket {
        /// <summary> Unique packet type identifier for ClientPing </summary>
        public const int UID = 4;
        
        /// <summary>
        /// Decodes generic packet data into a ClientPingPacket
        /// </summary>
        public ClientPingPacket(Packet packet){
        }

        /// <summary>
        /// Creates an encoded ClientPingPacket from arguments
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