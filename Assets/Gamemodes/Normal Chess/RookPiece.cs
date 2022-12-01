using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
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

        
        public override void OnMove(Move move, bool thisPiece)
        {
            if (!thisPiece) return;
            HasMoved = true;
        }

        public override AbstractPiece Clone(AbstractBoard newBoard)
        {
            RookPiece r = new RookPiece(Position, Team, newBoard);
            r.HasMoved = HasMoved;
            return r;
        }

        public override PieceSerialisationData GetData()
        {
            PieceSerialisationData data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();
            data.Data = BitConverter.GetBytes(HasMoved);
            return data;
        }

        public override void LoadData(PieceSerialisationData data)
        {
            Position = data.Position;
            Team = data.Team;
            HasMoved = BitConverter.ToBoolean(data.Data);
        }

        public override int GetUID() => 104;

        public override float GetValue() => 5f;
    }
}