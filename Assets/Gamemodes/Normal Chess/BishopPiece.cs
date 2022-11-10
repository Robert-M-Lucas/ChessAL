using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class BishopPiece : NormalChessPiece
    {
        public BishopPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 103;
            if (team != 0) AppearanceID += 6;
            
        }
        
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();

            V2[] directions = new V2[] { new V2(1, 1), new V2(-1, 1), new V2(1, -1), new V2(-1, -1) };
            foreach (V2 direction in directions)
            {
                moves = moves.Concat(GUtil.RaycastMoves(this, direction, Board)).ToList();
            }
           
            return moves;
        }

        /*
        public override void OnMove(V2 from, V2 to)
        {

        }
        */

        public override AbstractPiece Clone(AbstractBoard newBoard) => new BishopPiece(Position, Team, newBoard);

        public override int GetUID() => 103;

        public override float GetValue() => 3.2f;
    }
}