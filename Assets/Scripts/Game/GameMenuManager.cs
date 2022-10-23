using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameMenuManager : MonoBehaviour
    {
        public GameObject EscapeMenu;

        private bool showingEscapeMenu = false;
        private ChessManager chessManager;

        void Start()
        {
            chessManager = FindObjectOfType<ChessManager>();
        }

        public void Resume()
        {
            showingEscapeMenu = false;
            EscapeMenu.SetActive(false);
        }

        public void ExitToMenu()
        {
            chessManager.ExitGame();
        }

        // Update is called once per frame
        void Update()
        {
            if (I.GetKeyDown(K.EscapeKey))
            {
                showingEscapeMenu = !showingEscapeMenu;
                EscapeMenu.SetActive(showingEscapeMenu);
            }
        }
    }
}