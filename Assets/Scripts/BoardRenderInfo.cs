using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual board settings provided by board manager
/// </summary>
public struct BoardRenderInfo
{
    public V2 BoardSize;
    public List<V2> RemovedSquares;

    public BoardRenderInfo(V2 boardSize, List<V2> removedSquares = null)
    {
        BoardSize = boardSize;
        RemovedSquares = removedSquares ?? new List<V2>();
    }
}
