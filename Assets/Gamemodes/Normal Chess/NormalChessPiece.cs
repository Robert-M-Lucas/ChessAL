using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public abstract class NormalChessPiece : AbstractPiece
    {
        public NormalChessPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {

        }
    }

}
