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
#if PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Process.Start(Application.dataPath + "/StreamingAssets/Help/index.html");
#endif
    }

    /// <summary>
    /// Opens help page for a specific gamemode
    /// </summary>
    /// <param name="gamemodeID"></param>
    public static void OpenHelp(int gamemodeID)
    {
#if PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Process.Start(Application.dataPath + "/StreamingAssets/Help/" + gamemodeID + ".html");
#endif
    }

    /// <summary>
    /// Opens specified help page
    /// </summary>
    /// <param name="pageName"></param>
    public static void OpenHelp(string pageName)
    {
#if PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Process.Start(Application.dataPath + "/StreamingAssets/Help/" + pageName + ".html");
#endif
    }
}
