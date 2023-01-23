using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Gamemodes.Checkers
{
    /// <summary>
    /// Checkers Board
    /// </summary>
    public class Board : AbstractBoard
    {
        public Board(AbstractGameManager gameManager, bool initialise = true) : base(gameManager)
        {
            if(initialise) InitialiseBoard();
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
            return new BoardRenderInfo(8, new List<V2>(), null, true);
        }

        public override AbstractBoard Clone(AbstractGameManager newGameManager)
        {
            Board board = new Board(newGameManager, false);
            AbstractPiece[,] pieceBoard = new AbstractPiece[8, 8];
            for (int x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < PieceBoard.GetLength(0); y++)
                {
                    if (PieceBoard[x, y] is not null) pieceBoard[x, y] = PieceBoard[x, y].Clone(board);
                }
            }

            board.PieceBoard = pieceBoard;

            return board;
        }
    }
}