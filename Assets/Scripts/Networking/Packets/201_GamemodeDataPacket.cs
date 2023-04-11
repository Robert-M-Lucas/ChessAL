// ReSharper disable All
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class GamemodeDataPacket {
        /// <summary> Unique packet type identifier for GamemodeData </summary>
        public const int UID = 201;
        public int Gamemode;
        public byte[] SaveData;
        
        /// <summary>
        /// Decodes generic packet data into a GamemodeDataPacket
        /// </summary>
        public GamemodeDataPacket(Packet packet){
            Gamemode = BitConverter.ToInt32(packet.Contents[0]);
            SaveData = (packet.Contents[1]);
        }

        /// <summary>
        /// Creates an encoded GamemodeDataPacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build(int _Gamemode, byte[] _SaveData) {
            List<byte[]> contents = new List<byte[]>
            {
                BitConverter.GetBytes(_Gamemode),
                _SaveData,
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}