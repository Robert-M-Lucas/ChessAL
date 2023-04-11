using System.Collections.Generic;

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

        /// <summary>
        /// Returns a list of possible moves for this piece
        /// </summary>
        /// <returns></returns>
        public virtual List<Move> GetMoves() { return new List<Move>(); }

        /// <summary>
        /// Returns the piece's UID
        /// </summary>
        /// <returns></returns>
        public abstract int GetUID();

        /// <summary>
        /// Returns the piece's serialised data
        /// </summary>
        /// <returns></returns>
        public virtual PieceSerialisationData GetData()
        {
            var data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();
            return data;
        }

        /// <summary>
        /// Loads serialised data into the piece
        /// </summary>
        /// <param name="data"></param>
        public virtual void LoadData(PieceSerialisationData data)
        {
            Position = data.Position;
            Team = data.Team;
        }

        /// <summary>
        /// Applies a move to the piece
        /// </summary>
        /// <param name="move"></param>
        /// <param name="thisPiece"></param>
        public virtual void OnMove(Move move, bool thisPiece) { }

        /// <summary>
        /// Returns the piece's value to its team
        /// </summary>
        /// <returns></returns>
        public virtual float GetValue() => 1f;

        /// <summary>
        /// Returns a clone of the piece
        /// </summary>
        /// <param name="newBoard"></param>
        /// <returns></returns>
        public abstract AbstractPiece Clone(AbstractBoard newBoard);
    }
}