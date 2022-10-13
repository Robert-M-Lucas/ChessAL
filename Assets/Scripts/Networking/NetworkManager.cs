using System;
using UnityEngine;

#nullable enable

/// <summary>
/// Provides an interface for MonoBehaviour classes with the Client and Server
/// </summary>
public class NetworkManager : MonoBehaviour
{
    private ChessManager chessManager;

    private Server? server = null;
    private Client? client = null;

    private bool IsHost
    { get { return server is not null; } }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        chessManager = FindObjectOfType<ChessManager>();
    }

    private void Start()
    {

    }

    private void Update()
    {
    }

    private void OnApplicationQuit()
    {
        client?.Shutdown();
        server?.Shutdown();
    }

    private void OnHostSuccessOrFail(string? status, AbstractGameManagerData? gameMode)
    {
        Debug.Log(status);
        Debug.Log($"Connection status: {status ?? "Success"}");
        if (status is not null)
        {
            chessManager.HostFailed(status);
            ConnectionFailed();
            return;
        }

        client?.GetPing(OnPing);

        chessManager.HostSucceed();
    }

    private void OnJoinSuccessOrFail(string? status, AbstractGameManagerData? gameMode)
    {
        Debug.Log($"Connection status: {status ?? "Success"}");
        if (status is not null)
        {
            chessManager.JoinFailed(status);
            ConnectionFailed();
            return;
        }

        client?.GetPing(OnPing);

        chessManager.JoinSucceed();
    }

    private void ConnectionFailed()
    {
        server?.Shutdown();
        server = null;
        client?.Shutdown();
        client = null;
    }

    private void OnPing(int ping)
    {
        Debug.Log($"Ping {ping}ms");
    }

    /// <summary>
    /// Hosts a game
    /// </summary>
    public void Host(HostSettings settings)
    {
        ServerGameData gameData = new ServerGameData(settings.GameMode, settings.SavePath);

        server = new Server(gameData, settings.Password);
        server.Start();

        client = new Client("127.0.0.1", settings.Password, settings.PlayerName, OnHostSuccessOrFail, (_) => { }, (_, _) => { });
        client.Connect();
    }

    public void Join(JoinSettings settings, Action<string> onClientKick, Action<int, byte[]> onGamemodeRecieve)
    {
        client = new Client(settings.IP, settings.Password, settings.PlayerName, OnJoinSuccessOrFail, onClientKick, onGamemodeRecieve);
        client.Connect();
    }
}