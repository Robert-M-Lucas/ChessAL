using System;

/// <summary>
/// Provides additional functions for handling byte[]
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Overwrites part of an array with another
    /// </summary>
    /// <param name="bigArr"></param>
    /// <param name="smallArr"></param>
    /// <param name="index">Overwrite start</param>
    /// <param name="smallArrLength"></param>
    /// <returns></returns>
    public static byte[] Merge(byte[] bigArr, byte[] smallArr, int index, out int smallArrLength)
    {
        smallArrLength = smallArr.Length;

        foreach (var item in smallArr)
        {
            bigArr[index] = item;
            index++;
        }

        return bigArr;
    }
    public static byte[] Merge(byte[] bigArr, byte[] smallArr, int index = 0)
    {
        return Merge(bigArr, smallArr, index, out var _);
    }

    /// <summary>
    /// Gets a smaller array out of a larger one
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="start">(inclusive)</param>
    /// <param name="end">(exclusive)</param>
    /// <returns></returns>
    public static byte[] Slice(byte[] arr, int start, int end)
    {
        var to_return = new byte[end - start];
        var index = 0;
        for (var i = start; i < end; i++)
        {
            to_return[index] = arr[i];
            index++;
        }
        return to_return;
    }

    public static Tuple<byte[], int> ClearEmpty(byte[] arr)
    {
        var index = 0;
        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i] != 0)
            {
                index = i;
                break;
            }
        }
        return new Tuple<byte[], int>(
            Merge(new byte[arr.Length], Slice(arr, index, arr.Length)),
            index
        );
    }
}