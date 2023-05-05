using System.Collections.Generic;
using System.Linq;
using System;

namespace Gamemodes.NormalChess
{
    public class RookPiece : NormalChessPiece
    {
        /// <summary>
        /// Stores whether piece has moved. Used for castling.
        /// </summary>
        public bool HasMoved;

        public RookPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 104;
            if (team != 0) AppearanceID += 6;
        }
        
        public override List<Move> GetMoves()
        {
            var moves = new List<Move>();

            var directions = new V2[] { new V2(1, 0), new V2(-1, 0), new V2(0, -1), new V2(0, 1) };

            return directions.Aggregate(moves, (current, direction) 
                => current.Concat(GUtil.RaycastMoves(this, direction, Board)).ToList());
        }

        
        public override void OnMove(Move move, bool thisPiece)
        {
            if (!thisPiece) return;
            HasMoved = true;
        }

        public override AbstractPiece Clone(AbstractBoard newBoard)
        {
            var r = new RookPiece(Position, Team, newBoard);
            r.HasMoved = HasMoved;
            return r;
        }

        public override PieceSerialisationData GetData()
        {
            var data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();
            data.Data = BitConverter.GetBytes(HasMoved); // Add HasMoved to normal save data
            return data;
        }

        public override void LoadData(PieceSerialisationData data)
        {
            Position = data.Position;
            Team = data.Team;
            HasMoved = BitConverter.ToBoolean(data.Data); // Load value of HasMoved from save data
        }

        public override int GetUID() => 104;

        public override float GetValue() => 5f;
    }
}