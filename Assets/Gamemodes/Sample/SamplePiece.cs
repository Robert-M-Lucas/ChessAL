using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.Sample
{
    /// <summary>
    /// Sample Piece
    /// </summary>
    public class Piece : AbstractPiece
    {
        public Piece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 100;
        }

        public override List<Move> GetMoves()
        {
            return new List<Move>() { 
                new Move(Position, Position + new V2(1, 1)),
                new Move(Position, Position + new V2(-1, 1)),
                new Move(Position, Position + new V2(1, -1)),
                new Move(Position, Position + new V2(-1, -1)),
                new Move(Position, Position + new V2(0, -1)),
                new Move(Position, Position + new V2(-1, 0)),
                new Move(Position, Position + new V2(0, 1)),
                new Move(Position, Position + new V2(1, 0)),
            };
        }

        public override int GetUID() => 1;
    }
}