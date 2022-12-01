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

        game_managers_data.RemoveAll(o => o.GetUID() == 900);
        
        return game_managers_data.OrderBy(o => o.GetUID()).ToList();
    }

    /// <summary>
    /// Retrieves a list of all subclasses of AbstractPiece
    /// </summary>
    /// <returns></returns>
    public static List<Type> GetAllPieceTypes()
    {
        List<Type> pieces = new List<Type>();

        foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                 .Where(mytype => mytype.IsSubclassOf(typeof(AbstractPiece)) && !mytype.IsAbstract))
        {
            pieces.Add(type);
        }

        return pieces;
    }

    /// <summary>
    /// Retrieves a list of all subclasses of AbstractPiece
    /// </summary>
    /// <returns></returns>
    public static List<AbstractPiece> GetAllPieces()
    {
        List<AbstractPiece> pieces = new List<AbstractPiece>();

        foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                 .Where(mytype => mytype.IsSubclassOf(typeof(AbstractPiece)) && !mytype.IsAbstract))
        {
            pieces.Add((AbstractPiece)Activator.CreateInstance(type, new object[] { new V2(0, 0), 0, null }));
        }

        return pieces;
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
