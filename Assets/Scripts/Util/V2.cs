using System;
using UnityEngine;

/// <summary>
/// Simplified Vector2
/// </summary>
[System.Serializable]
public struct V2: IEquatable<V2>
{
    public int X;
    public int Y;

    public static V2 Up { get { return new V2(0, 1); } }
    public static V2 Down { get { return new V2(0, -1); } }
    public static V2 Left { get { return new V2(-1, 0); } }
    public static V2 Right { get { return new V2(1, 0); } }

    public V2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public V2(int xy)
    {
        X = xy;
        Y = xy;
    }

    public bool Equals(V2 b) => Equals((V2?)b);

    public bool Equals(V2? nb)
    {
        if (nb is null) return false;

        var b = (V2)nb;

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
        var h_code = X ^ Y; // Make hash code combination of X and Y
        return h_code.GetHashCode();
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}