using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class LiveGameData
{
    private ChessManager chessManager;

    public int LocalPlayerTeam;

    public int LocalPlayerID;

    public int CurrentPlayer;

    public LiveGameData(ChessManager chessManager) { this.chessManager = chessManager; }

    public LiveGameData Clone()
    {
        LiveGameData gameData = new LiveGameData(chessManager);
        gameData.LocalPlayerTeam = LocalPlayerTeam;
        gameData.LocalPlayerID = LocalPlayerID;
        gameData.CurrentPlayer = CurrentPlayer;
        return gameData;
    }

    public int GetPlayerByTeam(int team, int playerInTeam) => chessManager.GetPlayerByTeam(team, playerInTeam);

    public ConcurrentDictionary<int, Networking.Client.ClientPlayerData> GetPlayerList() => chessManager.GetPlayerList();
}
