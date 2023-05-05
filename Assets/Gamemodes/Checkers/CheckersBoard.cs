using System.Collections.Generic;

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

            for (var i = 0; i < 8; i += 2) PieceBoard[i, 0] = new CheckersPiece(new V2(i, 0), 0, this);
            for (var i = 1; i < 8; i += 2) PieceBoard[i, 1] = new CheckersPiece(new V2(i, 1), 0, this);
            for (var i = 0; i < 8; i += 2) PieceBoard[i, 2] = new CheckersPiece(new V2(i, 2), 0, this);

            for (var i = 1; i < 8; i += 2) PieceBoard[i, 5] = new CheckersPiece(new V2(i, 5), 1, this);
            for (var i = 0; i < 8; i += 2) PieceBoard[i, 6] = new CheckersPiece(new V2(i, 6), 1, this);
            for (var i = 1; i < 8; i += 2) PieceBoard[i, 7] = new CheckersPiece(new V2(i, 7), 1, this);
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            return new BoardRenderInfo(8, new List<V2>(), null, true);
        }

        public override AbstractBoard Clone(AbstractGameManager newGameManager)
        {
            var board = new Board(newGameManager, false);
            var piece_board = new AbstractPiece[8, 8];
            for (var x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (var y = 0; y < PieceBoard.GetLength(0); y++)
                {
                    if (PieceBoard[x, y] is not null) piece_board[x, y] = PieceBoard[x, y].Clone(board);
                }
            }

            board.PieceBoard = piece_board;

            return board;
        }
    }
}