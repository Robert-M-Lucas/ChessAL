using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Gamemodes.Checkers
{
    /// <summary>
    /// Sample Board
    /// </summary>
    public class Board : AbstractBoard
    {
        public Board(AbstractGameManager gameManager) : base(gameManager)
        {
            InitialiseBoard();
        }

        protected void InitialiseBoard()
        {
            PieceBoard = new AbstractPiece[8, 8];

            for (int i = 0; i < 8; i += 2) PieceBoard[i, 0] = new CheckersPiece(new V2(i, 0), 0, this);
            for (int i = 1; i < 8; i += 2) PieceBoard[i, 1] = new CheckersPiece(new V2(i, 1), 0, this);
            for (int i = 0; i < 8; i += 2) PieceBoard[i, 2] = new CheckersPiece(new V2(i, 2), 0, this);

            for (int i = 1; i < 8; i += 2) PieceBoard[i, 5] = new CheckersPiece(new V2(i, 5), 1, this);
            for (int i = 0; i < 8; i += 2) PieceBoard[i, 6] = new CheckersPiece(new V2(i, 6), 1, this);
            for (int i = 1; i < 8; i += 2) PieceBoard[i, 7] = new CheckersPiece(new V2(i, 7), 1, this);
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            return new BoardRenderInfo(8, new List<V2>());
        }
    }
}