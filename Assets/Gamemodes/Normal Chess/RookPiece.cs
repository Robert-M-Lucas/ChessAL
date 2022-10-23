using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class RookPiece : NormalChessPiece
    {
        public bool HasMoved;

        public RookPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 104;
            if (team != 0) AppearanceID += 6;
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

        public override NormalChessPiece Clone(AbstractBoard new_board)
        {
            RookPiece new_piece = new RookPiece(Position, Team, new_board);
            new_piece.HasMoved = HasMoved;
            return new_piece;
        }

        public override int GetUID() => 104;
    }
}