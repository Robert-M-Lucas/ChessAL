using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 100;

        public override string GetName() => "Normal Chess";

        public override string GetDescription()
        {
            return @"Normal Chess

Must have 2 teams of 1

Traditional chess played on an 8x8 board";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : AbstractGameManager
    {
        
        public bool CancelDefaultMove;

        public GameManager(AbstractGameManagerData d, ChessManager chessManager) : base(d, chessManager)
        {
            Board = new Board(this);
        }

        public override List<Move> GetMoves()
        {
            (Board as Board).VirtualTeam = chessManager.GetLocalPlayerTeam();
            List<Move> possible_moves = Board.GetMoves();
            
            int i = 0;
            while (i < possible_moves.Count)
            {
                Board temp_board = (Board as Board).Clone();
                FalseOnMove(temp_board, possible_moves[i].From, possible_moves[i].To);
                temp_board.VirtualTeam = GUtil.SwitchTeam(chessManager);

                bool failed = false;
                List<Move> possible_enemy_moves = temp_board.GetMoves();
                foreach (Move move in possible_enemy_moves)
                {
                    AbstractPiece piece = temp_board.GetPiece(move.To);
                    if (piece is not null && piece.GetUID() == PieceUIDs.KING && piece.Team == chessManager.GetLocalPlayerTeam())
                    {
                        failed = true;
                        break;
                    }
                }

                if (failed)
                {
                    possible_moves.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            return possible_moves;
        }

        protected int FalseOnMove(AbstractBoard board, V2 from, V2 to)
        {
            CancelDefaultMove = false;

            board.OnMove(from, to);

            if (!CancelDefaultMove)
            {
                board.PieceBoard[to.X, to.Y] = board.PieceBoard[from.X, from.Y];
                board.PieceBoard[to.X, to.Y].Position = to;
                board.PieceBoard[from.X, from.Y] = null;
            }

            (board as Board).MoveCounter++;

            bool white_king = false;
            bool black_king = false;
            for (int x = 0; x < board.PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < board.PieceBoard.GetLength(1); y++)
                {
                    if (board.PieceBoard[x, y] is not null)
                    {
                        if (board.PieceBoard[x, y].GetUID() == PieceUIDs.KING)
                        {
                            if (board.PieceBoard[x, y].Team == 0) white_king = true;
                            else black_king = true;
                        }
                    }
                }
            }

            if (!white_king) return GUtil.TurnEncodeTeam(1);
            if (!black_king) return GUtil.TurnEncodeTeam(0);

            return GUtil.SwitchTeam(chessManager);
        }

        public override int OnMove(V2 from, V2 to)
        {
            return FalseOnMove(Board, from, to);
        }
    }
}