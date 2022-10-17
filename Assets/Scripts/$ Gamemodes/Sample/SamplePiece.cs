using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.Sample
{
    /// <summary>
    /// Sample Piece
    /// </summary>
    public class SamplePiece : AbstractPiece
    {
        public SamplePiece(V2 position) : base(position)
        {
            AppearanceID = 0;
        }

        public override List<Move> GetMoves()
        {
            return new List<Move>() { new Move(Position.X, Position.Y, Position.X + 1, Position.Y + 1) };
        }

        public override int GetUID() => 1;
    }
}