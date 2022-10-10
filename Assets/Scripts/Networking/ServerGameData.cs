using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

public struct TeamSize
{
    int Min;
    int Max;

    public TeamSize(int min, int max)
    {
        Min = min;
        Max = max;
    }
}

#nullable enable
/// <summary>
/// Holds data about a server
/// </summary>
public class ServerGameData
{
    public int GameModeID;

    public TeamSize[] TeamSizes;

    public byte[] SaveData;

    public ServerGameData(GameManagerParent gameManager, string? savePath)
    {
        if (savePath is not null) SaveData = File.ReadAllBytes(savePath);
        else SaveData = new byte[0];

        GameModeID = gameManager.GetUID();

        TeamSizes = gameManager.TeamSizes();
    }
}
