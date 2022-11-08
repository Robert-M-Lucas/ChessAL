using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public static class HelpSystem
{
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
}
