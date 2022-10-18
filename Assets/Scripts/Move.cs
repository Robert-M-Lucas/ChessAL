using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a possible piece move
/// </summary>
public struct Move
{
    public V2 From;
    public V2 To;

    public Move(int xFrom, int yFrom, int xTo, int yTo)
    {
        From = new V2(xFrom, yFrom);
        To = new V2(xTo, yTo);
    }

    public Move(V2 from, V2 to) 
    {
        From = from;
        To = to;
    }
}