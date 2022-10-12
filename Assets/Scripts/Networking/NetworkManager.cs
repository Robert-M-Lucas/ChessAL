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
        chessManager = GetComponent<ChessManager>();
    }

    private void Start()
    {
        Host(new HostSettings(new SampleGameManagerData(), "Ham", "PlayerName", null));
    }

    private void Update()
    {
    }

    private void OnApplicationQuit()
    {
        client?.Shutdown();
        server?.Shutdown();
    }

    private void OnHostSuccess(string? status)
    {
        Debug.Log($"Connection status: {status ?? "Success"}");
        if (status is null) return;

        client?.GetPing(OnPing);

        chessManager.HostSucceed();
    }

    private void OnPing(int ping)
    {
        Debug.Log($"Ping {ping}ms");
    }

    /// <summary>
    /// Hosts a game
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="savePath">Path to the save file</param>
    public void Host(HostSettings settings)
    {
        ServerGameData gameData = new ServerGameData(settings.GameMode, settings.SavePath);

        server = new Server(gameData, settings.Password);
        server.Start();

        client = new Client("127.0.0.1", settings.Password, settings.PlayerName, OnHostSuccess);
        client.Connect();
    }
}