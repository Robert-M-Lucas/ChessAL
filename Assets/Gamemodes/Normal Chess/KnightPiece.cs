using System.Collections.Generic;

namespace Gamemodes.NormalChess
{
    public class KnightPiece : NormalChessPiece
    {
        public KnightPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 102;
            if (team != 0) AppearanceID += 6;
        }

        public override List<Move> GetMoves()
        {
            var moves = new List<Move>()
            {
                new (Position, Position + new V2(1, 2)),
                new (Position, Position + new V2(1, -2)),
                new (Position, Position + new V2(-1, 2)),
                new (Position, Position + new V2(-1, -2)),
                new (Position, Position + new V2(2, 1)),
                new (Position, Position + new V2(2, -1)),
                new (Position, Position + new V2(-2, 1)),
                new (Position, Position + new V2(-2, -1)),
            };
           
            return GUtil.RemoveFriendlies(GUtil.RemoveBlocked(moves, Board), Board);
        }

        public override AbstractPiece Clone(AbstractBoard newBoard) => new KnightPiece(Position, Team, newBoard);

        public override int GetUID() => 102;

        public override float GetValue() => 3f;
    }
}