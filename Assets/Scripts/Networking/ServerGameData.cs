using System.IO;

public struct TeamSize
{
    private int Min;
    private int Max;

    public TeamSize(int min, int max)
    {
        Min = min;
        Max = max;
    }
}

#nullable enable

/// <summary>
/// Holds data about a server
/// </summary>
public class ServerGameData
{
    public int GameModeID;

    public TeamSize[] TeamSizes;

    public byte[] SaveData;

    public ServerGameData(GameManagerParent gameManager, string? savePath)
    {
        if (savePath is not null) SaveData = File.ReadAllBytes(savePath);
        else SaveData = new byte[0];

        GameModeID = gameManager.GetUID();

        TeamSizes = gameManager.TeamSizes();
    }
}