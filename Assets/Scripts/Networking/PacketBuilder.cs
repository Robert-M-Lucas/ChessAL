using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets
{
    /// <summary>
    /// Exception during packet decoding
    /// </summary>
    public class PacketDecodeError : Exception
    {
        public PacketDecodeError()
        { }

        public PacketDecodeError(string message) : base(message)
        {
        }

        public PacketDecodeError(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// A generic packet - no specific types
    /// </summary>
    public struct Packet
    {
        public int UID;
        public int From;
        public List<byte[]> Contents;

        public Packet(int uid, List<byte[]> contents, int from = -1)
        {
            UID = uid;
            Contents = contents;
            From = from;
        }
    }

    /*
    public class PacketMissingAttributeException : Exception
    {
        public PacketMissingAttributeException()
        { }

        public PacketMissingAttributeException(string message) : base(message)
        {
        }

        public PacketMissingAttributeException(string message, Exception inner) : base(message, inner)
        { }
    }
    */

    /// <summary>
    /// Creates and decodes packets
    /// </summary>
    public static class PacketBuilder
    {
        private static Encoding _encoder = new UTF8Encoding();

        public const int PacketLenLen = 4;
        public const int UIDLen = 4;
        public const int DataLenLen = 4;

        /// <summary>
        /// Decodes packet length from packet
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Packet length</returns>
        public static int GetPacketLength(byte[] content)
        {
            return BitConverter.ToInt32(ArrayExtensions.Slice(content, 0, PacketLenLen)) + PacketLenLen;
        }

        /// <summary>
        /// Builds the packet - Adds the UID and calculates lengths for each section of the packet
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        /// <exception cref="PacketDecodeError"></exception>
        public static byte[] Build(int UID, List<byte[]> contents)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int cursor = PacketLenLen;
                ArrayExtensions.Merge(buffer, BitConverter.GetBytes(UID), cursor);
                cursor += UIDLen;

                foreach (byte[] c in contents)
                {
                    ArrayExtensions.Merge(buffer, BitConverter.GetBytes(c.Length), cursor);
                    cursor += 4;
                    ArrayExtensions.Merge(buffer, c, cursor);
                    cursor += c.Length;
                }

                // Add packet length
                ArrayExtensions.Merge(buffer, BitConverter.GetBytes(cursor - 4), 0);

                return ArrayExtensions.Slice(buffer, 0, cursor);
            }
            catch (Exception e)
            {
                throw new PacketDecodeError("Error decoding packet: " + e);
            }
        }

        /// <summary>
        /// Converts a string to bytes with the current encoding system
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Encoded bytes</returns>
        public static byte[] ByteEncode(string input)
        {
            return _encoder.GetBytes(input);
        }

        /// <summary>
        /// Separates raw packet data into its separate components
        /// </summary>
        /// <param name="data"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Packet Decode(byte[] data, int from = -1)
        {
            int cursor = 4;
            int UID = BitConverter.ToInt32(ArrayExtensions.Slice(data, cursor, cursor + UIDLen));
            cursor += UIDLen;

            List<byte[]> contents = new List<byte[]>();
            while (cursor < data.Length)
            {
                int data_len = BitConverter.ToInt32(
                    ArrayExtensions.Slice(data, cursor, cursor + DataLenLen)
                );
                data_len = BitConverter.ToInt32(
                    ArrayExtensions.Slice(data, cursor, cursor + DataLenLen)
                );
                cursor += DataLenLen;
                byte[] content = ArrayExtensions.Slice(data, cursor, cursor + data_len);
                cursor += data_len;
                contents.Add(content);
            }

            return new Packet(UID, contents, from);
        }
    }
}