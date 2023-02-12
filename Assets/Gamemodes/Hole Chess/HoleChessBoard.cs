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
            Initialise(); // NormalChess.Board.Initialise (place initial pieces)
        }

        // Setup board with 2x2 hole
        public override BoardRenderInfo GetBoardRenderInfo() => new BoardRenderInfo(8, new List<V2>() { new V2(3, 3), new V2(4, 3), new V2(3, 4), new V2(4, 4) }, null, true);
    }

}
