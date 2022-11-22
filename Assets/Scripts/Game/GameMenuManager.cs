using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameMenuManager : MonoBehaviour
    {
        public GameObject EscapeMenu;
        public TMP_InputField FileNameInput;
        public TMP_Text SaveStatusText;

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

        public void Save()
        {
            string fileName = FileNameInput.text;
            string status = Validators.ValidateFileName(fileName);
            if (status is not null)
            {
                SaveStatusText.text = status;
                return;
            }

            fileName += " - " + chessManager.GameManager.GameManagerData.GetName();
            status = chessManager.Save(fileName);
            if (status is not null)
            {
                SaveStatusText.text = status;
                return;
            }
            SaveStatusText.text = "Save successful";
        }

        public void OpenSaveLocation()
        {
            SaveSystem.OpenSavesFolder();
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