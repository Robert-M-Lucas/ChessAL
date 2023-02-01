using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class ServerOtherClientInfoPacket {
        public const int UID = 3;
        public int ClientUID;
        public string ClientName;
        public int ClientTeam;
        public int ClientPlayerInTeam;
        public ServerOtherClientInfoPacket(Packet packet){
            ClientUID = BitConverter.ToInt32(packet.Contents[0]);
            ClientName = ASCIIEncoding.ASCII.GetString(packet.Contents[1]);
            ClientTeam = BitConverter.ToInt32(packet.Contents[2]);
            ClientPlayerInTeam = BitConverter.ToInt32(packet.Contents[3]);
        }

       public static byte[] Build(int _ClientUID, string _ClientName="", int _ClientTeam=-1, int _ClientPlayerInTeam=-1) {
           List<byte[]> contents = new List<byte[]>();
           contents.Add(BitConverter.GetBytes(_ClientUID));
           contents.Add(ASCIIEncoding.ASCII.GetBytes(_ClientName));
           contents.Add(BitConverter.GetBytes(_ClientTeam));
           contents.Add(BitConverter.GetBytes(_ClientPlayerInTeam));
           return PacketBuilder.Build(UID, contents);
        }
    }
}