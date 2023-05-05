using UnityEngine;

namespace Game.UI
{
    public class PerspectiveTextController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (!FindObjectOfType<ChessManager>().GameManager.Board.GetBoardRenderInfo().Allows3D) gameObject.SetActive(false);
        }
    }
}