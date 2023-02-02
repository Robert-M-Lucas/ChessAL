using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{
    public class SampleTestPacket {
        /// <summary> Unique packet type identifier for SampleTest </summary>
        public const int UID = 100;
        public int ArgOne;
        public double ArgTwo;
        public string ArgThree;
        public string ArgFour;
        
        /// <summary>
        /// Decodes generic packet data into a SampleTestPacket
        /// </summary>
        public SampleTestPacket(Packet packet){
            ArgOne = BitConverter.ToInt32(packet.Contents[0]);
            ArgTwo = BitConverter.ToDouble(packet.Contents[1]);
            ArgThree = Encoding.ASCII.GetString(packet.Contents[2]);
            ArgFour = Encoding.ASCII.GetString(packet.Contents[3]);
        }

        /// <summary>
        /// Creates an encoded SampleTestPacket from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
        public static byte[] Build(int _ArgOne, double _ArgTwo, string _ArgThree, string _ArgFour="defaultVal") {
            List<byte[]> contents = new List<byte[]>
            {
                BitConverter.GetBytes(_ArgOne),
                BitConverter.GetBytes(_ArgTwo),
                Encoding.ASCII.GetBytes(_ArgThree),
                Encoding.ASCII.GetBytes(_ArgFour),
            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}