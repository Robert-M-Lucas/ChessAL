using Game;
using Gamemodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.ThreeD
{
    /// <summary>
    /// Stores data about an active ripple
    /// </summary>
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

        private Dictionary<V2, GameObject> pieces = new Dictionary<V2, GameObject>();

        private List<V2> highlightedSquares = new List<V2>();

        private V2 moveFromSquare = new V2(-1);
        private V2 moveToSquare = new V2(-1);

        private BoardRenderInfo boardRenderInfo;


        private float[,] targetDisplacement;
        private bool[,] highlighted;

        private V2 hoveringOver = new V2(-1, -1);
        private V2 selected = new V2(-1, -1);

        private List<RippleData> ripples = new List<RippleData>();

        /// <summary>
        /// Renders everything on the 3D board
        /// </summary>
        /// <param name="boardRenderInfo"></param>
        public void RenderBoard(BoardRenderInfo boardRenderInfo)
        {
            this.boardRenderInfo = boardRenderInfo;

            squares = new MeshRenderer[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];

            for (int x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (int y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    // Create new square
                    GameObject new_square = Instantiate(squarePrefab);
                    squares[x, y] = new_square.GetComponent<MeshRenderer>();
                    new_square.transform.SetParent(transform);

                    // Set square colour
                    ResetSquareColor(new V2(x, y));

                    // Set square position
                    new_square.transform.position = new Vector3(x - (boardRenderInfo.BoardSize / 2f), -0.25f, y - (boardRenderInfo.BoardSize / 2f));

                    // Show
                    new_square.SetActive(true);
                }
            }

            // Set camera position variables based on board size
            cameraManager.cameraPosition.localPosition = new Vector3(0, 0, -boardRenderInfo.BoardSize * 1.5f);
            cameraManager.minDist = Mathf.Sqrt(2 * Mathf.Pow((boardRenderInfo.BoardSize + 2) / 2, 2));
            cameraManager.maxDist = cameraManager.minDist * 2;
        }

        /// <summary>
        /// Sets square to its default colour
        /// </summary>
        /// <param name="position"></param>
        public void ResetSquareColor(V2 position)
        {
            if (VisualManager.IsWhite(position))
            {
                if (boardRenderInfo.RemovedSquares.Contains(position))
                    squares[position.X, position.Y].material = whiteBlockedMaterial;
                else if (boardRenderInfo.HighlightedSquares.Contains(position))
                    squares[position.X, position.Y].material = whiteHighlightMaterial;
                else if (position == moveFromSquare || position == moveToSquare)
                    squares[position.X, position.Y].material = whiteMoveMaterial;
                else
                    squares[position.X, position.Y].material = whiteMaterial;
            }
            else
            {
                if (boardRenderInfo.RemovedSquares.Contains(position))
                    squares[position.X, position.Y].material = blackBlockedMaterial;
                else if (boardRenderInfo.HighlightedSquares.Contains(position))
                    squares[position.X, position.Y].material = blackHighlightMaterial;
                else if (position == moveFromSquare || position == moveToSquare)
                    squares[position.X, position.Y].material = blackMoveMaterial;
                else
                     squares[position.X, position.Y].material = blackMaterial;
            }
        }

        /// <summary>
        /// Updates the material colours to a new given theme
        /// </summary>
        /// <param name="theme"></param>
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

        /// <summary>
        /// Creates a 3D piece
        /// </summary>
        /// <param name="piece"></param>
        public void Create(AbstractPiece piece)
        {
            GameObject new_piece = Instantiate(visualManager.InternalAppearanceMap[piece.AppearanceID].Prefab3D);
            new_piece.AddComponent<PieceControllerThreeD>();
            pieces.Add(piece.Position, new_piece);
            new_piece.transform.SetParent(squares[piece.Position.X, piece.Position.Y].transform);
            new_piece.transform.localPosition = new Vector3(0, 0.25f, 0);
        }

        /// <summary>
        /// Moves a 3D piece
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void Move(V2 from, V2 to)
        {
            bool jump = false;
            Vector2 delta = (to - from).Vector2();
            int count = (int)delta.magnitude;


            // Iterate through squares between start and end. If piece found between, jump
            for (int i = 0; i < count; i++)
            {
                Vector2 current_pos = from.Vector2() + (delta * (i / (float)count));
                V2 min_pos = new V2((int)Mathf.Floor(current_pos.x), (int)Mathf.Floor(current_pos.y));
                V2 max_pos = new V2((int)Mathf.RoundToInt(current_pos.x), (int)Mathf.RoundToInt(current_pos.y));
                if (min_pos != from && min_pos != to && pieces.ContainsKey(min_pos))
                {
                    jump = true; break;
                }
                if (max_pos != from && max_pos != to && pieces.ContainsKey(max_pos))
                {
                    jump = true; break;
                }
            }

            // Set piece as child of destination square
            pieces[from].transform.SetParent(squares[to.X, to.Y].transform);

            // Move piece
            pieces[from].GetComponent<PieceControllerThreeD>().Move(
                pieces[from].transform.localPosition,
                new Vector3(0, 0.25f, 0),
                jump, visualManager.pieceTravelTime, 2f);

            // Update piece dictionary
            pieces[to] = pieces[from];
            pieces.Remove(from);
        }

        /// <summary>
        /// Updates 3D piece appearance
        /// </summary>
        /// <param name="position"></param>
        public void UpdateAppearance(V2 position)
        {
            DestroyPiece(position, false);
            Create(visualManager.ChessManager.GameManager.Board.GetPiece(position));
        }

        /// <summary>
        /// Destroys a 3D piece
        /// </summary>
        /// <param name="position"></param>
        /// <param name="taken">Whether the piece take animation (ripple) should show</param>
        public void DestroyPiece(V2 position, bool taken = true)
        {
            Destroy(pieces[position]);
            pieces.Remove(position);
            if (taken) ripples.Add(new RippleData(position));
        }

        /// <summary>
        /// Clears all active ripples
        /// </summary>
        public void ClearRipples() => ripples.Clear();

        /// <summary>
        /// Shows a 3D square as being selected
        /// </summary>
        /// <param name="position"></param>
        /// <param name="visualManager"></param>
        private void SelectSquare(V2 position, VisualManager visualManager)
        {
            if (position == selected) selected = new V2(-1); // If already selected unselect
            else
            {
                // If clicking on a destination square make move
                if (visualManager.possibleMoves.FindIndex((m) => m.From == selected && m.To == position) != -1)
                {
                    visualManager.ChessManager.DoLocalMove(selected, position);
                    selected = new V2(-1);
                }
                else
                    selected = position;
            }
        }

        /// <summary>
        /// Highlights a square
        /// </summary>
        /// <param name="square"></param>
        private void HighlightSquare(V2 square)
        {
            highlightedSquares.Add(square);

            if (VisualManager.IsWhite(square))
            {
                if (pieces.ContainsKey(square)) squares[square.X, square.Y].material = white3DTakeHighlightMaterial;
                else squares[square.X, square.Y].material = white3DHighlightMaterial;
            }
            else
            {
                if (pieces.ContainsKey(square)) squares[square.X, square.Y].material = black3DTakeHighlightMaterial;
                else squares[square.X, square.Y].material = black3DHighlightMaterial;
            }
        }

        /// <summary>
        /// Clears all highlighted squares
        /// </summary>
        private void ClearHighlighted()
        {
            foreach (V2 square in highlightedSquares)
            {
                ResetSquareColor(square);
            }
            highlightedSquares.Clear();
        }

        /// <summary>
        /// Changes squares to show where last move came from
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void ShowLastMove(V2 from, V2 to)
        {
            // Clear moveFromSquare and moveToSquare before resetting to avoid rehighlighting
            V2 temp_place = moveFromSquare;
            moveFromSquare = new V2(-1);
            if (temp_place != new V2(-1)) ResetSquareColor(temp_place);
            temp_place = moveToSquare;
            moveToSquare = new V2(-1);
            if (temp_place != new V2(-1)) ResetSquareColor(temp_place);

            // Set colour
            if (VisualManager.IsWhite(from)) squares[from.X, from.Y].material = whiteMoveMaterial;
            else squares[from.X, from.Y].material = blackMoveMaterial;
            if (VisualManager.IsWhite(to)) squares[to.X, to.Y].material = whiteMoveMaterial;
            else squares[to.X, to.Y].material = blackMoveMaterial;

            // Update moves
            moveFromSquare = from;
            moveToSquare = to;
        }

        /// <summary>
        /// Updates VisualManagerThreeD
        /// </summary>
        public void ExternalUpdate()
        {
            V2? currentPosition = cameraManager.ExternalUpdate(boardRenderInfo.BoardSize);
            if (currentPosition is not null && !I.GetMouseButton(K.SecondaryClick))
            {
                if (I.GetMouseButtonDown(K.PrimaryClick)) SelectSquare((V2)currentPosition, visualManager);
                hoveringOver = (V2)currentPosition;
            }
            else
            {
                if (I.GetMouseButtonDown(K.PrimaryClick)) selected = new V2(-1);
                hoveringOver = new V2(-1);
            }

            UpdateDisplacementsAndHighlight();
        }

        /// <summary>
        /// Updates the heights and highlighting of all the squares on the 3D chess board
        /// </summary>
        private void UpdateDisplacementsAndHighlight()
        {
            // Set new ripple progress
            for (int i = 0; i < ripples.Count; i++)
            {
                ripples[i].time += Time.deltaTime / 2f;

                if (ripples[i].time >= 1f)
                {
                    ripples.RemoveAt(i);
                    i--;
                }
            }

            targetDisplacement = new float[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];
            highlighted = new bool[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];

            // Raise square hovered over
            if (hoveringOver.X != -1)
            {
                targetDisplacement[hoveringOver.X, hoveringOver.Y] += 0.2f;
            }

            // Raise and highlight selected square
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

            // Apply ripple height
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

            // Applies highlights and displacement
            ClearHighlighted();
            for (int x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (int y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    // Smoothly move to target position using log function
                    float target_delta = targetDisplacement[x, y] + 1 - squares[x, y].transform.position.y;
                    if (target_delta == 0) { return; }
                    squares[x, y].transform.position += Vector3.up * Mathf.Log(target_delta, 2) * Time.deltaTime * 10;
                    squares[x, y].transform.position = new Vector3(squares[x, y].transform.position.x, Mathf.Clamp(squares[x, y].transform.position.y, -10, 10), squares[x, y].transform.position.z);
                    
                    // Highlight squares
                    if (highlighted[x, y]) HighlightSquare(new V2(x, y));
                }
            }
        }
    }
}