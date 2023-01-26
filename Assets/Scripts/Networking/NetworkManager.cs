using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Networking.Server;
using Networking.Client;
using System.Net.NetworkInformation;
using System.Net;
using Networking;

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

    // Runs once
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
    public ConcurrentDictionary<int, ClientPlayerData> GetPlayerList() => client!.PlayerData;

    public int GetLocalPlayerID() => client!.PlayerID;
    #endregion

    /// <summary>
    /// Hosts a game
    /// </summary>
    public bool Host(HostSettings settings, Action onPlayersChange, Action onGameStart)
    {
        if (NetworkingUtils.PortInUse(NetworkSettings.PORT)) return false; // Check if port is in use

        ServerGameData gameData = new ServerGameData(settings.GameMode, settings.SaveData);

        // Start server
        server = new Server(gameData, settings.Password);
        server.Start();

        // Start local client
        client = new Client("127.0.0.1", settings.Password, settings.PlayerName, this, OnHostSuccessOrFail);
        client.Connect();
        return true;
    }

    /// <summary>
    /// Starts a game
    /// </summary>
    /// <returns>Null if successful or a string error</returns>
    public string? HostStartGame() => server?.StartGame();

    /// <summary>
    /// Joins a game
    /// </summary>
    /// <param name="settings"></param>
    public void Join(JoinSettings settings)
    {
        client = new Client(settings.IP, settings.Password, settings.PlayerName, this, OnJoinSuccessOrFail);
        client.Connect();
    }

    /// <summary>
    /// Called on connection failure to correctly shut down server and client
    /// </summary>
    private void ConnectionFailed() => Stop();

    /// <summary>
    /// Calls the callback with the ping as the parameter
    /// </summary>
    /// <param name="callback"></param>
    public void GetPing(Action<int> callback) => client?.GetPing(callback);

    /// <summary>
    /// Shuts down networking
    /// </summary>
    public void Stop()
    {
        Debug.LogWarning("Network Stop");
        server?.Shutdown();
        server = null;
        client?.Shutdown();
        client = null;
        Debug.LogWarning("Network Stop finished");
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
    public int FindNextNonFullTeam(int currentTeam, TeamSize[] teamSizes) => server!.FindNextNonFullTeam(currentTeam, teamSizes);
    #endregion
}