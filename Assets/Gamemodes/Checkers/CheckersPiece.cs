using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.Checkers
{
    public class CheckersPiece : AbstractPiece
    {
        public bool Queen = false;

        public CheckersPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 701;
            if (team != 0) AppearanceID -= 1;
        }

        public override List<Move> GetMoves()
        {
            int m = 1;
            if (Team == 1) m = -1;

            List<Move> normal_moves = new List<Move>()
            {
                new Move(Position, Position + new V2(1, 1 * m)),
                new Move(Position, Position + new V2(-1, 1 * m))
            };

            if (Queen)
            {
                normal_moves.Add(new Move(Position, Position + new V2(1, -1 * m)));
                normal_moves.Add(new Move(Position, Position + new V2(-1, -1 * m)));
            }

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

        public override void OnMove(Move move)
        {
            if (move.From != Position) return;

            // Jump
            if (move.To.X - move.From.X != 1 && move.To.X - move.From.X != -1)
            {
                V2 pos = move.From + ((move.To - move.From) / 2);
                Board.PieceBoard[pos.X, pos.Y] = null;
                (Board.GameManager as GameManager).PieceTaken = true;
            }

            if ((Team == 0 && move.To.Y == 7) || (Team == 1 && move.To.Y == 0))
            {
                if (!Queen) AppearanceID += 2;
                Queen = true;
            }

            base.OnMove(move);
        }

        public override PieceSerialisationData GetData()
        {
            PieceSerialisationData data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();
            data.Data = new byte[1];
            data.Data = ArrayExtensions.Merge(data.Data, BitConverter.GetBytes(Queen), 0);
            return data;
        }

        public override void LoadData(PieceSerialisationData data)
        {
            Queen = BitConverter.ToBoolean(data.Data);
            if (Queen) AppearanceID += 2;
            base.LoadData(data);
        }

        public override int GetUID() => 700;
    }
}