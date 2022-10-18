using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public string? SavePath;

    public HostSettings(AbstractGameManagerData gameMode, string password, string playerName, string? savePath)
    {
        GameMode = gameMode;
        Password = password;
        PlayerName = playerName;
        SavePath = savePath;
    }
}

/// <summary>
/// Settings for joining a game
/// </summary>
public class JoinSettings
{
    public string IP;
    public string Password;
    public string PlayerName;

    public JoinSettings(string IP, string password, string playerName)
    {
        this.IP = IP;
        Password = password;
        PlayerName = playerName;
    }
}
