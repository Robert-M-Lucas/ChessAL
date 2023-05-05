// ReSharper disable All
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ClientConnectRequestPacket {
        /// <summary> Unique packet type identifier for ClientConnectRequest </summary>
        public const int UID = 0;
        public string Name;
        public string Version;
        public string Password;
        
        /// <summary>
        /// Decodes generic packet data into a ClientConnectRequestPacket
        /// </summary>
        public ClientConnectRequestPacket(Packet packet){
            Name = Encoding.ASCII.GetString(packet.Contents[0]);
            Version = Encoding.ASCII.GetString(packet.Contents[1]);
            Password = Encoding.ASCII.GetString(packet.Contents[2]);
        }

        /// <summary>
        /// Creates an encoded ClientConnectRequestPacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build(string _Name, string _Version, string _Password="") {
            List<byte[]> contents = new List<byte[]>
            {
                Encoding.ASCII.GetBytes(_Name),
                Encoding.ASCII.GetBytes(_Version),
                Encoding.ASCII.GetBytes(_Password),
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}