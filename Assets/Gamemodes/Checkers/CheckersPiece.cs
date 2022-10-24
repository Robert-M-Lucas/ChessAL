using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.Checkers
{
    public class CheckersPiece : AbstractPiece
    {
        public CheckersPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 700;
            if (team != 0) AppearanceID += 1;
        }

        public override List<Move> GetMoves()
        {
            List<Move> normal_moves = new List<Move>()
            {
                new Move(Position, Position + new V2(1, 1)),
                new Move(Position, Position + new V2(-1, 1)),
                new Move(Position, Position + new V2(1, -1)),
                new Move(Position, Position + new V2(-1, -1))
            };

            normal_moves = GUtil.RemoveNonEmpty(GUtil.RemoveBlocked(normal_moves, Board), Board);

            List<Move> jump_moves = new List<Move>()
            {
                new Move(Position, Position + new V2(1, 1)),
                new Move(Position, Position + new V2(-1, 1)),
                new Move(Position, Position + new V2(1, -1)),
                new Move(Position, Position + new V2(-1, -1))
            };
            jump_moves = GUtil.RemoveFriendlies(GUtil.RemoveEmpty(GUtil.RemoveBlocked(jump_moves, Board), Board), Board);

            for (int i = 0; i < jump_moves.Count; i++)
            {
                V2 to = jump_moves[i].To;
                to -= Position;
                to *= 2;
                to += Position;
                jump_moves[i] = new Move(jump_moves[i].From, to);
            }

            jump_moves = GUtil.RemoveNonEmpty(GUtil.RemoveBlocked(jump_moves, Board), Board);

            return normal_moves.Concat(jump_moves).ToList();
        }

        public override void OnMove(V2 from, V2 to)
        {
            // Jump
            if (to.X - from.X != 1 && to.X - from.X != -1)
            {
                V2 pos = from + ((to - from) / 2);
                Board.PieceBoard[pos.X, pos.Y] = null;
                (Board.GameManager as GameManager).PieceTaken = true;
            }

            base.OnMove(from, to);
        }

        public override int GetUID() => 700;
    }
}