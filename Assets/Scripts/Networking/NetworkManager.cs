using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class NetworkManager : MonoBehaviour
{
    Server? server = null;
    Client? client = null;

    bool IsHost { get { return server is not null; } }

    void Start()
    {
        
    }

    void Update()
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

        client = new Client("127.0.0.1");
        client.Connect();
    }
}
