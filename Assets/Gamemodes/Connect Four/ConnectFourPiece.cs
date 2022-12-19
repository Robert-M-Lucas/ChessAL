using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.ConnectFour
{
    public class ConnectFourPiece : AbstractPiece
    {
        public ConnectFourPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 900;
            if (team != 0) AppearanceID += 1;
        }

        public override List<Move> GetMoves()
        {
            if (Position.X != 8) return new List<Move>();

            List<Move> moves = new List<Move>();

            for (int x = 0; x < 7; x++)
            {
                if (Board.GetPiece(new V2(x, 8)) is null) moves.Add(new Move(Position, new V2(x, 8)));
            }

            return moves;
        }

        public override void OnMove(Move move, bool thisPiece)
        {
            if (thisPiece)
            {
                // Make piece 'fall'
                for (int y = 8; y >= -1; y--)
                {
                    if (y == -1 || Board.GetPiece(new V2(move.To.X, y)) is not null)
                    {
                        Board.PieceBoard[move.To.X, y + 1] = new ConnectFourPiece(Position, Team, Board);
                        Board.PieceBoard[move.To.X, y + 1].Position = new V2(move.To.X, y + 1);
                        break;
                    }
                }
                
            }

            base.OnMove(move, thisPiece);
        }

        public override int GetUID() => 900;

        public override AbstractPiece Clone(AbstractBoard newBoard) => new ConnectFourPiece(Position, Team, newBoard);

        public override float GetValue() => 0f;
    }
}