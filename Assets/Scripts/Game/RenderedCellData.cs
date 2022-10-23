using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RenderedCellData : MonoBehaviour
    {
        public V2 Position;
        public InputManager inputManager;

        public void ActivateInputManager()
        {
            inputManager.OnCellClick(Position);
        }
    }

}