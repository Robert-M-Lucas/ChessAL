using Game;
using Gamemodes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RippleData
{
    public V2 position;
    public float time = 0;

    public RippleData(V2 position) { this.position = position; }
}

public class VisualManagerThreeD : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private VisualManager visualManager;

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
    [SerializeField] private Material white3DHighlightMaterial;
    [SerializeField] private Material black3DHighlightMaterial;
    [SerializeField] private Material white3DTakeHighlightMaterial;
    [SerializeField] private Material black3DTakeHighlightMaterial;

    private MeshRenderer[,] squares;

    private Dictionary<V2, GameObject> pieces= new Dictionary<V2, GameObject>();

    private List<V2> highlightedSquares= new List<V2>();

    private V2 moveFromSquare = new V2(-1);
    private V2 moveToSquare = new V2(-1);

    private BoardRenderInfo boardRenderInfo;
    

    private float[,] targetDisplacement;
    private bool[,] highlighted;

    private V2 hoveringOver = new V2(-1, -1);
    private V2 selected = new V2(-1, -1);

    private List<RippleData> ripples = new List<RippleData>();

    public void RenderBoard(BoardRenderInfo boardRenderInfo)
    {
        this.boardRenderInfo = boardRenderInfo;

        squares = new MeshRenderer[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];

        for (int x = 0; x < boardRenderInfo.BoardSize; x++)
        {
            for (int y = 0; y < boardRenderInfo.BoardSize; y++)
            {
                GameObject new_square = Instantiate(squarePrefab);
                squares[x, y] = new_square.GetComponent<MeshRenderer>();
                new_square.transform.SetParent(transform);

                ResetSquareColor(new V2(x, y));

                new_square.transform.position = new Vector3(x - (boardRenderInfo.BoardSize / 2f), -0.25f, y - (boardRenderInfo.BoardSize / 2f));
                new_square.SetActive(true);
            }
        }

        cameraManager.cameraPosition.localPosition = new Vector3(0, 0, -boardRenderInfo.BoardSize * 1.5f);
        cameraManager.minDist = Mathf.Sqrt(2 * Mathf.Pow((boardRenderInfo.BoardSize + 2) / 2, 2));
        cameraManager.maxDist = cameraManager.minDist * 2;
    }

    public void ResetSquareColor(V2 position)
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
            else if (position == moveFromSquare || position == moveToSquare)
            {
                squares[position.X, position.Y].material = whiteMoveMaterial;
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
            else if (position == moveFromSquare || position == moveToSquare)
            {
                squares[position.X, position.Y].material = blackMoveMaterial;
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
        white3DHighlightMaterial.color = theme.White3DHighlightColor;
        black3DHighlightMaterial.color = theme.Black3DHighlightColor;
        white3DTakeHighlightMaterial.color = theme.White3DTakeHighlightColor;
        black3DTakeHighlightMaterial.color = theme.Black3DTakeHighlightColor;
    }

    public void Create(AbstractPiece piece)
    {
        GameObject new_piece = Instantiate(visualManager.InternalAppearanceMap[piece.AppearanceID].Prefab3D);
        new_piece.AddComponent<PieceControllerThreeD>();
        pieces.Add(piece.Position, new_piece);
        new_piece.transform.SetParent(squares[piece.Position.X, piece.Position.Y].transform);
        new_piece.transform.localPosition = new Vector3(0, 0.25f, 0);
    }

    public void Move(V2 from, V2 to) 
    {
        bool jump = false;
        Vector2 delta = (to - from).Vector2();
        int count = (int) delta.magnitude;

        for (int i = 0; i < count; i++)
        {
            Vector2 current_pos = from.Vector2() + (delta * (i / (float)count));
            V2 min_pos = new V2((int) Mathf.Floor(current_pos.x), (int)Mathf.Floor(current_pos.y));
            V2 max_pos = new V2((int)Mathf.Ceil(current_pos.x), (int)Mathf.Ceil(current_pos.y));
            if (min_pos != from && min_pos != to)
            {
                if (pieces.ContainsKey(min_pos))
                {
                    jump = true; break;
                }
            }
            if (max_pos != from && max_pos != to)
            {
                if (pieces.ContainsKey(max_pos))
                {
                    jump = true; break;
                }
            }
        }


        pieces[from].transform.SetParent(squares[to.X, to.Y].transform);
        pieces[from].GetComponent<PieceControllerThreeD>().Move(
            pieces[from].transform.localPosition, 
            new Vector3(0, 0.25f, 0), 
            jump, visualManager.pieceTravelTime, 2f);

        pieces[to] = pieces[from];
        pieces.Remove(from);
    }

    public void DestroyPiece(V2 position) 
    {
        Destroy(pieces[position]);
        pieces.Remove(position);
        ripples.Add(new RippleData(position));
    }

    private void HoverSquare(V2 position) { hoveringOver = position; }

    private void HoverNone() { hoveringOver = new V2(-1, -1); }

    private void SelectSquare(V2 position, VisualManager visualManager)
    {
        if (position == selected) selected = new V2(-1);
        else
        {
            if (visualManager.possibleMoves.FindIndex((m) => m.From == selected && m.To == position) != -1)
            {
                visualManager.ChessManager.DoLocalMove(selected, position);
                selected = new V2(-1);
            }
            else
                selected = position;
        }
    }

    private void DeselectSquare() { selected = new V2(-1, -1); }

    private void HighlightSquare(V2 square)
    {
        highlightedSquares.Add(square);
        if (VisualManager.IsWhite(square))
            if (pieces.ContainsKey(square)) squares[square.X, square.Y].material = white3DTakeHighlightMaterial;
            else squares[square.X, square.Y].material = white3DHighlightMaterial;
        else
            if (pieces.ContainsKey(square)) squares[square.X, square.Y].material = black3DTakeHighlightMaterial;
            else squares[square.X, square.Y].material = black3DHighlightMaterial;
    }

    private void ClearHighlighted()
    {
        foreach(V2 square in highlightedSquares)
        {
            ResetSquareColor(square);
        }
        highlightedSquares.Clear();
    }

    public void ShowLastMove(V2 from, V2 to)
    {
        V2 temp_place = moveFromSquare;
        moveFromSquare = new V2(-1);
        if (temp_place != new V2(-1)) ResetSquareColor(temp_place);
        temp_place = moveToSquare;
        moveToSquare = new V2(-1);
        if (temp_place != new V2(-1)) ResetSquareColor(temp_place);

        if (VisualManager.IsWhite(from)) squares[from.X, from.Y].material = whiteMoveMaterial;
        else squares[from.X, from.Y].material = blackMoveMaterial;

        if (VisualManager.IsWhite(to)) squares[to.X, to.Y].material = whiteMoveMaterial;
        else squares[to.X, to.Y].material = blackMoveMaterial;

        moveFromSquare = from;
        moveToSquare = to;
    }   

    public void ExternalUpdate()
    {
        V2? currentPosition = cameraManager.ExternalUpdate(boardRenderInfo.BoardSize);
        if (currentPosition is not null && !I.GetMouseButton(K.SecondaryClick))
        {
            if (I.GetMouseButtonDown(K.PrimaryClick)) SelectSquare((V2)currentPosition, visualManager);
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
        for (int i = 0; i < ripples.Count; i++)
        {
            ripples[i].time += Time.deltaTime / 2f;
            if (ripples[i].time >= 1f)
            {
                ripples.RemoveAt(i);
                i--;
            }
            i++;
        }

        targetDisplacement = new float[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];
        highlighted = new bool[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];

        if (hoveringOver.X != -1)
        {
            targetDisplacement[hoveringOver.X, hoveringOver.Y] += 0.2f;
        }

        if (selected.X != -1)
        {
            targetDisplacement[selected.X, selected.Y] += 0.3f;
            // highlighted[selected.X, selected.Y] = true;

            if (visualManager.possibleMoves.FindIndex((m) => m.From == selected) != -1)
            {
                foreach (Move m in visualManager.possibleMoves.FindAll((m) => m.From == selected))
                {
                    highlighted[m.To.X, m.To.Y] = true;
                    targetDisplacement[m.To.X, m.To.Y] += 0.2f;
                }
            }
        }

        float max_distance = new Vector2(boardRenderInfo.BoardSize / 2f, boardRenderInfo.BoardSize / 2f).magnitude;
        for (int x = 0; x < boardRenderInfo.BoardSize; x++)
        {
            for (int y = 0; y < boardRenderInfo.BoardSize; y++)
            {
                foreach (RippleData ripple in ripples)
                {
                    float distance = (ripple.position - new V2(x, y)).Vector2().magnitude;
                    targetDisplacement[x, y] += MathP.Ripple(ripple.time, 1, distance, max_distance, 6) * 0.4f;
                }
            }
        }


            ClearHighlighted();
        for (int x = 0; x < boardRenderInfo.BoardSize; x++)
        {
            for (int y = 0; y < boardRenderInfo.BoardSize; y++)
            {
                float target_delta = targetDisplacement[x, y] + 1 - squares[x, y].transform.position.y;
                if (target_delta == 0) { return; }
                squares[x, y].transform.position += Vector3.up * Mathf.Log(target_delta, 2) * Time.deltaTime * 10;
                squares[x, y].transform.position = new Vector3(squares[x, y].transform.position.x, Mathf.Clamp(squares[x, y].transform.position.y, -10, 10), squares[x, y].transform.position.z);
                if (highlighted[x, y]) HighlightSquare(new V2(x, y));
            }
        }
    }
}
