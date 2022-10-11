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
    }

    private void Update()
    {
    }

    /// <summary>
    /// Hosts a game
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="savePath">Path to the save file</param>
    public void Host(GameManagerParent gameManager, string password, string? savePath = null)
    {
        ServerGameData gameData = new ServerGameData(gameManager, savePath);

        server = new Server(gameData, password);
        server.Start();

        // client = new Client("127.0.0.1");
        // client.Connect();
    }
}