using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathP
{
    public static float CosSmooth(float linear)
    {
        return (-Mathf.Cos(linear * Mathf.PI) + 1) / 2f;
    }

    public static float SmoothJump(float linear)
    {
        if (linear >= 1f) linear = 0.9999f;
        return Mathf.Sqrt(Mathf.Sin(linear * Mathf.PI));
    }

    public static float Ripple(float time, float cycles, float distance, float max_distance, float squares_per_ripple)
    {
        // y = -(1 / max_distance)x + 1
        float amplitude_falloff = -(1 / max_distance) * distance + 1;

        // y = sin(2pi * (1/spr) * x - 2pi * (t * c))
        float ripple = Mathf.Sin(2 * Mathf.PI * (1 / squares_per_ripple) * distance - 2 * Mathf.PI * (time * cycles));

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
