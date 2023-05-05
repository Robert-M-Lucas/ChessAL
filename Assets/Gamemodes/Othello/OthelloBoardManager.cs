using Game;
using System.Collections.Generic;

namespace Gamemodes.Othello
{
    /// <summary>
    /// Othello Board
    /// </summary>
    public class Board : AbstractBoard
    {
        private bool smallBoard = false;

        public Board(AbstractGameManager gameManager, LiveGameData initialData, bool initialise = true) : base(gameManager)
        {
            if (initialise) InitialiseBoard(initialData);
        }

        protected void InitialiseBoard(LiveGameData initialData)
        {
            PieceBoard = new AbstractPiece[8, 8];

            if (initialData.GetPlayerList().Count == 2)
            {
                PieceBoard[3, 3] = new OthelloPiece(new V2(3, 3), 0, this);
                PieceBoard[4, 3] = new OthelloPiece(new V2(4, 3), 1, this);
                PieceBoard[3, 4] = new OthelloPiece(new V2(3, 4), 1, this);
                PieceBoard[4, 4] = new OthelloPiece(new V2(4, 4), 0, this);
            }
            else if (initialData.GetPlayerList().Count == 3 && initialData.GetPlayerByTeam(2, 0) != null)
            {
                smallBoard = true;
                
                PieceBoard[2, 2] = new OthelloPiece(new V2(2, 2), 1, this);
                PieceBoard[2, 3] = new OthelloPiece(new V2(2, 3), 1, this);
                PieceBoard[2, 4] = new OthelloPiece(new V2(2, 4), 1, this);

                PieceBoard[4, 2] = new OthelloPiece(new V2(4, 2), 2, this);
                PieceBoard[4, 3] = new OthelloPiece(new V2(4, 3), 2, this);
                PieceBoard[4, 4] = new OthelloPiece(new V2(4, 4), 2, this);

                PieceBoard[3, 2] = new OthelloPiece(new V2(3, 2), 0, this);
                PieceBoard[3, 3] = new OthelloPiece(new V2(3, 3), 0, this);
                PieceBoard[3, 4] = new OthelloPiece(new V2(3, 4), 0, this);
            }
            else if (initialData.GetPlayerList().Count == 3 && initialData.GetPlayerByTeam(3, 0) != null)
            {
                smallBoard = true;

                PieceBoard[2, 2] = new OthelloPiece(new V2(2, 2), 1, this);
                PieceBoard[2, 3] = new OthelloPiece(new V2(2, 3), 1, this);
                PieceBoard[2, 4] = new OthelloPiece(new V2(2, 4), 1, this);

                PieceBoard[4, 2] = new OthelloPiece(new V2(4, 2), 3, this);
                PieceBoard[4, 3] = new OthelloPiece(new V2(4, 3), 3, this);
                PieceBoard[4, 4] = new OthelloPiece(new V2(4, 4), 3, this);

                PieceBoard[3, 2] = new OthelloPiece(new V2(3, 2), 0, this);
                PieceBoard[3, 3] = new OthelloPiece(new V2(3, 3), 0, this);
                PieceBoard[3, 4] = new OthelloPiece(new V2(3, 4), 0, this);
            }
            else
            {
                PieceBoard[3, 3] = new OthelloPiece(new V2(3, 3), 0, this);
                PieceBoard[4, 3] = new OthelloPiece(new V2(4, 3), 1, this);
                PieceBoard[3, 4] = new OthelloPiece(new V2(3, 4), 2, this);
                PieceBoard[4, 4] = new OthelloPiece(new V2(4, 4), 3, this);

                PieceBoard[1, 2] = new OthelloPiece(new V2(1, 2), 0, this);
                PieceBoard[5, 1] = new OthelloPiece(new V2(5, 1), 1, this);
                PieceBoard[2, 6] = new OthelloPiece(new V2(2, 6), 2, this);
                PieceBoard[6, 5] = new OthelloPiece(new V2(6, 5), 3, this);
            }
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            if (smallBoard)
            {
                return new BoardRenderInfo(7, new List<V2>());
            }
            return new BoardRenderInfo(8, new List<V2>());
        }

        public override AbstractBoard Clone(AbstractGameManager newGameManager)
        {
            var board = new Board(newGameManager, null, false);
            var piece_board = new AbstractPiece[PieceBoard.Length, PieceBoard.Length];
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