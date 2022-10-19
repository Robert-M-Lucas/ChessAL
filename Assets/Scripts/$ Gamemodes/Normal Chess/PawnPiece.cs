using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class PawnPiece : AbstractPiece
    {
        public int MoveCount = 0;

        public PawnPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 0;
        }

        public override List<Move> GetMoves()
        {
            List<Move> forward_moves = new List<Move>();
            List<Move> attacking_moves = new List<Move>();
            if (Team == 0)
            {
                forward_moves.Add(new Move(Position, Position + new V2(0, 1)));
                attacking_moves.Add(new Move(Position, Position + new V2(1, 1)));
                attacking_moves.Add(new Move(Position, Position + new V2(-1, 1)));
                if (MoveCount == 0)
                {
                    forward_moves.Add(new Move(Position, Position + new V2(0, 2)));
                }
            }
            else
            {
                forward_moves.Add(new Move(Position, Position + new V2(0, -1)));
                attacking_moves.Add(new Move(Position, Position + new V2(1, -1)));
                attacking_moves.Add(new Move(Position, Position + new V2(-1, -1)));
                if (MoveCount == 0)
                {
                    forward_moves.Add(new Move(Position, Position + new V2(0, -2)));
                }
            }
            forward_moves = GamemodeUtil.RemoveNonEmpty(GamemodeUtil.RemoveBlocked(forward_moves, Board), Board);
            attacking_moves = GamemodeUtil.RemoveNonEnemy(GamemodeUtil.RemoveBlocked(attacking_moves, Board), Board);
            return forward_moves.Concat(attacking_moves).ToList();
        }

        public override void OnMove(V2 from, V2 to)
        {
            if (to == Position) MoveCount++;
        }

        public override int GetUID() => 2;
    }
}