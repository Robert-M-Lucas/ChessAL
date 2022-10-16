using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles inputs in the main scene
/// </summary>
public class InputManager : MonoBehaviour
{
    private List<Move> possibleMoves = new List<Move>();

    private VisualManager visualManager;

    public void Start()
    {
        visualManager = FindObjectOfType<VisualManager>();
    }

    /// <summary>
    /// Updates the list of moves the player can make
    /// </summary>
    /// <param name="possibleMoves"></param>
    public void SetPossibleMoves(List<Move> possibleMoves)
    {
        this.possibleMoves = possibleMoves;
    }

    public void OnPieceClick(V2 position)
    {
        visualManager.ShowMoves(position);
    }

    private void Update()
    {

    }
}
