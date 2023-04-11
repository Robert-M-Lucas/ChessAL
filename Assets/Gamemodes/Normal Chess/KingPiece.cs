using System;
using System.Collections.Generic;

namespace Gamemodes.NormalChess
{
    public class KingPiece : NormalChessPiece
    {
        public bool HasMoved;

        public KingPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 101;
            if (team != 0) AppearanceID += 6;
        }

        public override List<Move> GetMoves()
        {
            var moves = new List<Move>()
            {
                new(Position, Position + new V2(1, 1)),
                new(Position, Position + new V2(1, -1)),
                new(Position, Position + new V2(-1, 1)),
                new(Position, Position + new V2(-1, -1)),
                new(Position, Position + new V2(1, 0)),
                new(Position, Position + new V2(0, 1)),
                new(Position, Position + new V2(-1, 0)),
                new(Position, Position + new V2(0, -1)),
            };

            // Castle logic
            if (!HasMoved)
            {
                if (Board.GetPiece(Position + new V2(-4, 0)) is not null
                    && typeof(RookPiece) == Board.GetPiece(Position + new V2(-4, 0)).GetType()
                    && !((RookPiece)Board.GetPiece(Position + new V2(-4, 0))).HasMoved) // If rook is there and hasn't moved
                {
                    // Is there clear path to rook?
                    var clear = true;
                    for (var x = 1; x <= 3; x++)
                    {
                        if (Board.GetPiece(new V2(x, Position.Y)) is null) continue;
                        clear = false; break;
                    }
                    if (clear) moves.Add(new Move(Position, Position + new V2(-2, 0))); // Add castle move
                }

                // Same for castling on other side
                if (Board.GetPiece(Position + new V2(3, 0)) is null
                    || typeof(RookPiece) != Board.GetPiece(Position + new V2(3, 0)).GetType()
                    || ((RookPiece) Board.GetPiece(Position + new V2(3, 0))).HasMoved)
                    return GUtil.RemoveFriendlies(GUtil.RemoveBlocked(moves, Board), Board);
                {
                    var clear = true;
                    for (var x = 5; x <= 6; x++)
                    {
                        if (Board.GetPiece(new V2(x, Position.Y)) is null) continue;
                        clear = false; break;
                    }
                    if (clear) moves.Add(new Move(Position, Position + new V2(2, 0)));
                }
            }   

            // Return moves that don't move onto blocked or friendly squares
            return GUtil.RemoveFriendlies(GUtil.RemoveBlocked(moves, Board), Board);
        }

        public override void OnMove(Move move, bool thisPiece)
        {
            if (!thisPiece) return;

            HasMoved = true;

            switch (move.To.X - move.From.X)
            {
                // Castle (king can only move two placen in a castle
                case 2:
                    // Move rook
                    Board.PieceBoard[move.To.X - 1, move.To.Y] = Board.PieceBoard[move.To.X + 1, move.To.Y];
                    Board.PieceBoard[move.To.X - 1, move.To.Y].Position = new V2(move.To.X - 1, move.To.Y);
                    Board.PieceBoard[move.To.X - 1, move.To.Y].OnMove(move, false);

                    // Set has moved
                    ((RookPiece) Board.PieceBoard[move.To.X - 1, move.To.Y]).HasMoved = true;  

                    // Remove old rook position
                    Board.PieceBoard[move.To.X + 1, move.To.Y] = null;
                    break;
                // Castle on other side
                case -2:
                    Board.PieceBoard[move.To.X + 1, move.To.Y] = Board.PieceBoard[move.To.X - 2, move.To.Y];
                    Board.PieceBoard[move.To.X + 1, move.To.Y].Position = new V2(move.To.X + 1, move.To.Y);
                    Board.PieceBoard[move.To.X + 1, move.To.Y].OnMove(move, false);
                    ((RookPiece) Board.PieceBoard[move.To.X + 1, move.To.Y]).HasMoved = true;
                    Board.PieceBoard[move.To.X - 2, move.To.Y] = null;
                    break;
            }
        }

        public override PieceSerialisationData GetData()
        {
            var data = new PieceSerialisationData
            {
                Team = Team,
                Position = Position,
                UID = GetUID(),
                Data = BitConverter.GetBytes(HasMoved) // Add custom data
            };
            return data;
        }

        public override void LoadData(PieceSerialisationData data)
        {
            Position = data.Position;
            Team = data.Team;
            HasMoved = BitConverter.ToBoolean(data.Data); // Load custom data
        }

        public override AbstractPiece Clone(AbstractBoard newBoard)
        {
            var k = new KingPiece(Position, Team, newBoard);
            k.HasMoved = HasMoved;
            return k;
        }

        public override int GetUID() => 101;

        public override float GetValue() => 1000f;
    }
}