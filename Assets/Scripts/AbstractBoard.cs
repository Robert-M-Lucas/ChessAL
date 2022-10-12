using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the board's configuration and stores pieces
/// </summary>
public abstract class AbstractBoard
{
    public abstract List<Move> GetMoves();

    public abstract BoardRenderInfo GetBoardRenderInfo();
}
