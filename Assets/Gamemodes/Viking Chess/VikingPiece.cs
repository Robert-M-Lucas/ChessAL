using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.VikingChess
{
    public class VikingPiece : AbstractPiece
    {
        public VikingPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            if (team == 0) AppearanceID = 110;
            else AppearanceID = 104;
        }

        public override int GetUID() => PieceUIDs.Piece;

        public override List<Move> GetMoves()
        {
            // Rook like moves
            List<Move> moves = GUtil.RaycastMoves(this, new V2(1, 0), Board)
                .Concat(GUtil.RaycastMoves(this, new V2(-1, 0), Board))
                .Concat(GUtil.RaycastMoves(this, new V2(0, 1), Board))
                .Concat(GUtil.RaycastMoves(this, new V2(0, -1), Board))
                .ToList();

            // Remove occupied squares
            moves = GUtil.RemoveNonEmpty(moves, Board);

            // Don't allow stopping in centre
            if (GetUID() == PieceUIDs.Piece)
            {
                for (int i = 0; i < moves.Count; i++)
                {
                    if ((moves[i].To.X == 0 || moves[i].To.X == 10) && (moves[i].To.Y == 0 || moves[i].To.Y == 10) ||
                        moves[i].To == VikingChess.Board.CENTRE)
                    {
                        moves.RemoveAt(i);
                        i--;
                    }
                }
            }

            return moves;
        }

        public override AbstractPiece Clone(AbstractBoard newBoard) => new VikingPiece(Position, Team, newBoard);
    }

}
