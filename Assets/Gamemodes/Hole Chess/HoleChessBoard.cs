using Gamemodes.NormalChess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.HoleChess
{
    public class Board : NormalChess.Board
    {
        public Board(AbstractGameManager gameManager) : base(gameManager)
        {
            Initialise();
        }

        /*
        public override NormalChess.Board Clone()
        {
            Board new_board = new Board(GameManager);

            new_board.MoveCounter = MoveCounter;
            new_board.VirtualTeam = VirtualTeam;
            new_board.PieceBoard = new AbstractPiece[PieceBoard.GetLength(0), PieceBoard.GetLength(1)];

            for (int x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < PieceBoard.GetLength(1); y++)
                {
                    if (PieceBoard[x, y] is not null)
                    {
                        new_board.PieceBoard[x, y] = (PieceBoard[x, y] as NormalChessPiece).Clone(new_board);
                    }
                }
            }
            return new_board;
        }
        */

        public override BoardRenderInfo GetBoardRenderInfo() => new BoardRenderInfo(8, new List<V2>() { new V2(3, 3), new V2(4, 3), new V2(3, 4), new V2(4, 4) }, null, true);
    }

}
