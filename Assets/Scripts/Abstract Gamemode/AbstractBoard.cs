using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles the board's configuration and stores pieces
/// </summary>
public abstract class AbstractBoard
{
    public AbstractPiece[,] PieceBoard;

    public virtual List<Move> GetMoves()
    {
        IEnumerable<Move> moves = new List<Move>();
        for (int x = 0; x < PieceBoard.GetLength(0); x++)
        {
            for (int y = 0; y < PieceBoard.GetLength(1); y++)
            {
                if (PieceBoard[x, y] is not null) moves = moves.Concat(PieceBoard[x, y].GetMoves());
            }
        }
        return moves.ToList();
    }

    public abstract BoardRenderInfo GetBoardRenderInfo();
}
