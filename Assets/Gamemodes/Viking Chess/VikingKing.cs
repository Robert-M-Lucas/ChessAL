using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.VikingChess
{
    public class VikingKing : VikingPiece
    {
        public VikingKing(V2 position, int team, AbstractBoard board): base(position, team, board)
        {
            AppearanceID = 101;
        }

        public override int GetUID() => 801;
    }

}
