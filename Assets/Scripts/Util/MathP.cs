using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathP
{
    public static float CosSmooth(float linear)
    {
        return (-Mathf.Cos(linear * Mathf.PI) + 1) / 2;
    }
}
