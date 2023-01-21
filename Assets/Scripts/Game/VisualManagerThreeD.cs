using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualManagerThreeD : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;

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

    private int boardSize;

    private float[,] targetDisplacement;
    private int[,] highlighted;

    private V2 hoveringOver = new V2(-1, -1);
    private V2 selected = new V2(-1, -1);

    public void RenderBoard(BoardRenderInfo boardRenderInfo)
    {
        boardSize = boardRenderInfo.BoardSize;

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

        cameraManager.cameraPosition.localPosition = new Vector3(0, 0, -boardRenderInfo.BoardSize * 1.5f);
        cameraManager.minDist = Mathf.Sqrt(2 * Mathf.Pow((boardRenderInfo.BoardSize + 2) / 2, 2));
        cameraManager.maxDist = cameraManager.minDist * 2;
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

    public void Create() { }

    public void Move() { }

    public void Destroy() { }

    private void HoverSquare(V2 position) { hoveringOver = position; }

    private void HoverNone() { hoveringOver = new V2(-1, -1); }

    private void SelectSquare(V2 position) { selected = position; }

    private void DeselectSquare() { selected = new V2(-1, -1); }

    public void ExternalUpdate()
    {

        V2? currentPosition = cameraManager.ExternalUpdate(boardSize);
        if (currentPosition is not null && !I.GetMouseButton(K.SecondaryClick))
        {
            if (I.GetMouseButtonDown(K.PrimaryClick)) SelectSquare((V2)currentPosition);
            HoverSquare((V2)currentPosition);
        }
        else
        {
            if (I.GetMouseButtonDown(K.PrimaryClick)) DeselectSquare();
            HoverNone();
        }

        UpdateDisplacements();
    }

    private void UpdateDisplacements()
    {
        targetDisplacement = new float[boardSize, boardSize];
        highlighted = new int[boardSize, boardSize];

        if (hoveringOver.X != -1)
        {
            targetDisplacement[hoveringOver.X, hoveringOver.Y] += 0.2f;
        }

        if (selected.X != -1)
        {
            targetDisplacement[selected.X, selected.Y] += 0.3f;
        }

        /*
        foreach (Tuple<Vector2Int, bool> possible_move in chessManagerInterface.possibleMoves)
        {
            targetDisplacement[possible_move.Item1.x, possible_move.Item1.y] += 0.2f;
            if (chessManagerInterface.chessManager.State.GetPieceAtPosition(possible_move.Item1) != null)
            {
                highlighted[possible_move.Item1.x, possible_move.Item1.y] = 2;
            }
            else
            {
                highlighted[possible_move.Item1.x, possible_move.Item1.y] = 1;
            }
        }
        */

        /*
        int i = 0;
        while (i < ripples.Count)
        {
            if (ripples[i].frame_num >= Ripple.FRAME_COUNT)
            {
                ripples.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        foreach (RippleData ripple in ripples)
        {
            float[,] frame = ripple.GetFrame();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    targetDisplacement[x, y] += frame[x, y];
                }
            }
        }
        */

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                float target_delta = targetDisplacement[x, y] + 1 - squares[x, y].transform.position.y;
                if (target_delta == 0) { return; }
                squares[x, y].transform.position += Vector3.up * Mathf.Log(target_delta, 2) * Time.deltaTime * 10;
                squares[x, y].transform.position = new Vector3(squares[x, y].transform.position.x, Mathf.Clamp(squares[x, y].transform.position.y, -10, 10), squares[x, y].transform.position.z);
            }
        }
    }
}
