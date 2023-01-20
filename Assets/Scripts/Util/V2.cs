using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simplified Vector2
/// </summary>
[System.Serializable]
public struct V2: IEquatable<V2>
{
    public int X;
    public int Y;

    public V2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(V2 b) => Equals((V2?)b);

    public bool Equals(V2? nb)
    {
        if (nb is null) return false;

        V2 b = (V2)nb;

        if (this.X == b.X && this.Y == b.Y)
            return true;
        else
            return false;
    }

    public override bool Equals(object b) => Equals(b as V2?);

    public static bool operator ==(V2 lhs, V2 rhs)
    {
        return lhs.Equals(rhs);
    }
    public static bool operator !=(V2 lhs, V2 rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static V2 operator +(V2 lhs, V2 rhs)
    {
        return new V2(lhs.X + rhs.X, lhs.Y + rhs.Y);
    }
    public static V2 operator -(V2 lhs, V2 rhs)
    {
        return new V2(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static V2 operator *(V2 lhs, int rhs)
    {
        return new V2(lhs.X * rhs, lhs.Y * rhs);
    }

    public static V2 operator /(V2 lhs, int rhs)
    {
        return new V2(lhs.X / rhs, lhs.Y / rhs);
    }

    public Vector2 Vector2()
    {
        return new Vector2(X, Y);
    }

    public override int GetHashCode()
    {
        int hCode = X ^ Y; // Make hash code combination of X and Y
        return hCode.GetHashCode();
    }
}