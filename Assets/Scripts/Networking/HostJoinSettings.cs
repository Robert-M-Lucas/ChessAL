using Gamemodes;

#nullable enable
/// <summary>
/// Settings for hosting a game
/// </summary>
public class HostSettings
{
    public AbstractGameManagerData GameMode;
    public string Password;
    public string PlayerName;
    public byte[] SaveData;

    public HostSettings(AbstractGameManagerData gameMode, string password, string playerName, byte[]? saveData = null)
    {
        GameMode = gameMode;
        Password = password;
        PlayerName = playerName;
        SaveData = saveData ?? new byte[0];
    }
}

/// <summary>
/// Settings for joining a game
/// </summary>
public class JoinSettings
{
    public readonly string IP;
    public readonly string Password;
    public readonly string PlayerName;

    public JoinSettings(string ip, string password, string playerName)
    {
        IP = ip;
        Password = password;
        PlayerName = playerName;
    }
}
