using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes
{
    /// <summary>
    /// A game piece
    /// </summary>
    public abstract class AbstractPiece
    {
        public V2 Position;
        /// <summary>
        /// ID corresponding to sprites in the VisualManager
        /// </summary>
        public int AppearanceID;

        public int Team;

        public AbstractBoard Board;

        public AbstractPiece(V2 position, int team, AbstractBoard board)
        {
            Position = position;
            Team = team;
            Board = board;
        }

        public virtual List<Move> GetMoves() { return new List<Move>(); }

        public abstract int GetUID();

        public virtual PieceSerialisationData GetData()
        {
            PieceSerialisationData data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();
            return data;
        }

        public virtual void LoadData(PieceSerialisationData data)
        {
            Position = data.Position;
            Team = data.Team;
        }

        public virtual void OnMove(Move move) { }

        public virtual float GetValue() => 1f;
    }
}