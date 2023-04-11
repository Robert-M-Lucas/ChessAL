using System.Collections.Generic;
using System.Linq;

namespace Gamemodes.NormalChess
{
    public class BishopPiece : NormalChessPiece
    {
        public BishopPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 103;
            if (team != 0) AppearanceID += 6;
            
        }
        
        public override List<Move> GetMoves()
        {
            var moves = new List<Move>();

            var directions = new V2[] { new V2(1, 1), new V2(-1, 1), new V2(1, -1), new V2(-1, -1) };

            return directions.Aggregate(moves, (current, direction) => current.Concat(GUtil.RaycastMoves(this, direction, Board)).ToList());
        }

        public override AbstractPiece Clone(AbstractBoard newBoard) => new BishopPiece(Position, Team, newBoard);

        public override int GetUID() => 103;

        public override float GetValue() => 3.2f;
    }
}