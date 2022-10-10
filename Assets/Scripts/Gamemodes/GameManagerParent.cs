using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameManagerParent
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Unique UID of this GameMode</returns>
    public abstract int GetUID();

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Minimum players to start game</returns>
    public abstract int GetMinPlayers();
    public abstract int GetMaxPlayers();

    /// <summary>
    /// 
    /// </summary>
    /// <returns>{ Team1Size, Team2Size, Team3Size, ... }</returns>
    public abstract int[] TeamSizes();
}
