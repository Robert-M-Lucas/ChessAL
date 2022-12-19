using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual board settings provided by Board Manager
/// </summary>
public struct BoardRenderInfo
{
    public int BoardSize;
    public List<V2> RemovedSquares;
    public List<V2> HighlightedSquares;

    public BoardRenderInfo(int boardSize, List<V2> removedSquares = null, List<V2> highlightedSquares = null)
    {
        BoardSize = boardSize;
        RemovedSquares = removedSquares ?? new List<V2>();
        HighlightedSquares = highlightedSquares ?? new List<V2>();
    }
}
