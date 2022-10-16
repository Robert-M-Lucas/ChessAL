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
    public AbstractGameManagerData GameManagerData;
    public AbstractBoard Board;

    public AbstractGameManager(AbstractGameManagerData gameManagerData)
    {
        this.GameManagerData = gameManagerData;
    }

    public abstract void LoadData(byte[] data);

    public abstract void OnForeignMove(MoveData moveData);

    public virtual List<Move> GetMoves()
    {
        return Board.GetMoves();
    }
}