using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

#nullable enable
public class ServerGameData
{
    public int GameModeID;

    public int MaxPlayers;
    public int MinPlayers;

    public int[] TeamSizes;

    public byte[] SaveData;

    public ServerGameData(GameManagerParent gameManager, string? savePath)
    {
        if (savePath is not null) SaveData = File.ReadAllBytes(savePath);
        else SaveData = new byte[0];

        GameModeID = gameManager.GetUID();
        MinPlayers = gameManager.GetMinPlayers();
        MaxPlayers = gameManager.GetMaxPlayers();

        TeamSizes = gameManager.TeamSizes();
    }
}
