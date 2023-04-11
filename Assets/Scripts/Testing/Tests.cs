using UnityEditor;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Networking.Packets;
using Networking.Packets.Generated;
using Gamemodes;
using System.IO;

#nullable enable

// Don't run in build
#if UNITY_EDITOR

/// <summary>
/// Class for automatically testing project every time the solution is recompiled. Not included in build.
/// </summary>
[InitializeOnLoad]
public class Tests
{
    private static List<Func<bool>> tests = new List<Func<bool>> { TestPacketEncoding, TestGameManagers, TestPieces, TestValidators, TestV2 };

    private static string outputString = "Test log: [...]\n";

    static Tests()
    {
        // Add method to event
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    /// <summary>
    /// Called when code is recompiled
    /// </summary>
    private static void OnAfterAssemblyReload()
    {
        Debug.Log("Starting tests");
        var s = new Stopwatch();
        s.Start();

        foreach (var test in tests)
        {
            if (!test())
            {
                Debug.Log(outputString);
                Debug.LogError("Tests failed");
                return;
            }
        }

        Debug.Log(outputString);

        Debug.Log($"Tests successful! ({s.ElapsedMilliseconds}ms)");
    }

    /// <summary>
    /// Tests packet encoding forwards and backwards to ensure no data loss
    /// </summary>
    /// <returns></returns>
    private static bool TestPacketEncoding()
    {
        outputString += "Testing packet encoding\n";
        try
        {
            var arg_one = 2;
            var arg_two = 3.8;
            var arg_three = "ldksjfhalkahsdflkbvb";
            var arg_four = "hjsdagfgyurbfv";

            var test_packet = new SampleTestPacket(PacketBuilder.Decode(SampleTestPacket.Build(arg_one, arg_two, arg_three, arg_four)));

            // ReSharper disable once CompareOfFloatsByEqualityOperator
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
        outputString += "Running Game Manager Tests\n";

        // Get all AbstractGameManagerData
        List<AbstractGameManagerData> abstract_game_managers_data = Util.GetAllGameManagers();

        // Ensure no duplicate UIDs
        var used_uids = new HashSet<int>();
        foreach (var data in abstract_game_managers_data)
        {
            if (used_uids.Contains(data.GetUID())) { Debug.LogError($"Gamemode UID ({data.GetUID()}) used twice"); return false; }
            used_uids.Add(data.GetUID());

            // Ensure all gamemode names can be used in file names
            if (data.GetName().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                Debug.LogError($"{data.GetName()} contains invalid characters for a file name");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Runs various tests on game managers
    /// </summary>
    /// <returns></returns>
    private static bool TestPieces()
    {
        outputString += "Running Piece Tests\n";

        // Get all AbstractGameManagerData
        List<AbstractPiece> pieces = Util.GetAllPieces();

        // Ensure no duplicate UIDs
        var used_uids = new HashSet<int>();
        foreach (var data in pieces)
        {
            if (used_uids.Contains(data.GetUID())) { Debug.LogError($"Piece UID ({data.GetUID()}) used twice"); return false; }
            used_uids.Add(data.GetUID());
        }

        return true;
    }

    /// <summary>
    /// Tests validators such as those found in the 'Validators' static class
    /// </summary>
    /// <returns></returns>
    private static bool TestValidators()
    {
        outputString += "Running validator tests\n";

        Func<string, string?> validator = Validators.ValidatePlayerName;
        Tuple<string, bool>[] validation_tests = new Tuple<string, bool>[] {
            new Tuple<string, bool>("PlayerName", true), 
            new Tuple<string, bool>("Player Name", false), // Illegal char
            new Tuple<string, bool>("1234", true),
            new Tuple<string, bool>("pls", false),
            new Tuple<string, bool>("", false),// Too short
            new Tuple<string, bool>("rjfnsmekfntismes", true),
            new Tuple<string, bool>("rjfnsmekfntismess", false), // Too long
        };

        foreach (Tuple<string, bool> test in validation_tests)
        {
            if ((validator(test.Item1) is null) != test.Item2)
            {
                if (test.Item2) Debug.LogError($"Name '{test.Item1}' failed validation when it should have passed");
                else Debug.LogError($"Name '{test.Item1}' passed validation when it should have failed");
                return false;
            }
        }

        validator = Validators.ValidatePassword;
        validation_tests = new Tuple<string, bool>[] {
            new Tuple<string, bool>("Password", true),
            new Tuple<string, bool>("", true), // No password
            new Tuple<string, bool>("My Password", false), // Illegal char
            new Tuple<string, bool>("sfneksmrnaweirma", true),
            new Tuple<string, bool>("sfneksmrnaweirmaa", false), // Too long
            new Tuple<string, bool>("asd!", true),
            new Tuple<string, bool>("ad!", false), // Too short
            new Tuple<string, bool>("ad!\\", false), // Illegal char
        };

        foreach (Tuple<string, bool> test in validation_tests)
        {
            if ((validator(test.Item1) is null) != test.Item2)
            {
                if (test.Item2) Debug.LogError($"Password '{test.Item1}' failed validation when it should have passed");
                else Debug.LogError($"Password '{test.Item1}' passed validation when it should have failed");
                return false;
            }
        }

        return true;
    }

    /*
    /// <summary>
    /// Validates the team composition checker
    /// </summary>
    /// <returns></returns>
    private static bool TestTeamValidators()
    {
        output_string += "Running team validator tests\n";
        Func<TeamSize[], string?> validator = Validators.ValidateTeams;
        Tuple<TeamSize[], bool>[] tests = new Tuple<TeamSize[], bool>[] {
            new Tuple<TeamSize[], bool>("PlayerName", true),
            new Tuple<TeamSize[], bool>("Player Name", false),
            new Tuple<TeamSize[], bool>("1234", true),
            new Tuple<TeamSize[], bool>("pls", false),
            new Tuple<TeamSize[], bool>("", false),// Too short
            new Tuple<TeamSize[], bool>("rjfnsmekfntismes", true),
            new Tuple<TeamSize[], bool>("rjfnsmekfntismess", false),
        };

        foreach (Tuple<string, bool> test in tests)
        {
            if ((validator(test.Item1) is null) != test.Item2)
            {
                if (test.Item2) Debug.LogError($"Name '{test.Item1}' failed validation when it should have passed");
                else Debug.LogError($"Name '{test.Item1}' passed validation when it should have failed");
                return false;
            }
        }

        return true;
    }
    */

    /// <summary>
    /// Runs tests on V2 methods and operations
    /// </summary>
    /// <returns></returns>
    private static bool TestV2()
    {
        outputString += "Testing V2 functions\n";

        // ReSharper disable once EqualExpressionComparison
        if (new V2(1, 3) != new V2(1,3) || new V2(1, 5) == new V2(3, 4))
        {
            Debug.LogError("V2 equality testing failed");
            return false;
        }

        if (new V2(1, 4) + new V2(2, 7) != new V2(3, 11) || new V2(20, 10) - new V2(2, 4) != new V2(18, 6))
        {
            Debug.LogError("V2 operator testing failed");
            return false;
        }

        return true;
    }
}

/*
public class TestNetworkManager: NetworkManager
{

}
*/
#endif