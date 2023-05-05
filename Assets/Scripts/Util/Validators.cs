using Networking.Server;
using Networking.Client;
using System.Collections.Generic;
using System.IO;

#nullable enable
public static class Validators
{
    /// <summary>
    /// Validates a player's name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Null if successful or a string error</returns>
    public static string? ValidatePlayerName(string name)
    {
        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890_";

        // if (Util.RemoveInvisibleChars(name) != name) return "Contains invisible characters"; // Invisible characters

        if (name.Length > 16) return "Name must be shorter than 17 characters"; // Too long

        if (name.Length < 4) return "Name must be longer than 3 characters"; // Too short

        foreach (var c in name)
        {
            if (!allowedChars.Contains(c))
            {
                if (c == ' ') return $"Character '{c}' (space) not allowed";
                return $"Character '{c}' not allowed";
            }
        }

        return null;
    }

    /// <summary>
    /// Validates a game password
    /// </summary>
    /// <param name="password"></param>
    /// <returns>Null if successful or a string error</returns>
    public static string? ValidatePassword(string password)
    {
        if (password == string.Empty) return null;

        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890_!@#$*&;:[]{}?.,<>~-=+|";

        // if (Util.RemoveInvisibleChars(name) != name) return "Contains invisible characters"; // Invisible characters

        if (password.Length > 16) return "Password must be shorter than 17 characters"; // Too long

        if (password.Length < 4) return "Password must be longer than 3 characters"; // Too short

        foreach (var c in password)
        {
            if (!allowedChars.Contains(c))
            {
                if (c == ' ') return $"Character '{c}' (space) not allowed";
                return $"Character '{c}' not allowed";
            }
        }

        return null;
    }

    /// <summary>
    /// Validates a file name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string? ValidateFileName(string fileName)
    {
        if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return $"File name contains illegal character '{fileName[fileName.IndexOfAny(Path.GetInvalidFileNameChars())]}'";
        }

        if (fileName.Length > 16) return "File name must be shorter than 17 character";
        if (fileName.Length < 4) return "File name must be longer than 3 character";

        return null;
    }

    /// <summary>
    /// Ensures that the team compositions are correct for the game to start
    /// </summary>
    /// <returns>Null if successful or a string error</returns>
    public static string? ValidateTeams(List<ServerPlayerData> playerData, ServerGameData gameData)
    {

        var max_team = 0;
        var team_dict = new Dictionary<int, int>();
        Dictionary<int, List<int>> players_in_teams = new Dictionary<int, List<int>>();
        foreach (var player in playerData)
        {
            if (player.Team > max_team) max_team = player.Team;

            if (!team_dict.ContainsKey(player.Team))
            {
                players_in_teams.Add(player.Team, new List<int>() { player.PlayerInTeam });
                team_dict.Add(player.Team, 1);
            }
            else
            {
                team_dict[player.Team]++;
                if (players_in_teams[player.Team].Contains(player.PlayerInTeam) && player.Team != -1) return "Duplicate player in team"; // Duplicate player in team (not applicable to spectators)
                players_in_teams[player.Team].Add(player.PlayerInTeam);
            }
        }

        if (max_team != gameData.TeamSizes.Length - 1) return "Wrong number of teams"; // Wrong number of teams

        for (var i = 0; i < max_team; i++)
        {
            if (!team_dict.ContainsKey(i) && gameData.TeamSizes[i].Min > 0) return "Team missing"; // Team missing

            if (team_dict[i] < gameData.TeamSizes[i].Min) return "Team too small"; // Team too small
            if (team_dict[i] > gameData.TeamSizes[i].Max) return "Team too big"; // Team too big
        }

        return null;
    }

    /// <summary>
    /// Ensures that the team compositions are correct for the game to start
    /// </summary>
    /// <returns>Null if successful or a string error</returns>
    public static string? ValidateTeams(List<ClientPlayerData> playerData, HostSettings gameData)
    {
        var max_team = 0;

        var team_dict = new Dictionary<int, int>();
        Dictionary<int, List<int>> players_in_teams = new Dictionary<int, List<int>>();
        foreach (var player in playerData)
        {
            if (player.Team > max_team) max_team = player.Team;

            if (!team_dict.ContainsKey(player.Team))
            {
                players_in_teams.Add(player.Team, new List<int>() { player.PlayerInTeam });
                team_dict.Add(player.Team, 1);
            }
            else
            {
                team_dict[player.Team]++;
                if (players_in_teams[player.Team].Contains(player.PlayerInTeam) && player.Team != -1) return "Duplicate player in team"; // Duplicate player in team (not applicable to spectators)
                players_in_teams[player.Team].Add(player.PlayerInTeam);
            }
        }

        var team_sizes = gameData.GameMode.GetTeamSizes();

        // if (max_team != teamSizes.Length - 1) return "Wrong number of teams"; // Wrong number of teams

        for (var i = 0; i < max_team; i++)
        {
            if (!team_dict.ContainsKey(i) && team_sizes[i].Min > 0) return "Team missing"; // Team missing
            if (!team_dict.ContainsKey(i) && team_sizes[i].Min == 0) continue;

            if (team_dict[i] < team_sizes[i].Min)
            {
                if (team_sizes[i].Min == 0) return "Wrong number of teams";
                return "Team too small"; // Team too small
            }
            if (team_dict[i] > team_sizes[i].Max) return "Team too big"; // Team too big
        }

        return null;
    }
}
