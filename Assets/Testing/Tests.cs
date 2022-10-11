using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[InitializeOnLoad]
public class Tests
{
    private static List<Func<bool>> tests = new List<Func<bool>> { TestPacketEncoding };

    static Tests()
    {
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

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
}