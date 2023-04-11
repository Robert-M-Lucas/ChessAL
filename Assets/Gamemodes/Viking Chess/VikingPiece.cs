using System.Collections.Generic;
using System.Linq;

namespace Gamemodes.VikingChess
{
    public class VikingPiece : AbstractPiece
    {
        public VikingPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = team == 0 ? 110 : 104;
        }

        public override int GetUID() => PieceUIDs.PIECE;

        public override List<Move> GetMoves()
        {
            // Rook like moves
            var moves = GUtil.RaycastMoves(this, new V2(1, 0), Board)
                .Concat(GUtil.RaycastMoves(this, new V2(-1, 0), Board))
                .Concat(GUtil.RaycastMoves(this, new V2(0, 1), Board))
                .Concat(GUtil.RaycastMoves(this, new V2(0, -1), Board))
                .ToList();

            // Remove occupied squares
            moves = GUtil.RemoveNonEmpty(moves, Board);

            // Don't allow stopping in centre
            if (GetUID() != PieceUIDs.PIECE) return moves;
            for (var i = 0; i < moves.Count; i++)
            {
                if (((moves[i].To.X != 0 && moves[i].To.X != 10) || (moves[i].To.Y != 0 && moves[i].To.Y != 10)) &&
                    moves[i].To != VikingChess.Board.CENTRE) continue;
                moves.RemoveAt(i);
                i--;
            }

            return moves;
        }

        public override AbstractPiece Clone(AbstractBoard newBoard) => new VikingPiece(Position, Team, newBoard);
    }

}
