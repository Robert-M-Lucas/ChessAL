using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    /// <summary>
    /// Sample Board
    /// </summary>
    public class Board : AbstractBoard
    {
        public Board(AbstractGameManager gameManager) : base(gameManager)
        {
            PieceBoard = new AbstractPiece[8, 8];
            PieceBoard[0, 1] = new PawnPiece(new V2(0, 1), 0, this);
            PieceBoard[1, 1] = new PawnPiece(new V2(1, 1), 0, this);
            PieceBoard[2, 1] = new PawnPiece(new V2(2, 1), 0, this);
            PieceBoard[3, 1] = new PawnPiece(new V2(3, 1), 0, this);
            PieceBoard[4, 1] = new PawnPiece(new V2(4, 1), 0, this);
            PieceBoard[5, 1] = new PawnPiece(new V2(5, 1), 0, this);
            PieceBoard[6, 1] = new PawnPiece(new V2(6, 1), 0, this);
            PieceBoard[7, 1] = new PawnPiece(new V2(7, 1), 0, this);

            PieceBoard[0, 6] = new PawnPiece(new V2(0, 6), 1, this);
            PieceBoard[1, 6] = new PawnPiece(new V2(1, 6), 1, this);
            PieceBoard[2, 6] = new PawnPiece(new V2(2, 6), 1, this);
            PieceBoard[3, 6] = new PawnPiece(new V2(3, 6), 1, this);
            PieceBoard[4, 6] = new PawnPiece(new V2(4, 6), 1, this);
            PieceBoard[5, 6] = new PawnPiece(new V2(5, 6), 1, this);
            PieceBoard[6, 6] = new PawnPiece(new V2(6, 6), 1, this);
            PieceBoard[7, 6] = new PawnPiece(new V2(7, 6), 1, this);

            PieceBoard[4, 0] = new KingPiece(new V2(4, 0), 0, this);
            PieceBoard[4, 7] = new KingPiece(new V2(4, 7), 1, this);
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            return new BoardRenderInfo(8, new List<V2>());
        }
    }
}