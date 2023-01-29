using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using UnityEngine;
using UnityEngine.Rendering;
using System.Diagnostics;
using System.Linq;

#nullable enable

public static class SaveSystem
{
    public static string GetSaveFolderAndCreateIfDoesntExist() {
        string save_location = Application.persistentDataPath + "\\Saves";
        if (!Directory.Exists(save_location))
        {
            Directory.CreateDirectory(save_location); ;
        }
        return save_location;
    }

    public static void Save(Gamemodes.SerialisationData data, string fileName)
    {
        Save(Gamemodes.SerialisationUtil.Construct(data), fileName);
    }

    /// <summary>
    /// Saves game data to disk
    /// </summary>
    /// <param name="data"></param>
    /// <param name="fileName"></param>
    /// <returns>Null if successful or a string exception</returns>
    public static string? Save(byte[] data, string fileName)
    {
        string save_location = GetSaveFolderAndCreateIfDoesntExist();
        try
        {
            File.WriteAllBytes(save_location + "\\" + fileName + ".sav", data);
        }
        catch (IOException) { return "IO error when trying to write save data"; }
        catch (UnauthorizedAccessException) { return "Write permissions denied";  }
        catch (SecurityException) { return "Write permissions denied"; }
        catch (Exception e) { return $"Unhandled exception:\n{e}"; }
        return null;
    }

    /// <summary>
    /// Returns a list of save files
    /// </summary>
    /// <returns></returns>
    public static string[] ListAllSaveFiles()
    {
        string save_location = GetSaveFolderAndCreateIfDoesntExist();
        DirectoryInfo info = new DirectoryInfo(save_location);
        FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
        string[] file_names = new string[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            file_names[i] = files[i].Name;
        }
        return file_names;
    }

    /// <summary>
    /// Parses a file name and returns its bytes
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static byte[] Load(string fileName)
    {
        // Remove leading and trailing " if exists
        if (fileName[0] == '"') fileName = fileName.Substring(1, fileName.Length - 1);
        if (fileName[fileName.Length - 1] == '"') fileName = fileName.Substring(0, fileName.Length - 1);

        // Adds extension if needed
        if (Path.GetExtension(fileName) != ".sav") fileName += ".sav";

        // Adds path to save folder if only filename is provided
        string save_location = GetSaveFolderAndCreateIfDoesntExist();
        if (!fileName.Contains('\\') && !fileName.Contains('/')) fileName = save_location + "\\" + fileName;

        return File.ReadAllBytes(fileName);
    }

    /// <summary>
    /// Opens the windows file explorer in the saves folder
    /// </summary>
    public static void OpenSavesFolder()
    {
        string save_location = GetSaveFolderAndCreateIfDoesntExist();
        if (!Directory.Exists(save_location)) Directory.CreateDirectory(save_location);

        // Windows only open file explorer
#if PLATFORM_STANDALONE_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Process.Start(new ProcessStartInfo()
        {
            FileName = save_location + "\\",
            UseShellExecute = true,
            Verb = "open"
        });
#endif
    }
}
