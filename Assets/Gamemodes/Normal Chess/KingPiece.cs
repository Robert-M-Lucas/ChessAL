using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

            // Castle logic
            if (!HasMoved)
            {
                if (Board.GetPiece(Position + new V2(-4, 0)) is not null
                    && typeof(RookPiece) == Board.GetPiece(Position + new V2(-4, 0)).GetType()
                    && !((RookPiece)Board.GetPiece(Position + new V2(-4, 0))).HasMoved) // If rook is there and hasn't moved
                {
                    // Is there clear path to rook?
                    bool clear = true;
                    for (int x = 1; x <= 3; x++)
                    {
                        if (Board.GetPiece(new V2(x, Position.Y)) is not null)
                        {
                            clear = false; break;
                        }
                    }
                    if (clear) moves.Add(new Move(Position, Position + new V2(-2, 0))); // Add castle move
                }

                // Same for castling on other side
                if (Board.GetPiece(Position + new V2(3, 0)) is not null
                    && typeof(RookPiece) == Board.GetPiece(Position + new V2(3, 0)).GetType()
                    && !((RookPiece)Board.GetPiece(Position + new V2(3, 0))).HasMoved) 
                {
                    bool clear = true;
                    for (int x = 5; x <= 6; x++)
                    {
                        if (Board.GetPiece(new V2(x, Position.Y)) is not null)
                        {
                            clear = false; break;
                        }
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

            // Castle (king can only move two placen in a castle
            if (move.To.X - move.From.X == 2)
            {
                // Move rook
                Board.PieceBoard[move.To.X - 1, move.To.Y] = Board.PieceBoard[move.To.X + 1, move.To.Y];
                Board.PieceBoard[move.To.X - 1, move.To.Y].Position = new V2(move.To.X - 1, move.To.Y);
                Board.PieceBoard[move.To.X - 1, move.To.Y].OnMove(move, false);

                // Set has moved
                (Board.PieceBoard[move.To.X - 1, move.To.Y] as RookPiece).HasMoved = true;  

                // Remove old rook position
                Board.PieceBoard[move.To.X + 1, move.To.Y] = null;
            }
            // Castle on other side
            else if (move.To.X - move.From.X == -2)
            {
                Board.PieceBoard[move.To.X + 1, move.To.Y] = Board.PieceBoard[move.To.X - 2, move.To.Y];
                Board.PieceBoard[move.To.X + 1, move.To.Y].Position = new V2(move.To.X + 1, move.To.Y);
                Board.PieceBoard[move.To.X + 1, move.To.Y].OnMove(move, false);
                (Board.PieceBoard[move.To.X + 1, move.To.Y] as RookPiece).HasMoved = true;
                Board.PieceBoard[move.To.X - 2, move.To.Y] = null;
            }
        }

        public override PieceSerialisationData GetData()
        {
            PieceSerialisationData data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();
            data.Data = BitConverter.GetBytes(HasMoved); // Add custom data
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
            KingPiece k = new KingPiece(Position, Team, newBoard);
            k.HasMoved = HasMoved;
            return k;
        }

        public override int GetUID() => 101;

        public override float GetValue() => 1000f;
    }
}