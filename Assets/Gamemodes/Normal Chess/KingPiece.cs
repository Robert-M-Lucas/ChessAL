using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class KingPiece : AbstractPiece
    {
        public KingPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 101;
            if (team != 0) AppearanceID += 6;
        }

        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>()
            {
                new Move(Position, Position + new V2(1, 1)),
                new Move(Position, Position + new V2(1, -1)),
                new Move(Position, Position + new V2(-1, 1)),
                new Move(Position, Position + new V2(-1, -1)),
                new Move(Position, Position + new V2(1, 0)),
                new Move(Position, Position + new V2(0, 1)),
                new Move(Position, Position + new V2(-1, 0)),
                new Move(Position, Position + new V2(0, -1)),
            };
           
            return GUtil.RemoveFriendlies(GUtil.RemoveBlocked(moves, Board), Board);
        }

        /*
        public override void OnMove(V2 from, V2 to)
        {

        }
        */

        public override int GetUID() => 3;
    }
}