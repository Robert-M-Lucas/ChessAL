using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Stores a single mapping from a piece ID to its sprite
/// </summary>
[System.Serializable]
public class PieceSprite
{
    public string Name;
    public int ID;
    public Sprite Sprite;
}


/// <summary>
/// Stores a map of Appearance IDs to Sprites
/// </summary>
[CreateAssetMenu(fileName = "AppearanceTable", menuName = "AppearanceTable", order = 1)]
public class AppearanceTable: ScriptableObject
{
    public PieceSprite[] Appearances;
}

/*
public static class AppearanceTableUtil
{
    public static AppearanceTable[] GetAllInstances()
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(AppearanceTable).Name);  //FindAssets uses tags check documentation for more info
        AppearanceTable[] a = new AppearanceTable[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<AppearanceTable>(path);
        }

        return a;

    }
}
*/