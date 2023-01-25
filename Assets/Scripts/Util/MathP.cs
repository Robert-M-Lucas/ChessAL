using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class containing additional math functions
/// </summary>
public static class MathP
{
    /// <summary>
    /// Smooths a movement using the first half of a cos wave
    /// </summary>
    /// <param name="linear"></param>
    /// <returns>Progress between 0 and 1</returns>
    public static float CosSmooth(float linear)
    {
        return (-Mathf.Cos(linear * Mathf.PI) + 1) / 2f;
    }

    /// <summary>
    /// Creates a jump shape using the first half of a sin wave
    /// </summary>
    /// <param name="linear"></param>
    /// <returns>Height at given distance between 0 and 1</returns>
    public static float SmoothJump(float linear)
    {
        if (linear >= 1f) linear = 0.9999f;
        return Mathf.Sqrt(Mathf.Sin(linear * Mathf.PI));
    }

    /// <summary>
    /// Creates ripples between an origin point and max distance
    /// </summary>
    /// <param name="time">How much time (0 - 1) has passed relative to the max time</param>
    /// <param name="cycles">How many ripples should be created</param>
    /// <param name="distance">How far the point is from the origin</param>
    /// <param name="max_distance">The furthest point the ripples can reach</param>
    /// <param name="ripple_Length">How long each ripple should be</param>
    /// <returns>Height at given distance</returns>
    public static float Ripple(float time, float cycles, float distance, float max_distance, float ripple_Length)
    {
        // y = -(1 / max_distance)x + 1
        float amplitude_falloff = -(1 / max_distance) * distance + 1;

        // y = sin(2pi * (1/spr) * x - 2pi * (t * c))
        float ripple = Mathf.Sin(2 * Mathf.PI * (1 / ripple_Length) * distance - 2 * Mathf.PI * (time * cycles));

        float overall_amplitude;
        if (time < 0.2f)
        {
            overall_amplitude = Mathf.Sqrt(Mathf.Sin(Mathf.PI * time * 2.5f));
        }
        else
        {
            overall_amplitude = (-1 * ((time-0.2f) / 0.8f)) + 1;
        }

        return ripple * amplitude_falloff * overall_amplitude;
    }
}
