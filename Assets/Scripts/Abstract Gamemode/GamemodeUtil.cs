using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes
{
    public static class GamemodeUtil
    {
        public static List<Move> RemoveBlocked(List<Move> moves, AbstractBoard board)
        {
            BoardRenderInfo boardRenderInfo = board.GetBoardRenderInfo();

            for (int i = 0; i < moves.Count; i++)
            {
                if (boardRenderInfo.RemovedSquares.Contains(moves[i].To) || moves[i].To.X < 0 || moves[i].To.Y < 0 || moves[i].To.X >= boardRenderInfo.BoardSize || moves[i].To.Y >= boardRenderInfo.BoardSize)
                {
                    moves.RemoveAt(i);
                    i--;
                }
            }

            return moves;
        }

        public static List<Move> RemoveFriendlies(List<Move> moves, AbstractBoard board)
        {
            BoardRenderInfo boardRenderInfo = board.GetBoardRenderInfo();

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
            BoardRenderInfo boardRenderInfo = board.GetBoardRenderInfo();

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

        public static int SwitchPlayerTeam(ChessManager chessManager)
        {
            int team = chessManager.GetLocalPlayerTeam();
            
            if (team == 0) team = 1;
            else team = 0;

            return chessManager.GetPlayerByTeam(team, 0);
        }
    }
}