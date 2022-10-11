using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SampleTestTwoPacket {
    public const int UID = 101;
    public int ArgOne;
    public double ArgTwo;
    public string ArgThree;
    public string ArgFour;
    public SampleTestTwoPacket(Packet packet){
        ArgTwo = BitConverter.ToDouble(packet.Contents[0]);
        ArgThree = ASCIIEncoding.ASCII.GetString(packet.Contents[1]);
        ArgFour = ASCIIEncoding.ASCII.GetString(packet.Contents[2]);
    }

    public static byte[] Build(int _ArgOne, double _ArgTwo, string _ArgThree, string _ArgFour="defaultVal") {
            List<byte[]> contents = new List<byte[]>();
            contents.Add(BitConverter.GetBytes(_ArgTwo));
            contents.Add(ASCIIEncoding.ASCII.GetBytes(_ArgThree));
            contents.Add(ASCIIEncoding.ASCII.GetBytes(_ArgFour));
            return PacketBuilder.Build(UID, contents);
    }
}