using System.IO;

/// <summary>
/// Holds minimum and maximum players per team
/// </summary>
public struct TeamSize
{
    public int Min;
    public int Max;

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

    public ServerGameData(AbstractGameManagerData gameManager, string? savePath)
    {
        if (savePath is not null) SaveData = File.ReadAllBytes(savePath);
        else SaveData = new byte[0];

        GameModeID = gameManager.GetUID();

        TeamSizes = gameManager.GetTeamSizes();
    }
}