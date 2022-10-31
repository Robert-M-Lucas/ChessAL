using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public static class HelpSystem
{
    public static void OpenHelp()
    {
        Process.Start(Application.dataPath + "/StreamingAssets/Help/index.html");
    }

    public static void OpenHelp(int gamemodeID)
    {
        Process.Start(Application.dataPath + "/StreamingAssets/Help/" + gamemodeID + ".html");
    }
}
