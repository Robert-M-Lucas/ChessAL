using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper for Unity's input. Adds support for multiple key presses.
/// </summary>
public static class I
{
    /// <summary>
    /// Is player holding this key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool GetKey(KeyCode key)
    {
        return Input.GetKey(key);
    }

    /// <summary>
    /// Is player holding all of these keys
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static bool GetKey(KeyCode[] keys)
    {
        foreach (var key in keys) { if (!Input.GetKey(key)) return false; }
        return true;
    }

    /// <summary>
    /// Is player holding one of these key combinations
    /// </summary>
    /// <param name="key_combinations"></param>
    /// <returns></returns>
    public static bool GetKey(KeyCode[][] key_combinations)
    {
        foreach (var key_combination in key_combinations)
        {
            if (GetKey(key_combination)) return true;
        }
        return false;
    }

    /// <summary>
    /// Has player pressed this key down this frame
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    /// <summary>
    /// Is client pressing this combination and pressed the last key this frame
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static bool GetKeyDown(KeyCode[] keys)
    {
        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (!Input.GetKey(keys[i])) return false;
        }

        return Input.GetKeyDown(keys[keys.Length - 1]);
    }

    /// <summary>
    /// Is client pressing one of these combinations and pressed the last key this frame
    /// </summary>
    /// <param name="key_combinations"></param>
    /// <returns></returns>
    public static bool GetKeyDown(KeyCode[][] key_combinations)
    {
        foreach (var key_combination in key_combinations)
        {
            if (GetKeyDown(key_combination)) return true;
        }
        return false;
    }

    /// <summary>
    /// Has player let go of this key down this frame
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    /// <summary>
    /// Is client pressing this combination and let go of the last key this frame
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static bool GetKeyUp(KeyCode[] keys)
    {
        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (!Input.GetKey(keys[i])) return false;
        }

        return Input.GetKeyUp(keys[keys.Length - 1]);
    }

    /// <summary>
    /// Is client pressing one of these combinations and let go of the last key this frame
    /// </summary>
    /// <param name="key_combinations"></param>
    /// <returns></returns>
    public static bool GetKeyUp(KeyCode[][] key_combinations)
    {
        foreach (var key_combination in key_combinations)
        {
            if (GetKeyUp(key_combination)) return true;
        }
        return false;
    }

    /// <summary>
    /// Is this mouse button being pressed
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool GetMouseButton(int button) => Input.GetMouseButton(button);

    /// <summary>
    /// Has this mouse button been pressed this frame
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool GetMouseButtonDown(int button) => Input.GetMouseButtonDown(button);

    /// <summary>
    /// Has this mouse button been let go of this frame
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool GetMouseButtonUp(int button) => Input.GetMouseButtonUp(button);
}