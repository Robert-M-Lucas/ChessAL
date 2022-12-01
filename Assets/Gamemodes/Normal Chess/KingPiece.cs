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
                    && !((RookPiece)Board.GetPiece(Position + new V2(-4, 0))).HasMoved) 
                {
                    bool clear = true;
                    for (int x = 1; x <= 3; x++)
                    {
                        if (Board.GetPiece(new V2(x, Position.Y)) is not null)
                        {
                            clear = false; break;
                        }
                    }
                    if (clear) moves.Add(new Move(Position, Position + new V2(-2, 0)));
                }
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

            return GUtil.RemoveFriendlies(GUtil.RemoveBlocked(moves, Board), Board);
        }

        public override void OnMove(Move move)
        {
            HasMoved = true;

            // Castle
            if (move.To.X - move.From.X == 2)
            {
                Board.PieceBoard[move.To.X - 1, move.To.Y] = Board.PieceBoard[move.To.X + 1, move.To.Y];
                Board.PieceBoard[move.To.X - 1, move.To.Y].Position = new V2(move.To.X - 1, move.To.Y);
                Board.PieceBoard[move.To.X - 1, move.To.Y].OnMove(move);
                Board.PieceBoard[move.To.X + 1, move.To.Y] = null;
            }
            else if (move.To.X - move.From.X == -2)
            {
                Board.PieceBoard[move.To.X + 1, move.To.Y] = Board.PieceBoard[move.To.X - 2, move.To.Y];
                Board.PieceBoard[move.To.X + 1, move.To.Y].Position = new V2(move.To.X + 1, move.To.Y);
                Board.PieceBoard[move.To.X + 1, move.To.Y].OnMove(move);
                Board.PieceBoard[move.To.X - 2, move.To.Y] = null;
            }
        }

        public override PieceSerialisationData GetData()
        {
            PieceSerialisationData data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();
            data.Data = BitConverter.GetBytes(HasMoved);
            return data;
        }

        public override void LoadData(PieceSerialisationData data)
        {
            Position = data.Position;
            Team = data.Team;
            HasMoved = BitConverter.ToBoolean(data.Data);
        }

        public override AbstractPiece Clone(AbstractBoard newBoard)
        {
            KingPiece k = new KingPiece(Position, Team, newBoard);
            k.HasMoved = HasMoved;
            return k;
        }

        public override int GetUID() => 101;

        public override float GetValue() => 2f;
    }
}