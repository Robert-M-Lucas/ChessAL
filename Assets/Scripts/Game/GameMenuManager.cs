using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.UI
{
    public class GameMenuManager : MonoBehaviour
    {
        [FormerlySerializedAs("EscapeMenu")] [SerializeField] private GameObject escapeMenu;
        [FormerlySerializedAs("FileNameInput")] [SerializeField] private TMP_InputField fileNameInput;
        [FormerlySerializedAs("SaveStatusText")] [SerializeField] private TMP_Text saveStatusText;

        public bool ShowingEscapeMenu = false;
        private ChessManager chessManager;

        void Start()
        {
            chessManager = FindObjectOfType<ChessManager>();
        }

        public void Resume()
        {
            ShowingEscapeMenu = false;
            escapeMenu.SetActive(false);
        }

        public void ExitToMenu()
        {
            chessManager.ExitGame();
        }

        public void Save()
        {
            var file_name = fileNameInput.text;

            // Check file name
            var status = Validators.ValidateFileName(file_name);
            if (status is not null)
            {
                saveStatusText.text = status;
                return;
            }

            // fileName += " - " + chessManager.GameManager.GameManagerData.GetName();

            status = chessManager.Save(file_name);
            if (status is not null)
            {
                saveStatusText.text = status;
                return;
            }
            saveStatusText.text = "Save successful";
        }

        public void OpenSaveLocation() => SaveSystem.OpenSavesFolder();

        public void ShowHelp() => HelpSystem.OpenHelp(HelpSystem.IN_GAME_PAGE_NAME);

        // Update is called once per frame
        void Update()
        {
            // Toggle escape menu
            if (I.GetKeyDown(K.EscapeKey))
            {
                ShowingEscapeMenu = !ShowingEscapeMenu;
                escapeMenu.SetActive(ShowingEscapeMenu);
            }
        }
    }
}