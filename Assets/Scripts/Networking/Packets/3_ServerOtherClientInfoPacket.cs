using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerOtherClientInfoPacket {
        /// <summary> Unique packet type identifier for ServerOtherClientInfo </summary>
        public const int UID = 3;
        public int ClientUID;
        public string ClientName;
        public int ClientTeam;
        public int ClientPlayerInTeam;
        
        /// <summary>
        /// Decodes generic packet data into a ServerOtherClientInfoPacket
        /// </summary>
        public ServerOtherClientInfoPacket(Packet packet){
            ClientUID = BitConverter.ToInt32(packet.Contents[0]);
            ClientName = Encoding.ASCII.GetString(packet.Contents[1]);
            ClientTeam = BitConverter.ToInt32(packet.Contents[2]);
            ClientPlayerInTeam = BitConverter.ToInt32(packet.Contents[3]);
        }

        /// <summary>
        /// Creates an encoded ServerOtherClientInfoPacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build(int _ClientUID, string _ClientName="", int _ClientTeam=-1, int _ClientPlayerInTeam=-1) {
            List<byte[]> contents = new List<byte[]>
            {
                BitConverter.GetBytes(_ClientUID),
                Encoding.ASCII.GetBytes(_ClientName),
                BitConverter.GetBytes(_ClientTeam),
                BitConverter.GetBytes(_ClientPlayerInTeam),
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}