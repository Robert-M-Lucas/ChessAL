using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class RookPiece : AbstractPiece
    {
        public bool HasMoved;

        public RookPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 4;
        }
        
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            V2[] directions = new V2[] { new V2(1, 0), new V2(-1, 0), new V2(0, -1), new V2(0, 1) };
            foreach (V2 direction in directions)
            {
                moves = moves.Concat(GUtil.RaycastMoves(this, direction, Board)).ToList();
            }
           
            return moves;
        }

        
        public override void OnMove(V2 from, V2 to)
        {
            HasMoved = true;
        }
        

        public override int GetUID() => 6;
    }
}