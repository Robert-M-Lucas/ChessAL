using System.Collections.Generic;

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

    public abstract AbstractGameManager Instantiate();
}

/// <summary>
/// Top level gamemode control
/// </summary>
public abstract class AbstractGameManager
{
    public AbstractGameManagerData gameManagerData;

    public AbstractGameManager(AbstractGameManagerData gameManagerData)
    {
        this.gameManagerData = gameManagerData;
    }

    public abstract List<Move> GetMoves();
}