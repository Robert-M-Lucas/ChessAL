using System.Collections.Generic;

namespace Gamemodes.VikingChess
{
    public class Board : AbstractBoard
    {
        public static readonly V2 CENTRE = new V2(5, 5);

        public Board(AbstractGameManager gameManager, bool initialise = true) : base(gameManager)
        {
            if (initialise) Initialise();
        }

        public void Initialise()
        {
            PieceBoard = new AbstractPiece[11, 11];

            for (var y = 0; y < 11; y += 10)
            {
                for (var x = 3; x < 8; x++)
                {
                    PieceBoard[x, y] = new VikingPiece(new V2(x, y), 0, this);
                }
            }

            for (var x = 0; x < 11; x += 10)
            {
                for (var y = 3; y < 8; y++)
                {
                    PieceBoard[x, y] = new VikingPiece(new V2(x, y), 0, this);
                }
            }

            for (var x = 4; x < 7; x++)
            {
                for (var y = 4; y < 7; y++)
                {
                    if (x == 5 && y == 5) PieceBoard[x, y] = new VikingKing(new V2(x, y), 1, this);
                    else PieceBoard[x, y] = new VikingPiece(new V2(x, y), 1, this);
                }
            }

            PieceBoard[5, 1] = new VikingPiece(new V2(5, 1), 0, this);
            PieceBoard[1, 5] = new VikingPiece(new V2(1, 5), 0, this);
            PieceBoard[9, 5] = new VikingPiece(new V2(9, 5), 0, this);
            PieceBoard[5, 9] = new VikingPiece(new V2(5, 9), 0, this);

            PieceBoard[5, 3] = new VikingPiece(new V2(5, 3), 1, this);
            PieceBoard[3, 5] = new VikingPiece(new V2(3, 5), 1, this);
            PieceBoard[7, 5] = new VikingPiece(new V2(7, 5), 1, this);
            PieceBoard[5, 7] = new VikingPiece(new V2(5, 7), 1, this);
        }

        public override BoardRenderInfo GetBoardRenderInfo() => 
            new BoardRenderInfo(11, null, new List<V2>() { new V2(0, 0), new V2(10, 0), new V2(0, 10), new V2(10, 10), CENTRE }, true);

        public override AbstractBoard Clone(AbstractGameManager newGameManager)
        {
            var board = new Board(newGameManager, false);
            var piece_board = new AbstractPiece[11, 11];
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