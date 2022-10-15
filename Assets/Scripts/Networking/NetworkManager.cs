using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnApplicationQuit()
    {
        client?.Shutdown();
        server?.Shutdown();
    }

    private void ConnectionFailed()
    {
        server?.Shutdown();
        server = null;
        client?.Shutdown();
        client = null;
    }

    public void OnLocalMove(MoveData moveData) => client?.OnLocalMove(moveData);

    /// <summary>
    /// Gets the player list from the client
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public ConcurrentDictionary<int, ClientPlayerData> GetPlayerList() => client?.PlayerData ?? throw new NullReferenceException();
    public int GetLocalPlayerID() => client!.PlayerID;

    /// <summary>
    /// Hosts a game
    /// </summary>
    public void Host(HostSettings settings, Action onPlayersChange, Action onGameStart)
    {
        ServerGameData gameData = new ServerGameData(settings.GameMode, settings.SavePath);

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
    }

    #region Client Callbacks
    public void OnHostSuccessOrFail(string? status)
    {
        if (status is not null)
        {
            chessManager.HostFailed(status);
            ConnectionFailed();
            return;
        }

        client?.GetPing(OnPing);

        chessManager.HostSucceed();
    }
    public void OnJoinSuccessOrFail(string? status)
    {
        if (status is not null)
        {
            chessManager.JoinFailed(status);
            ConnectionFailed();
            return;
        }

        client?.GetPing(OnPing);

        chessManager.JoinSucceed();
    }
    public void OnClientKick(string reason) => chessManager.JoinFailed(reason);
    public void OnPlayersChange() => chessManager.PlayerListUpdate();
    public void OnGamemodeRecieve(int gameMode, byte[] saveData) => chessManager.GameDataRecived(gameMode, saveData);
    public void OnGameStart() => chessManager.OnGameStart();
    public void OnForeignMove(MoveData moveData) => chessManager.OnForeignMove(moveData);
    private void OnPing(int ping) => Debug.Log($"Ping {ping}ms");
    public void HostSetTeam(int playerID, int team, int playerInTeam) => server?.SetTeam(playerID, team, playerInTeam);
    #endregion
}