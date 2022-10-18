using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Gamemodes;

/// <summary>
/// Utility class
/// </summary>
public static class Util
{
    /// <summary>
    /// Retrieves a list of all subclasses of AbstractGameManagerData
    /// </summary>
    /// <returns></returns>
    public static List<AbstractGameManagerData> GetAllGameManagers()
    {
        List<AbstractGameManagerData> game_managers_data = new List<AbstractGameManagerData>();

        foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                 .Where(mytype => mytype.IsSubclassOf(typeof(AbstractGameManagerData))))
        {
            game_managers_data.Add((AbstractGameManagerData)Activator.CreateInstance(type));
        }

        return game_managers_data;
    }

    /// <summary>
    /// Strips a string of invisible characters
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string RemoveInvisibleChars(string input)
    {
        return Regex.Replace(input, @"\p{C}+", string.Empty);
    } 
}
