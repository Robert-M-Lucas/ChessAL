// ReSharper disable All
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ClientDisconnectPacket {
        /// <summary> Unique packet type identifier for ClientDisconnect </summary>
        public const int UID = 7;
        
        /// <summary>
        /// Decodes generic packet data into a ClientDisconnectPacket
        /// </summary>
        public ClientDisconnectPacket(Packet packet){
        }

        /// <summary>
        /// Creates an encoded ClientDisconnectPacket from arguments
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