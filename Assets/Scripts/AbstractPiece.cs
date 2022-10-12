using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A game piece
/// </summary>
public class AbstractPiece 
{
    public virtual List<Move> GetMoves() { return new List<Move>(); }
}
