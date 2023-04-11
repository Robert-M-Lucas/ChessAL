// ReSharper disable All
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerInformOfClientDisconnectPacket {
        /// <summary> Unique packet type identifier for ServerInformOfClientDisconnect </summary>
        public const int UID = 6;
        public int ClientUID;
        
        /// <summary>
        /// Decodes generic packet data into a ServerInformOfClientDisconnectPacket
        /// </summary>
        public ServerInformOfClientDisconnectPacket(Packet packet){
            ClientUID = BitConverter.ToInt32(packet.Contents[0]);
        }

        /// <summary>
        /// Creates an encoded ServerInformOfClientDisconnectPacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build(int _ClientUID) {
            List<byte[]> contents = new List<byte[]>
            {
                BitConverter.GetBytes(_ClientUID),
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}