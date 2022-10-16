using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        const string allowed_chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890_";

        // if (Util.RemoveInvisibleChars(name) != name) return "Contains invisible characters"; // Invisible characters

        if (name.Length > 16) return "Name must be shorter than 17 characters"; // Too long

        if (name.Length < 4) return "Name must be longer than 3 characters"; // Too short

        foreach (char c in name)
        {
            if (!allowed_chars.Contains(c))
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

        const string allowed_chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890_!@#$*&;:[]{}?.,<>~-=+|";

        // if (Util.RemoveInvisibleChars(name) != name) return "Contains invisible characters"; // Invisible characters

        if (password.Length > 16) return "Password must be shorter than 17 characters"; // Too long

        if (password.Length < 4) return "Password must be longer than 3 characters"; // Too short

        foreach (char c in password)
        {
            if (!allowed_chars.Contains(c))
            {
                if (c == ' ') return $"Character '{c}' (space) not allowed";
                return $"Character '{c}' not allowed";
            }
        }

        return null;
    }
}
