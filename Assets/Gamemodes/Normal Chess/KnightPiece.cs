using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            List<Move> moves = new List<Move>()
            {
                new Move(Position, Position + new V2(1, 2)),
                new Move(Position, Position + new V2(1, -2)),
                new Move(Position, Position + new V2(-1, 2)),
                new Move(Position, Position + new V2(-1, -2)),
                new Move(Position, Position + new V2(2, 1)),
                new Move(Position, Position + new V2(2, -1)),
                new Move(Position, Position + new V2(-2, 1)),
                new Move(Position, Position + new V2(-2, -1)),
            };
           
            return GUtil.RemoveFriendlies(GUtil.RemoveBlocked(moves, Board), Board);
        }

        /*
        public override void OnMove(V2 from, V2 to)
        {

        }
        */

        public override AbstractPiece Clone(AbstractBoard newBoard) => new KnightPiece(Position, Team, newBoard);

        public override int GetUID() => 102;

        public override float GetValue() => 3f;
    }
}