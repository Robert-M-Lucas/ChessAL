using Gamemodes.NormalChess;
using System.Collections.Generic;

namespace Gamemodes.MegaChess
{
    public class Board : NormalChess.Board
    {
        public Board(AbstractGameManager gameManager) : base(gameManager)
        {
            MegaInitialiseBoard();
        }

        public void MegaInitialiseBoard()
        {
            PieceBoard = new AbstractPiece[16, 16];

            PieceBoard[6, 0] = new QueenPiece(new V2(6, 0), 0, this);
            PieceBoard[7, 0] = new QueenPiece(new V2(7, 0), 0, this);
            PieceBoard[9, 0] = new QueenPiece(new V2(9, 0), 0, this);
            PieceBoard[8, 0] = new KingPiece(new V2(8, 0), 0, this);
            for (var i = 0; i < 16; i++) PieceBoard[i, 2] = new PawnPiece(new V2(i, 2), 0, this);
            for(var i = 1; i < 7; i++) PieceBoard[i, 1] = new KnightPiece(new V2(i, 1), 0, this);
            for(var i = 9; i < 16; i++) PieceBoard[i, 1] = new KnightPiece(new V2(i, 1), 0, this);
            PieceBoard[0, 1] =  new RookPiece(new V2(0, 1), 0, this);
            PieceBoard[15, 1] = new RookPiece(new V2(15, 1), 0, this);
            PieceBoard[7, 1] =  new RookPiece(new V2(7, 1), 0, this);
            PieceBoard[8, 1] =  new RookPiece(new V2(8, 1), 0, this);
            PieceBoard[0, 0] =  new RookPiece(new V2(0, 0), 0, this);
            PieceBoard[15, 0] = new RookPiece(new V2(15, 0), 0, this);
            PieceBoard[1, 0] =  new RookPiece(new V2(1, 0), 0, this);
            PieceBoard[14, 0] = new RookPiece(new V2(14, 0), 0, this);
            for (var i = 2; i < 7; i++) PieceBoard[i, 0] = new BishopPiece(new V2(i, 0), 0, this);
            for (var i = 9; i < 14; i++) PieceBoard[i, 0] = new BishopPiece(new V2(i, 0), 0, this);

            PieceBoard[6, 15] = new QueenPiece(new V2(6, 15), 1, this);
            PieceBoard[7, 15] = new QueenPiece(new V2(7, 15), 1, this);
            PieceBoard[9, 15] = new QueenPiece(new V2(9, 15), 1, this);
            PieceBoard[8, 15] = new KingPiece(new V2(8, 15), 1, this);
            for (var i = 0; i < 16; i++) PieceBoard[i, 13] = new PawnPiece(new V2(i, 13), 1, this);
            for (var i = 1; i < 7; i++) PieceBoard[i, 14] = new KnightPiece(new V2(i, 14), 1, this);
            for (var i = 9; i < 16; i++) PieceBoard[i, 14] = new KnightPiece(new V2(i, 14), 1, this);
            PieceBoard[0, 14] = new RookPiece(new V2(0, 14), 1, this);
            PieceBoard[15, 14] = new RookPiece(new V2(15, 14), 1, this);
            PieceBoard[7, 14] = new RookPiece(new V2(7, 14), 1, this);
            PieceBoard[8, 14] = new RookPiece(new V2(8, 14), 1, this);
            PieceBoard[0, 15] = new RookPiece(new V2(0, 15), 1, this);
            PieceBoard[15, 15] = new RookPiece(new V2(15, 15), 1, this);
            PieceBoard[1, 15] = new RookPiece(new V2(1, 15), 1, this);
            PieceBoard[14, 15] = new RookPiece(new V2(14, 15), 1, this);
            for (var i = 2; i < 7; i++) PieceBoard[i, 15] = new BishopPiece(new V2(i, 15), 1, this);
            for (var i = 9; i < 14; i++) PieceBoard[i, 15] = new BishopPiece(new V2(i, 15), 1, this);
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            return new BoardRenderInfo(16, new List<V2>(), null, true);
        }
    }

}
