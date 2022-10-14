using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simplified Vector2
/// </summary>
public struct V2
{
    public ushort X;
    public ushort Y;

    public V2(int x, int y)
    {
        X = (ushort)x;
        Y = (ushort)y;
    }

    public V2(ushort x, ushort y)
    {
        X = x;
        Y = y;
    }
}
