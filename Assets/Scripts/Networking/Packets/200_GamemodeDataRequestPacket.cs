// ReSharper disable All
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class GamemodeDataRequestPacket {
        /// <summary> Unique packet type identifier for GamemodeDataRequest </summary>
        public const int UID = 200;
        
        /// <summary>
        /// Decodes generic packet data into a GamemodeDataRequestPacket
        /// </summary>
        public GamemodeDataRequestPacket(Packet packet){
        }

        /// <summary>
        /// Creates an encoded GamemodeDataRequestPacket from arguments
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