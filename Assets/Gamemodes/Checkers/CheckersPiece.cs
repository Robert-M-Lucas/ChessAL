using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamemodes.Checkers
{
    public class CheckersPiece : AbstractPiece
    {
        public bool Queen;

        public CheckersPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = PieceUIDs.CHECKER;
            if (team == 0) AppearanceID += 1;
        }

        public override List<Move> GetMoves()
        {
            var m = 1;
            if (Team == 1) m = -1;

            var normal_moves = new List<Move>()
            {
                new Move(Position, Position + new V2(1, 1 * m)),
                new Move(Position, Position + new V2(-1, 1 * m))
            };

            // Queen can move backwards
            if (Queen)
            {
                normal_moves.Add(new Move(Position, Position + new V2(1, -1 * m)));
                normal_moves.Add(new Move(Position, Position + new V2(-1, -1 * m)));
            }

            normal_moves = GUtil.RemoveNonEmpty(GUtil.RemoveBlocked(normal_moves, Board), Board);

            // Create list of squares around piece
            var jump_moves = new List<Move>()
            {
                new Move(Position, Position + new V2(1, 1)),
                new Move(Position, Position + new V2(-1, 1)),
                new Move(Position, Position + new V2(1, -1)),
                new Move(Position, Position + new V2(-1, -1))
            };
            // Remove all unoccupied or friendly occupied
            jump_moves = GUtil.RemoveFriendlies(GUtil.RemoveEmpty(GUtil.RemoveBlocked(jump_moves, Board), Board), Board);

            for (var i = 0; i < jump_moves.Count; i++)
            {
                var to = jump_moves[i].To;
                to -= Position; // Make irrelevant of position
                to *= 2; // Double distance
                to += Position; // Make relative to position
                jump_moves[i] = new Move(jump_moves[i].From, to); // Replace
            }

            // Remove blocked
            jump_moves = GUtil.RemoveNonEmpty(GUtil.RemoveBlocked(jump_moves, Board), Board);

            return normal_moves.Concat(jump_moves).ToList();
        }

        public override void OnMove(Move move, bool thisPiece)
        {
            if (thisPiece)
            {
                // Jump
                if (move.To.X - move.From.X != 1 && move.To.X - move.From.X != -1)
                {
                    var pos = move.From + ((move.To - move.From) / 2);
                    Board.PieceBoard[pos.X, pos.Y] = null;
                    ((GameManager) Board.GameManager).PieceTaken = true;
                }

                // Become queen
                if ((Team == 0 && move.To.Y == 7) || (Team == 1 && move.To.Y == 0))
                {
                    if (!Queen) AppearanceID += 2;
                    Queen = true;
                }
            }

            base.OnMove(move, thisPiece);
        }

        public override PieceSerialisationData GetData()
        {
            var data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();
            data.Data = BitConverter.GetBytes(Queen); // Save custom data
            return data;
        }

        public override void LoadData(PieceSerialisationData data)
        {
            Queen = BitConverter.ToBoolean(data.Data); // Load custom data
            if (Queen) AppearanceID += 2;
            base.LoadData(data);
        }

        public override int GetUID() => 700;

        public override AbstractPiece Clone(AbstractBoard newBoard)
        {
            var piece = new CheckersPiece(Position, Team, newBoard);
            piece.Queen = Queen;
            return piece;
        }

        public override float GetValue()
        {
            return Queen ? 3f : 1f;
        }
    }
}