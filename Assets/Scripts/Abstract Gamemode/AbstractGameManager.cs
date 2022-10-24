using System.Collections.Generic;

namespace Gamemodes
{
    /// <summary>
    /// A reduced version of AbstractGameManager for use before a game has been selected
    /// </summary>
    public abstract class AbstractGameManagerData
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>Unique UID of this GameMode</returns>
        public abstract int GetUID();

        /// <summary>
        ///
        /// </summary>
        /// <returns>Non-unique name of this GameMode</returns>
        public abstract string GetName();

        /// <summary>
        ///
        /// </summary>
        /// <returns>{ Team1Size, Team2Size, Team3Size, ... }</returns>
        public abstract TeamSize[] GetTeamSizes();

        public virtual string GetDescription() => "No description";

        public abstract AbstractGameManager Instantiate(ChessManager chessManager);
    }

    /// <summary>
    /// Top level gamemode control
    /// </summary>
    public abstract class AbstractGameManager
    {
        public AbstractGameManagerData GameManagerData;
        public AbstractBoard Board;
        public ChessManager chessManager;

        public AbstractGameManager(AbstractGameManagerData gameManagerData, ChessManager chessManager)
        {
            this.GameManagerData = gameManagerData;
            this.chessManager = chessManager;
        }

        public virtual SerialisationData GetData()
        {
            SerialisationData serialisationData = Board.GetData();
            serialisationData.GamemodeUID = GameManagerData.GetUID();
            return serialisationData;
        }

        public virtual void LoadData(SerialisationData data)
        {
            Board.LoadData(data);
        }

        /// <summary>
        /// Returns a list of possible moves
        /// </summary>
        /// <returns></returns>
        public virtual List<Move> GetMoves()
        {
            return Board.GetMoves();
        }

        /// <summary>
        /// Pass turn on if no moves are available
        /// </summary>
        /// <returns>Next player's turn / Winning team (negative TeamID - 1)</returns>
        public virtual int OnNoMoves()
        {
            return -1;
        }

        /// <summary>
        /// Handles an incoming move (local or foreign)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Next player's turn / Winning team (negative TeamID - 1)</returns>
        public virtual int OnMove(V2 from, V2 to)
        {
            Board.OnMove(from, to);
            return -1;
        }
    }
}