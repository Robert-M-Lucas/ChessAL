// ReSharper disable All
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerKickPacket {
        /// <summary> Unique packet type identifier for ServerKick </summary>
        public const int UID = 2;
        public string Reason;
        
        /// <summary>
        /// Decodes generic packet data into a ServerKickPacket
        /// </summary>
        public ServerKickPacket(Packet packet){
            Reason = Encoding.ASCII.GetString(packet.Contents[0]);
        }

        /// <summary>
        /// Creates an encoded ServerKickPacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build(string _Reason) {
            List<byte[]> contents = new List<byte[]>
            {
                Encoding.ASCII.GetBytes(_Reason),
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}