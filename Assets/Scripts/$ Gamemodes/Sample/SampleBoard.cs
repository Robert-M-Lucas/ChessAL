using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.Sample
{
    /// <summary>
    /// Sample Board
    /// </summary>
    public class SampleBoard : AbstractBoard
    {
        public SampleBoard()
        {
            PieceBoard = new AbstractPiece[8, 8];
            PieceBoard[1, 1] = new SamplePiece(new V2(1, 1));
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            return new BoardRenderInfo(8, new List<V2>() { new V2(3, 3), new V2(4, 3), new V2(3, 4), new V2(4, 4), });
        }
    }
}