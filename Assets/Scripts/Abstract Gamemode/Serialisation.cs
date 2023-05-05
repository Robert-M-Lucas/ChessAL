using System.Collections.Generic;
using System;

namespace Gamemodes
{
    /// <summary>
    /// Class to hold serialised game data
    /// </summary>
    public class SerialisationData
    {
        public int GamemodeUID = -1;
        public int TeamTurn = -1;
        public int PlayerOnTeamTurn = -1;
        public long ElapsedTime = -1;
        public byte[] GameManagerData = new byte[0];
        public byte[] BoardData = new byte[0];
        public List<PieceSerialisationData> PieceData = new List<PieceSerialisationData>();
    }

    /// <summary>
    /// Class to hold serialised piece data
    /// </summary>
    public class PieceSerialisationData
    {
        public int Team = -1;
        public V2 Position = new V2(-1, -1);
        public int UID = -1;
        public byte[] Data = new byte[0];
    }

    /// <summary>
    /// Utility class for serialisation
    /// </summary>
    public static class SerialisationUtil
    {
        /// <summary>
        /// Converts SerialisationData to a raw byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Construct(SerialisationData data) // SEE DOCUMENTATION FOR SAVE FILE FORMATTING
        {
            var length = 20; // GamemodeUID, TeamTurn, PlayerOnTeamTurn, EllapsedTime (long - 4 bytes)
            length += 4; // GameManagerData length
            length += data.GameManagerData.Length;
            length += 4; // BoardData Length
            length += data.BoardData.Length;

            foreach (var piece_data in data.PieceData)
            {
                length += 16; // Team, Position, UID
                length += 4; // PieceData length
                length += piece_data.Data.Length;
            }

            var output = new byte[length];

            var cursor = 0;

            ArrayExtensions.Merge(output, BitConverter.GetBytes(data.GamemodeUID), cursor);
            cursor += 4;
            ArrayExtensions.Merge(output, BitConverter.GetBytes(data.TeamTurn), cursor);
            cursor += 4;
            ArrayExtensions.Merge(output, BitConverter.GetBytes(data.PlayerOnTeamTurn), cursor);
            cursor += 4;

            ArrayExtensions.Merge(output, BitConverter.GetBytes(data.ElapsedTime), cursor);
            cursor += 8;

            ArrayExtensions.Merge(output, BitConverter.GetBytes(data.GameManagerData.Length), cursor);
            cursor += 4;
            ArrayExtensions.Merge(output, data.GameManagerData, cursor);
            cursor += data.GameManagerData.Length;

            ArrayExtensions.Merge(output, BitConverter.GetBytes(data.BoardData.Length), cursor);
            cursor += 4;
            ArrayExtensions.Merge(output, data.BoardData, cursor);
            cursor += data.BoardData.Length;

            foreach (var piece_data in data.PieceData)
            {
                ArrayExtensions.Merge(output, BitConverter.GetBytes(piece_data.Team), cursor);
                cursor += 4;
                ArrayExtensions.Merge(output, BitConverter.GetBytes(piece_data.Position.X), cursor);
                cursor += 4;
                ArrayExtensions.Merge(output, BitConverter.GetBytes(piece_data.Position.Y), cursor);
                cursor += 4;
                ArrayExtensions.Merge(output, BitConverter.GetBytes(piece_data.UID), cursor);
                cursor += 4;

                ArrayExtensions.Merge(output, BitConverter.GetBytes(piece_data.Data.Length), cursor);
                cursor += 4;
                ArrayExtensions.Merge(output, piece_data.Data, cursor);
                cursor += piece_data.Data.Length;
            }

            return output;
        }

        /// <summary>
        /// Returns the GamemodeUID from save data without having to decode the entire file
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        public static int GetGamemodeUID(byte[] saveData)
        {
            return BitConverter.ToInt32(ArrayExtensions.Slice(saveData, 0, 4));
        }

        /// <summary>
        /// Converts raw save data to SerialisationData
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        public static SerialisationData Deconstruct(byte[] saveData) // SEE DOCUMENTATION FOR SAVE FILE FORMATTING
        {
            var data = new SerialisationData();

            var cursor = 0;

            // ReSharper disable once UselessBinaryOperation
            data.GamemodeUID = BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
            cursor += 4;
            data.TeamTurn = BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
            cursor += 4;
            data.PlayerOnTeamTurn = BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
            cursor += 4;

            data.ElapsedTime = BitConverter.ToInt64(ArrayExtensions.Slice(saveData, cursor, cursor + 8));
            cursor += 8;

            var game_manager_data_length = BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
            cursor += 4;

            data.GameManagerData = ArrayExtensions.Slice(saveData, cursor, cursor + game_manager_data_length);
            cursor += game_manager_data_length;

            var board_data_length = BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
            cursor += 4;

            data.BoardData = ArrayExtensions.Slice(saveData, cursor, cursor + board_data_length);
            cursor += board_data_length;

            while (cursor < saveData.Length)
            {
                var piece_data = new PieceSerialisationData();

                piece_data.Team= BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
                cursor += 4;
                piece_data.Position.X= BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
                cursor += 4;
                piece_data.Position.Y = BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
                cursor += 4;
                piece_data.UID = BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
                cursor += 4;

                var piece_data_length = BitConverter.ToInt32(ArrayExtensions.Slice(saveData, cursor, cursor + 4));
                cursor += 4;

                piece_data.Data = ArrayExtensions.Slice(saveData, cursor, cursor + piece_data_length);
                cursor += piece_data_length;

                data.PieceData.Add(piece_data);
            }

            return data;
        }
    }
}

