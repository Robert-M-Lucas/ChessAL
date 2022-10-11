using UnityEngine;

#nullable enable

/// <summary>
/// Provides an interface for MonoBehaviour classes with the Client and Server
/// </summary>
public class NetworkManager : MonoBehaviour
{
    private Server? server = null;
    private Client? client = null;

    private bool IsHost
    { get { return server is not null; } }

    private void Start()
    {
        Host(new SampleGameManager(), "player_name", "Ham", null);
    }

    private void Update()
    {
    }

    private void OnApplicationQuit()
    {
        client?.Shutdown();
        server?.Shutdown();
    }

    private void OnConnect(string? status)
    {
        Debug.Log($"Connection statu: {status ?? "null"}");
        client.GetPing(OnPing);
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
    public void Host(GameManagerParent gameManager, string playerName, string password, string? savePath = null)
    {
        ServerGameData gameData = new ServerGameData(gameManager, savePath);

        server = new Server(gameData, password);
        server.Start();

        client = new Client("127.0.0.1", password, playerName, OnConnect);
        client.Connect();
    }
}