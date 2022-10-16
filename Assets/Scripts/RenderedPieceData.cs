using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderedPieceData : MonoBehaviour
{
    public V2 Position;
    public InputManager inputManager;

    public void ActivateInputManager()
    {
        inputManager.OnPieceClick(Position);
    }
}
