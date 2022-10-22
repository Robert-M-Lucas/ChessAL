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
            InitialiseBoard();
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            return new BoardRenderInfo(8, new List<V2>() { new V2(3, 3), new V2(4, 3), new V2(3, 4), new V2(4, 4), });
        }
    }

}
