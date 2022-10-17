using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class GamemodeDataPacket {
        public const int UID = 201;
        public int Gamemode;
        public byte[] SaveData;
        public GamemodeDataPacket(Packet packet){
            Gamemode = BitConverter.ToInt32(packet.Contents[0]);
            SaveData = (packet.Contents[1]);
        }

       public static byte[] Build(int _Gamemode, byte[] _SaveData) {
           List<byte[]> contents = new List<byte[]>();
           contents.Add(BitConverter.GetBytes(_Gamemode));
           contents.Add(_SaveData);
           return PacketBuilder.Build(UID, contents);
    }
    }
}