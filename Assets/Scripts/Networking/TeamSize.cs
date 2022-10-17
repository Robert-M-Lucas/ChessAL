
/// <summary>
/// Holds minimum and maximum players per team
/// </summary>
public struct TeamSize
{
    public int Min;
    public int Max;

    public TeamSize(int min, int max)
    {
        Min = min;
        Max = max;
    }
}
