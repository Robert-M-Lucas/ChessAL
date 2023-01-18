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

        /// <summary></summary>
        /// <returns>{ Team1Size, Team2Size, Team3Size, ... }</returns>
        public abstract TeamSize[] GetTeamSizes();

        /// <summary>
        /// Returns a list of aliases for teams e.g. { "White", "Black" }
        /// </summary>
        /// <returns></returns>
        public virtual string[] TeamAliases() { return new string[0]; }

        public virtual string GetDescription() => "No description";

        public abstract AbstractGameManager Instantiate();
    }

    /// <summary>
    /// Top level gamemode control
    /// </summary>
    public abstract class AbstractGameManager
    {
        public AbstractGameManagerData GameManagerData;
        public AbstractBoard Board;

        public AbstractGameManager(AbstractGameManagerData gameManagerData)
        {
            this.GameManagerData = gameManagerData;
        }

        /// <summary>
        /// Returns the serialised game
        /// </summary>
        /// <returns></returns>
        public virtual SerialisationData GetData()
        {
            SerialisationData serialisationData = Board.GetData();
            serialisationData.GamemodeUID = GameManagerData.GetUID();
            return serialisationData;
        }

        /// <summary>
        /// Loads serialisation data into the game
        /// </summary>
        /// <param name="data"></param>
        public virtual void LoadData(SerialisationData data)
        {
            Board.LoadData(data);
        }

        /// <summary>
        /// Returns a list of possible moves
        /// </summary>
        /// <returns></returns>
        public virtual List<Move> GetMoves(LiveGameData gameData, bool fastMode)
        {
            return Board.GetMoves(gameData);
        }

        /// <summary>
        /// Pass turn on if no moves are available
        /// </summary>
        /// <returns>Next player's turn / Winning team (negative TeamID - 1)</returns>
        public virtual int OnNoMoves(LiveGameData gameData)
        {
            return GUtil.TurnEncodeTeam(GUtil.SwitchTeam(gameData)); ;
        }

        /// <summary>
        /// Handles an incoming move (local or foreign)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Next player's turn / Winning team (negative TeamID - 1)</returns>
        public virtual int OnMove(Move move, LiveGameData gameData)
        {
            Board.OnMove(move);
            return GUtil.SwitchPlayerTeam(gameData);
        }

        /// <summary>
        /// Gets a score for the current board. Mostly used for AI
        /// </summary>
        /// <param name="gameData"></param>
        /// <returns></returns>
        public virtual float GetScore(LiveGameData gameData)
        {
            return Board.GetScore(gameData);
        }

        /// <summary>
        /// Returns a clone of the game
        /// </summary>
        /// <returns></returns>
        public abstract AbstractGameManager Clone();
    }
}