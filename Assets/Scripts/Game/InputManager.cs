using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles inputs in the main scene
/// </summary>
public class InputManager : MonoBehaviour
{
    private List<Move> possibleMoves = new List<Move>();

    private VisualManager visualManager;

    private bool showingMoves = false;
    private V2? selectedPiece = null;
    
    private ChessManager chessManager;

    public void Start()
    {
        visualManager = FindObjectOfType<VisualManager>();
        chessManager = FindObjectOfType<ChessManager>();
    }

    /// <summary>
    /// Updates the list of moves the player can make
    /// </summary>
    /// <param name="possibleMoves"></param>
    public void SetPossibleMoves(List<Move> possibleMoves)
    {
        this.possibleMoves = possibleMoves;
    }

    public void OnCellClick(V2 position)
    {
        Debug.Log("Cell click");
        if (!chessManager.MyTurn)
        { 
            visualManager.HideMoves(); 
            return; 
        }

        Debug.Log("Showing moves");

        if (showingMoves)
        {
            foreach (Move m in possibleMoves)
            {
                if ((V2)selectedPiece == m.From && position == m.To)
                {
                    chessManager.GetLocalMove(m.From, m.To);
                    visualManager.ToggleShowMoves(m.From);
                    return;
                }
            }
        }

        showingMoves = visualManager.ToggleShowMoves(position);
        if (showingMoves) selectedPiece = position;
    }

    private void Update()
    {

    }
}
