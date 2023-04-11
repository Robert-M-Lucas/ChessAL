using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class RenderedCellData : MonoBehaviour
    {
        public V2 Position;
        [FormerlySerializedAs("inputManager")] public InputManager InputManager;

        // Called when clicked on
        public void ActivateInputManager()
        {
            InputManager.OnCellClick(Position);
        }
    }

}