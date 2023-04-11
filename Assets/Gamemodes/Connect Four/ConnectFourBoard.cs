using System.Collections.Generic;
using Game;

namespace Gamemodes.ConnectFour
{
    /// <summary>
    /// Connect Four Board
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
            var removed_squares = new List<V2>();
            for (var y = 0; y < 9; y++) removed_squares.Add(new V2(7, y));
            for (var y = 2; y < 9; y++) removed_squares.Add(new V2(8, y));

            return new BoardRenderInfo(9, removed_squares, new List<V2> { new V2(8, 1), new V2(8, 0) });
        }

        public override List<Move> GetMoves(LiveGameData gameData)
        {
            // Red
            return gameData.CurrentTeam == 0 ? GetPiece(new V2(8, 1)).GetMoves() :
                // Yellow
                GetPiece(new V2(8, 0)).GetMoves();
        }

        public override AbstractBoard Clone(AbstractGameManager newGameManager)
        {
            var new_board = new Board(newGameManager, false);

            var piece_board = new AbstractPiece[9, 9];
            for (var x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (var y = 0; y < PieceBoard.GetLength(1); y++)
                {
                    if (PieceBoard[x, y] is not null) piece_board[x, y] = PieceBoard[x, y].Clone(new_board);
                }
            }

            new_board.PieceBoard = piece_board;

            return new_board;
        }
    }
}