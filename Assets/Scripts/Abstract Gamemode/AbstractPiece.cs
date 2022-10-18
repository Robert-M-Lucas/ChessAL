using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes
{
    /// <summary>
    /// A game piece
    /// </summary>
    public abstract class AbstractPiece
    {
        public V2 Position;
        /// <summary>
        /// ID corresponding to sprites in the VisualManager
        /// </summary>
        public int AppearanceID;

        public AbstractPiece(V2 position)
        {
            Position = position;
        }

        public virtual List<Move> GetMoves() { return new List<Move>(); }

        public abstract int GetUID();

        public virtual void OnMove(V2 from, V2 to) { }
    }
}