using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
/// <summary>
/// Class for automatically testing project every time the solution is recompiled
/// </summary>
[InitializeOnLoad]
public class Tests
{
    private static List<Func<bool>> tests = new List<Func<bool>> { TestPacketEncoding, TestGameManagers };

    static Tests()
    {
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    /// <summary>
    /// Called when code is recompiled
    /// </summary>
    private static void OnAfterAssemblyReload()
    {
        Debug.Log("Starting tests");

        foreach (var test in tests)
        {
            if (!test())
            {
                Debug.LogError("Tests failed");
                return;
            }
        }

        Debug.Log("Tests successful!");
    }

    /// <summary>
    /// Tests packet encoding forwards and backwards to ensure no data loss
    /// </summary>
    /// <returns></returns>
    private static bool TestPacketEncoding()
    {
        Debug.Log("Testing packet encoding");
        try
        {
            int arg_one = 2;
            double arg_two = 3.8;
            string arg_three = "ldksjfhalkahsdflkbvb";
            string arg_four = "hjsdagfgyurbfv";

            SampleTestPacket test_packet = new SampleTestPacket(PacketBuilder.Decode(SampleTestPacket.Build(arg_one, arg_two, arg_three, arg_four)));

            if (arg_one != test_packet.ArgOne || arg_two != test_packet.ArgTwo || arg_three != test_packet.ArgThree || arg_four != test_packet.ArgFour)
            {
                Debug.LogError("Packet data corrupted in conversion");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Packet encoding test failed with exception {e}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Runs various tests on game managers
    /// </summary>
    /// <returns></returns>
    private static bool TestGameManagers()
    {
        Debug.Log("Running Game Manager Tests");

        // Get all AbstractGameManagerData
        List<AbstractGameManagerData> abstract_game_managers_data = Util.GetAllGameManagers();

        // Ensure no duplicate UIDs
        HashSet<int> used_uids = new HashSet<int>();
        foreach (AbstractGameManagerData data in abstract_game_managers_data)
        {
            if (used_uids.Contains(data.GetUID())) { Debug.LogError($"Gamemode UID ({data.GetUID()}) used twice"); return false; }
            used_uids.Add(data.GetUID());
        }

        return true;
    }
}
#endif