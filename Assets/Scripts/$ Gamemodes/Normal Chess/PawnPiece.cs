using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class PawnPiece : AbstractPiece
    {
        public bool HasMoved = false;
        public int DashMove = -1;

        public PawnPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 0;
        }

        public override List<Move> GetMoves()
        {
            List<Move> forward_moves = new List<Move>();
            List<Move> attacking_moves = new List<Move>();
            List<Move> en_passant = new List<Move>();
            int m = 1;
            if (Team == 1) m = -1;

            forward_moves.Add(new Move(Position, Position + new V2(0, 1 * m)));

            // En passant logic

            if (GamemodeUtil.IsOnBoard(Position + new V2(1, 1 * m), Board) && // Is move to square empty
                Board.GetPiece(Position + new V2(1, 1 * m)) is null)
            {
                if (GamemodeUtil.IsOnBoard(Position + new V2(-1, 0), Board) && Board.GetPiece(Position + new V2(1, 0)) is not null) // Is there a piece beside me
                {
                    AbstractPiece piece = Board.GetPiece(Position + new V2(1, 0));
                    if (piece.GetUID() == GetUID() && // Is this a pawn, dashed last turn and not on my team
                        (piece as PawnPiece).DashMove == (Board.GameManager as GameManager).MoveCounter - 1 &&
                        (piece as PawnPiece).Team != Team) en_passant.Add(new Move(Position, Position + new V2(1, 1 * m)));
                }
            }
            else
            {
                attacking_moves.Add(new Move(Position, Position + new V2(1, 1 * m)));
            }

            if (GamemodeUtil.IsOnBoard(Position + new V2(-1, 1 * m), Board) &&
                Board.GetPiece(Position + new V2(-1, 1 * m)) is null)
            {
                if (GamemodeUtil.IsOnBoard(Position + new V2(-1, 0), Board) && Board.GetPiece(Position + new V2(-1, 0)) is not null)
                {
                    AbstractPiece piece = Board.GetPiece(Position + new V2(-1, 0));
                    if (piece.GetUID() == GetUID() && 
                        (piece as PawnPiece).DashMove == (Board.GameManager as GameManager).MoveCounter - 1 &&
                        (piece as PawnPiece).Team != Team) en_passant.Add(new Move(Position, Position + new V2(-1, 1 * m)));
                }
            }
            else
            {
                attacking_moves.Add(new Move(Position, Position + new V2(-1, 1 * m)));
            }

            attacking_moves = GamemodeUtil.RemoveNonEnemy(GamemodeUtil.RemoveBlocked(attacking_moves, Board), Board);

            if (!HasMoved)
            {
                forward_moves.Add(new Move(Position, Position + new V2(0, 2 * m)));
            }

            forward_moves = GamemodeUtil.RemoveNonEmpty(GamemodeUtil.RemoveBlocked(forward_moves, Board), Board);
            return forward_moves.Concat(attacking_moves).Concat(en_passant).ToList();
        }

        public override void OnMove(V2 from, V2 to)
        {
            HasMoved = true;
            if (to - from == new V2(0, 2) || to - from == new V2(0, -2))
            {
                DashMove = (Board.GameManager as GameManager).MoveCounter;
            }
            else if ((to - from).X != 0 && Board.GetPiece(to) is null)
            {
                if (Team == 0)
                {
                    Board.PieceBoard[to.X, to.Y - 1] = null;
                }
                else
                {
                    Board.PieceBoard[to.X, to.Y + 1] = null;
                }
            }

            if ((to.Y == 7 && Team == 0) || (to.Y == 0 && Team == 1))
            {
                Board.PieceBoard[to.X, to.Y] = new QueenPiece(to, Team, Board);
                Board.PieceBoard[from.X, from.Y] = null;
            }
        }

        public override int GetUID() => 2;
    }
}