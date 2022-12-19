using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject EscapeMenu;
        [SerializeField] private TMP_InputField FileNameInput;
        [SerializeField] private TMP_Text SaveStatusText;

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

            // Check file name
            string status = Validators.ValidateFileName(fileName);
            if (status is not null)
            {
                SaveStatusText.text = status;
                return;
            }

            // fileName += " - " + chessManager.GameManager.GameManagerData.GetName();

            status = chessManager.Save(fileName);
            if (status is not null)
            {
                SaveStatusText.text = status;
                return;
            }
            SaveStatusText.text = "Save successful";
        }

        public void OpenSaveLocation() => SaveSystem.OpenSavesFolder();

        public void ShowHelp() => HelpSystem.OpenHelp(HelpSystem.IN_GAME_PAGE_NAME);

        // Update is called once per frame
        void Update()
        {
            // Toggle escape menu
            if (I.GetKeyDown(K.EscapeKey))
            {
                showingEscapeMenu = !showingEscapeMenu;
                EscapeMenu.SetActive(showingEscapeMenu);
            }
        }
    }
}