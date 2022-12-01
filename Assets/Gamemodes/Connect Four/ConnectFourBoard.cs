using System.Collections.Generic;

namespace Gamemodes.ConnectFour
{
    /// <summary>
    /// Sample Board
    /// </summary>
    public class Board : AbstractBoard
    {
        public Board(AbstractGameManager gameManager, bool initialise = true) : base(gameManager)
        {
            if (initialise) InitialiseBoard();
        }

        protected void InitialiseBoard()
        {
            PieceBoard = new AbstractPiece[9, 9];
            PieceBoard[8, 1] = new ConnectFourPiece(new V2(8, 1), 0, this);
            PieceBoard[8, 0] = new ConnectFourPiece(new V2(8, 0), 1, this);
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            List<V2> removed_squares = new List<V2>();
            for (int y = 0; y < 9; y++) removed_squares.Add(new V2(7, y));
            for (int y = 2; y < 9; y++) removed_squares.Add(new V2(8, y));

            return new BoardRenderInfo(9, removed_squares, new List<V2> { new V2(8, 1), new V2(8, 0) });
        }

        public override List<Move> GetMoves(LiveGameData gameData)
        {
            // Red
            if (gameData.CurrentTeam == 0) return GetPiece(new V2(8, 1)).GetMoves();
            // Yellow
            else return GetPiece(new V2(8, 0)).GetMoves();
        }

        public override AbstractBoard Clone(AbstractGameManager newGameManager)
        {
            Board new_board = new Board(newGameManager, false);
            AbstractPiece[,] pieceBoard = new AbstractPiece[9, 9];
            for (int x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < PieceBoard.GetLength(0); y++)
                {
                    if (PieceBoard[x, y] is not null) pieceBoard[x, y] = PieceBoard[x, y].Clone(new_board);
                }
            }

            new_board.PieceBoard = pieceBoard;

            return new_board;
        }
    }
}