using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.VikingChess
{
    public class Board : AbstractBoard
    {
        public Board(AbstractGameManager gameManager) : base(gameManager)
        {
            PieceBoard = new AbstractPiece[11, 11];

            for (int y = 0; y < 11; y += 10)
            {
                for (int x = 3; x < 8; x++)
                {
                    PieceBoard[x, y] = new VikingPiece(new V2(x, y), 1, this);
                } 
            }

            for (int x = 0; x < 11; x += 10)
            {
                for (int y = 3; y < 8; y++)
                {
                    PieceBoard[x, y] = new VikingPiece(new V2(x, y), 1, this);
                }
            }

            for (int x = 4; x < 7; x ++)
            {
                for (int y = 4; y < 7; y++)
                {
                    if (x == 5 && y == 5) PieceBoard[x, y] = new VikingKing(new V2(x, y), 0, this);
                    else PieceBoard[x, y] = new VikingPiece(new V2(x, y), 0, this);
                }
            }

            PieceBoard[5, 1] = new VikingPiece(new V2(5, 1), 1, this);
            PieceBoard[1, 5] = new VikingPiece(new V2(1, 5), 1, this);
            PieceBoard[10, 5] = new VikingPiece(new V2(10, 5), 1, this);
            PieceBoard[5, 10] = new VikingPiece(new V2(5, 10), 1, this);

            PieceBoard[5, 3] = new VikingPiece(new V2(5, 3), 0, this);
            PieceBoard[3, 5] = new VikingPiece(new V2(3, 5), 0, this);
            PieceBoard[7, 5] = new VikingPiece(new V2(7, 5), 0, this);
            PieceBoard[5, 7] = new VikingPiece(new V2(5, 7), 0, this);
        }

        public override BoardRenderInfo GetBoardRenderInfo() => new BoardRenderInfo(11, null, new List<V2>() { new V2(0, 0), new V2(10, 0), new V2(0, 10), new V2(10, 10) });
    }
}