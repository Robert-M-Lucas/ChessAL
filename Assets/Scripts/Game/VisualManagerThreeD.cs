using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManagerThreeD : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteHighlightMaterial;
    [SerializeField] private Material blackHighlightMaterial;
    [SerializeField] private Material whiteSelectMaterial;
    [SerializeField] private Material blackSelectMaterial;
    [SerializeField] private Material whiteBlockedMaterial;
    [SerializeField] private Material blackBlockedMaterial;
    [SerializeField] private Material whiteMoveMaterial;
    [SerializeField] private Material blackMoveMaterial;

    private MeshRenderer[,] squares;

    public void RenderBoard(BoardRenderInfo boardRenderInfo)
    {
        squares = new MeshRenderer[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];

        for (int x = 0; x < boardRenderInfo.BoardSize; x++)
        {
            for (int y = 0; y < boardRenderInfo.BoardSize; y++)
            {
                GameObject new_square = Instantiate(squarePrefab);
                squares[x, y] = new_square.GetComponent<MeshRenderer>();
                new_square.transform.SetParent(transform);

                ResetSquareColor(new V2(x, y), boardRenderInfo);

                new_square.transform.position = new Vector3(x - (boardRenderInfo.BoardSize / 2f), -0.25f, y - (boardRenderInfo.BoardSize / 2f));
                new_square.SetActive(true);
            }
        }
    }

    public void ResetSquareColor(V2 position, BoardRenderInfo boardRenderInfo)
    {
        if (VisualManager.IsWhite(position))
        {
            if (boardRenderInfo.RemovedSquares.Contains(position))
            {
                squares[position.X, position.Y].material = whiteBlockedMaterial;
            }
            else if (boardRenderInfo.HighlightedSquares.Contains(position))
            {
                squares[position.X, position.Y].material = whiteHighlightMaterial;
            }
            else
            {
                squares[position.X, position.Y].material = whiteMaterial;
            }
        }
        else
        {
            if (boardRenderInfo.RemovedSquares.Contains(position))
            {
                squares[position.X, position.Y].material = blackBlockedMaterial;
            }
            else if (boardRenderInfo.HighlightedSquares.Contains(position))
            {
                squares[position.X, position.Y].material = blackHighlightMaterial;
            }
            else
            {
                squares[position.X, position.Y].material = blackMaterial;
            }
        }
    }

    public void UpdateTheme(Theme theme)
    {
        whiteMaterial.color = theme.WhiteColor;
        blackMaterial.color = theme.BlackColor;
        whiteHighlightMaterial.color = theme.WhiteHighlightColor;
        blackHighlightMaterial.color = theme.BlackHighlightColor;
        whiteSelectMaterial.color = theme.WhiteSelectColor;
        blackSelectMaterial.color = theme.BlackSelectColor;
        whiteMoveMaterial.color = theme.WhiteMoveColor;
        blackMoveMaterial.color = theme.BlackMoveColor;
        whiteBlockedMaterial.color = theme.WhiteBlockedColor;
        blackBlockedMaterial.color = theme.BlackBlockedColor;
    }
}
