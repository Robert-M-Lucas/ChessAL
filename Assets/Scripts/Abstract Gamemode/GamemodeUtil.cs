using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes
{
    /// <summary>
    /// Utility class for the gamemode system
    /// </summary>
    public static class GUtil
    {
        public static List<Move> RemoveBlocked(List<Move> moves, AbstractBoard board)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                if (!IsOnBoard(moves[i].To, board))
                {
                    moves.RemoveAt(i);
                    i--;
                }
            }

            return moves;
        }

        public static bool IsOnBoard(V2 position, AbstractBoard board)
        {
            BoardRenderInfo boardRenderInfo = board.GetBoardRenderInfo();

            return !(boardRenderInfo.RemovedSquares.Contains(position) || position.X < 0 || position.Y < 0
                    || position.X >= boardRenderInfo.BoardSize || position.Y >= boardRenderInfo.BoardSize);
        }

        public static List<Move> RemoveFriendlies(List<Move> moves, AbstractBoard board)
        {
            // BoardRenderInfo boardRenderInfo = board.GetBoardRenderInfo();

            for (int i = 0; i < moves.Count; i++)
            {
                if (board.PieceBoard[moves[i].To.X, moves[i].To.Y] is not null && board.PieceBoard[moves[i].To.X, moves[i].To.Y].Team == board.PieceBoard[moves[i].From.X, moves[i].From.Y].Team)
                {
                    moves.RemoveAt(i);
                    i--;
                }
            }

            return moves;
        }

        public static List<Move> RemoveNonEnemy(List<Move> moves, AbstractBoard board)
        {
            BoardRenderInfo boardRenderInfo = board.GetBoardRenderInfo();

            for (int i = 0; i < moves.Count; i++)
            {
                if (board.PieceBoard[moves[i].To.X, moves[i].To.Y] is null || board.PieceBoard[moves[i].To.X, moves[i].To.Y].Team == board.PieceBoard[moves[i].From.X, moves[i].From.Y].Team)
                {
                    moves.RemoveAt(i);
                    i--;
                }
            }

            return moves;
        }

        public static List<Move> RemoveNonEmpty(List<Move> moves, AbstractBoard board)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                if (board.PieceBoard[moves[i].To.X, moves[i].To.Y] is not null)
                {
                    moves.RemoveAt(i);
                    i--;
                }
            }

            return moves;
        }

        public static List<Move> RemoveEmpty(List<Move> moves, AbstractBoard board)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                if (board.PieceBoard[moves[i].To.X, moves[i].To.Y] is null)
                {
                    moves.RemoveAt(i);
                    i--;
                }
            }

            return moves;
        }


        /// <summary>
        /// Returns a list of moves in the given direction. Stops before hitting a friendly or one move after hitting an enemy
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="direction"></param>
        /// <param name="board"></param>
        /// <param name="maxMoves"></param>
        /// <returns></returns>
        public static List<Move> RaycastMoves(AbstractPiece piece, V2 direction, AbstractBoard board, int maxMoves = -1)
        {
            List<Move> moves = new List<Move>();

            V2 current_pos = piece.Position + direction;
            int move = 0;
            while (true)
            {
                if (move == maxMoves) break;
                if (!IsOnBoard(current_pos, board)) break;

                if (board.PieceBoard[current_pos.X, current_pos.Y] is not null)
                {
                    if (board.PieceBoard[current_pos.X, current_pos.Y].Team == piece.Team) break;
                    moves.Add(new Move(piece.Position, current_pos));
                    break;
                }

                moves.Add(new Move(piece.Position, current_pos));
                move++;
                current_pos += direction;
            }

            return moves;
        }

        public static int SwitchPlayerTeam(LiveGameData gameData)
        {
            int team = gameData.CurrentTeam;
            
            if (team == 0) team = 1;
            else team = 0;

            return gameData.GetPlayerByTeam(team, 0);
        }

        public static int SwitchTeam(LiveGameData gameData)
        {
            int team = gameData.CurrentTeam;

            if (team == 0) team = 1;
            else team = 0;

            return team;
        }

        public static int TurnEncodeTeam(int winningTeam)
        {
            return -winningTeam - 1;
        }

        public static int TurnDecodeTeam(int winningTeam)
        {
            return -(winningTeam + 1);
        }
    }
}