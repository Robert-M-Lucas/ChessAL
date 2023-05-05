using System.Collections.Generic;
using System;
using Game;

namespace Gamemodes.NormalChess
{
    /// <summary>
    /// Adds scores to pieces based on their positions
    /// </summary>
    public static class Heatmap
    {
        /// <summary>
        /// Returns float representing the strength of a pieces position on the board relative to the gameData.LocalPlayer
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public static float GetHeatmapScore(LiveGameData gameData, Board board)
        {
            float total = 0;

            // Float arrays represent value of piece in given position. Top is current player.
            var heatmaps = new Dictionary<Type, float[,]>
            {
                {
                    typeof(PawnPiece),
                    new float[,]
                    {
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0.1f, 0.1f, 0.1f, 0.15f, 0.15f, 0.1f, 0.1f, 0.1f },
                        { 0.2f, 0.2f, 0.22f, 0.25f, 0.25f, 0.22f, 0.2f, 0.2f },
                        { 0.3f, 0.3f, 0.32f, 0.33f, 0.33f, 0.32f, 0.3f, 0.3f },
                        { 0.35f, 0.35f, 0.37f, 0.4f, 0.4f, 0.37f, 0.35f, 0.35f },
                        { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f },
                        { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f },
                    }
                },
                {
                    typeof(KingPiece),
                    new float[,]
                    {
                        { 0.3f, 0.2f, 0.1f, 0, 0, 0.1f, 0.2f, 0.3f },
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0 },
                    }
                },
                {
                    typeof(RookPiece),
                    new float[,]
                    {
                        { 0f, 0.05f, 0.1f, 0.15f, 0.15f, 0.1f, 0.05f, 0f },
                        { 0.1f, 0.1f, 0.1f, 0.2f, 0.2f, 0.1f, 0.1f, 0.1f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                    }
                },
                {
                    typeof(QueenPiece),
                    new float[,]
                    {
                        { 0f, 0.05f, 0.1f, 0.15f, 0.15f, 0.1f, 0.05f, 0f },
                        { 0.1f, 0.1f, 0.1f, 0.2f, 0.2f, 0.1f, 0.1f, 0.1f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                    }
                },
                {
                    typeof(BishopPiece),
                    new float[,]
                    {
                        { 0f, 0.05f, 0.1f, 0.15f, 0.15f, 0.1f, 0.05f, 0f },
                        { 0.1f, 0.1f, 0.1f, 0.2f, 0.2f, 0.1f, 0.1f, 0.1f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                    }
                },
                {
                    typeof(KnightPiece),
                    new float[,]
                    {
                        { 0f, 0.05f, 0.1f, 0.15f, 0.15f, 0.1f, 0.05f, 0f },
                        { 0.1f, 0.1f, 0.1f, 0.2f, 0.2f, 0.1f, 0.1f, 0.1f },
                        { 0.2f, 0.2f, 0.25f, 0.2f, 0.2f, 0.25f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                        { 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f },
                    }
                },
            };

            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    if (board.PieceBoard[x, y] is null) continue;

                    if (!heatmaps.ContainsKey(board.PieceBoard[x, y].GetType())) continue;
                    var true_y = y;

                    // Uses multiplier to add or subtract points based on whose side the pieces belong to
                    var multiplier = 1;
                    if (board.PieceBoard[x, y].Team != gameData.LocalPlayerTeam)
                        multiplier = -1;

                    // Flip board if on other team
                    if (board.PieceBoard[x, y].Team != 0)
                        true_y = 7 - y;

                    // Add heatmap score
                    total += heatmaps[board.PieceBoard[x, y].GetType()][true_y, x] * multiplier;
                }
            }

            return total;
        }
    }

}