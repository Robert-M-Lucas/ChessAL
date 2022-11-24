using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public static class HelpSystem
{
    public const string IN_GAME_PAGE_NAME = "in_game";

    /// <summary>
    /// Opens help homepage
    /// </summary>
    public static void OpenHelp()
    {
        Process.Start(Application.dataPath + "/StreamingAssets/Help/index.html");
    }

    /// <summary>
    /// Opens help page for a specific gamemode
    /// </summary>
    /// <param name="gamemodeID"></param>
    public static void OpenHelp(int gamemodeID)
    {
        Process.Start(Application.dataPath + "/StreamingAssets/Help/" + gamemodeID + ".html");
    }

    /// <summary>
    /// Opens specified help page
    /// </summary>
    /// <param name="pageName"></param>
    public static void OpenHelp(string pageName)
    {
        Process.Start(Application.dataPath + "/StreamingAssets/Help/" + pageName + ".html");
    }
}
