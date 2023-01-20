using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManagerThreeD : MonoBehaviour
{
    public GameObject SquarePrefab;

    public void RenderBoard(BoardRenderInfo boardRenderInfo)
    {
        for (int x = 0; x < boardRenderInfo.BoardSize; x++)
        {
            for (int y = 0; y < boardRenderInfo.BoardSize; y++)
            {
                GameObject new_square = Instantiate(SquarePrefab);
                new_square.transform.SetParent(transform);
                new_square.transform.position = new Vector3(x - (boardRenderInfo.BoardSize / 2f), -0.25f, y - (boardRenderInfo.BoardSize / 2f));
                new_square.SetActive(true);
            }
        }
    }
}
