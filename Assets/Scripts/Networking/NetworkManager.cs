using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Networking.Server;
using Networking.Client;

#nullable enable

/// <summary>
/// Provides an interface for MonoBehaviour classes with the Client and Server
/// </summary>
public class NetworkManager : MonoBehaviour
{
    private ChessManager chessManager = default!;

    private Server? server = null;
    private Client? client = null;

    public bool IsHost { get { return server is not null; } }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        chessManager = FindObjectOfType<ChessManager>();
    }

    // Shuts down client and server correctly
    private void OnApplicationQuit() => Stop();

    #region Client encapsulations
    public void OnLocalMove(int nextPlayer, V2 from, V2 to) => client?.OnLocalMove(nextPlayer, from, to);

    /// <summary>
    /// Gets the player list from the client
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public ConcurrentDictionary<int, ClientPlayerData> GetPlayerList()
    {
        Debug.LogWarning(GetHashCode());
        return client!.PlayerData;
    }
    public int GetLocalPlayerID()
    {
        Debug.LogWarning(GetHashCode());
        return client!.PlayerID;
    }
    #endregion

    /// <summary>
    /// Hosts a game
    /// </summary>
    public void Host(HostSettings settings, Action onPlayersChange, Action onGameStart)
    {
        ServerGameData gameData = new ServerGameData(settings.GameMode, settings.SaveData);

        server = new Server(gameData, settings.Password);
        server.Start();

        client = new Client("127.0.0.1", settings.Password, settings.PlayerName, this, OnHostSuccessOrFail);
        client.Connect();
    }

    /// <summary>
    /// Starts a game
    /// </summary>
    /// <returns>Null if successful or a string error</returns>
    public string? HostStartGame()
    {
        return server?.StartGame();
    }

    /// <summary>
    /// Joins a game
    /// </summary>
    /// <param name="settings"></param>
    public void Join(JoinSettings settings)
    {
        client = new Client(settings.IP, settings.Password, settings.PlayerName, this, OnJoinSuccessOrFail);
        client.Connect();

        Debug.Log(client);
    }

    /// <summary>
    /// Called on connection failure to correctly shut down server and client
    /// </summary>
    private void ConnectionFailed() => Stop();

    public void GetPing(Action<int> callback)
    {
        if (client is null) return;
        client.GetPing(callback);
    }

    public void Stop()
    {
        Debug.LogWarning("Stop");
        server?.Shutdown();
        server = null;
        client?.Shutdown();
        client = null;
        Debug.Log(client);
        Debug.LogWarning("Stop finished");
    }

    // Encapsulated methods the client calls
    #region Client Callbacks
    public void OnHostSuccessOrFail(string? status)
    {
        if (status is not null)
        {
            chessManager.HostFailed(status);
            ConnectionFailed();
            return;
        }

        chessManager.HostSucceed();
        OnPlayersChange();
    }
    public void OnJoinSuccessOrFail(string? status)
    {
        if (status is not null)
        {
            chessManager.JoinFailed(status);
            ConnectionFailed();
            return;
        }

        chessManager.JoinSucceed();
        OnPlayersChange();
    }
    public void OnClientKick(string reason) => chessManager.JoinFailed(reason);
    public void OnPlayersChange() => chessManager.PlayerListUpdate();
    public void OnGamemodeRecieve(int gameMode, byte[] saveData) => chessManager.GameDataRecived(gameMode, saveData);
    public void OnGameStart() => chessManager.OnGameStart();
    public void OnForeignMove(int nextPlayer, V2 from, V2 to) => chessManager.OnForeignMoveUpdate(nextPlayer, from, to);
    public void HostSetTeam(int playerID, int team, int playerInTeam) => server?.SetTeam(playerID, team, playerInTeam);
    #endregion
}