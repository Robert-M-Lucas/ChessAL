using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections.Concurrent;
using UnityEngine.UI;
using System;
using Networking.Client;
using Gamemodes;
using System.Linq;

#nullable enable

namespace MainMenu
{
    /// <summary>
    /// Manages the UI on the main menu
    /// </summary>
    public class MenuUIManager : MonoBehaviour
    {
        #region GameObject References
        [Header("Host")]
        [SerializeField] private GameObject HostConfigScreen = default!;
        private bool showingHostSettings = false;
        [SerializeField] private TMP_InputField HostNameInput = default!;
        [SerializeField] private TMP_Text HostNameDisallowedReason = default!;
        [SerializeField] private TMP_InputField HostPasswordInput = default!;
        [SerializeField] private GamemodeSelector HostGamemodeSelector = default!;
        [SerializeField] private TMP_Text HostConfigHelpText = default!;
        [SerializeField] private TMP_Text HostStatusText = default!;
        [SerializeField] private TMP_Text HostScreenDescriptionText = default!;
        [SerializeField] private SaveSelector HostSaveInput = default!;
        [SerializeField] private Button HostStartButton = default!;
        [SerializeField] private GameObject HostScreen = default!;
        [SerializeField] private TMP_Text HostStartFailText = default!;
        private bool showingHostScreen = false;

        [Header("Join")]
        [SerializeField] private GameObject JoinConfigScreen = default!;
        private bool showingJoinSettings = false;
        [SerializeField] private TMP_InputField JoinIpInput = default!;
        [SerializeField] private TMP_InputField JoinNameInput = default!;
        [SerializeField] private TMP_Text JoinNameDisallowedReason = default!;
        [SerializeField] private TMP_InputField JoinPasswordInput = default!;
        [SerializeField] private GameObject JoinScreen = default!;
        private bool showingJoinScreen = false;
        [SerializeField] private TMP_Text JoinStatusText = default!;
        [SerializeField] private TMP_Text JoinScreenDescriptionText = default!;

        [Header("Local")]
        [SerializeField] private GameObject LocalConfigScreen = default!;
        private bool showingLocalSettings = false;
        [SerializeField] private GamemodeSelector LocalGamemodeSelector = default!;
        [SerializeField] private TMP_Text LocalConfigHelpText = default!;
        [SerializeField] private TMP_Text LocalScreenDescriptionText = default!;
        [SerializeField] private SaveSelector LocalSaveInput = default!;
        [SerializeField] private GameObject LocalScreen = default!;
        [SerializeField] private TMP_Text LocalStartFailText = default!;
        private bool showingLocalScreen = false;
        [SerializeField] private TMP_InputField AITurnTime = default!;

        [Header("Other")]
        [SerializeField] private GameObject LobbyDisplay = default!;
        [SerializeField] private PlayerCardController PlayerCardPrefab = default!;
        // [SerializeField] private GameObject ShuttingDownServerScreen = default!;

        #endregion

        private ChessManager chessManager = default!;

        private List<PlayerCardController> playerCardControllers = new List<PlayerCardController>();
        private Dictionary<string, AbstractGameManagerData> gamemodes = new Dictionary<string, AbstractGameManagerData>();

        void Start()
        {
            chessManager = FindObjectOfType<ChessManager>();

            // Populate dropdowns with gamemode options
            List<AbstractGameManagerData> gamemode_data = Util.GetAllGameManagers();
            foreach (AbstractGameManagerData game in gamemode_data)
            {
                gamemodes.Add(game.GetName(), game);
                HostGamemodeSelector.Options.Add(game.GetName());
                LocalGamemodeSelector.Options.Add(game.GetName());
            }

            HideAllScreens();
        }
        
        /// <summary>
        /// Hides all currently showing main menu screens
        /// </summary>
        public void HideAllScreens()
        {
            showingHostScreen = false;
            showingHostSettings = false;
            showingJoinScreen = false;
            showingJoinSettings = false;
            showingLocalScreen = false;
            showingLocalSettings = false;

            LobbyDisplay.SetActive(false);
            HostConfigScreen.SetActive(false);
            JoinConfigScreen.SetActive(false);
            LocalConfigScreen.SetActive(false);
            HostScreen.SetActive(false);
            JoinScreen.SetActive(false);
            LocalScreen.SetActive(false);
            HostStartFailText.text = string.Empty;
            LocalStartFailText.text = string.Empty;
            HostNameDisallowedReason.text = string.Empty;
            JoinNameDisallowedReason.text = string.Empty;
        }

        public void OpenSavesFolder() => SaveSystem.OpenSavesFolder();

        /// <summary>
        /// Shows help for the current gamemode
        /// </summary>
        public void ShowGamemodeHelp() => HelpSystem.OpenHelp(chessManager.CurrentGameManager.GetUID());

        /// <summary>
        /// Shows general help (not for specific gamemode)
        /// </summary>
        public void ShowHelp() => HelpSystem.OpenHelp();

        public void ShowJoinHelp() => HelpSystem.OpenHelp("joining");

        public void WhichIPHelp() => HelpSystem.OpenHelp("hosting");

        /// <summary>
        /// Fully exits the game
        /// </summary>
        public void Quit()
        {
            chessManager.StopNetworking();
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        /*
         * [Type]Config - Opens configuration screen for type
         * [Type]ShowGamemodeHelp - Shows gamemode help for the currently selected gamemode on that config screen
         * Full[Type] - Puts player into lobby of that type
         */

        #region Host
        public void HostConfig()
        {
            if (showingJoinSettings || showingJoinScreen || showingHostScreen || showingLocalSettings || showingLocalScreen) return;

            HideAllScreens();
            showingHostSettings = true;
            HostConfigScreen.SetActive(true);
        }

        public void OnHostGamemodeSwitch()
        {
            if (HostGamemodeSelector.CurrentlyShowingPos == 0) HostConfigHelpText.text = "";
            HostConfigHelpText.text = gamemodes[HostGamemodeSelector.CurrentlyShowing].GetDescription();
        }

        public void HostShowGamemodeHelp()
        {
            if (HostGamemodeSelector.CurrentlyShowingPos == 0) return;

            HelpSystem.OpenHelp(gamemodes[HostGamemodeSelector.CurrentlyShowing].GetUID());
            HostConfigHelpText.text = gamemodes[HostGamemodeSelector.CurrentlyShowing].GetDescription();
        }

        public void LoadSaveAndFullHost()
        {
            if (HostSaveInput.SelectedFile == string.Empty) return;

            byte[] save_data = chessManager.LoadSave(HostSaveInput.SelectedFile);
            int gamemode = SerialisationUtil.GetGamemodeUID(save_data);
            FullHost(save_data, gamemode);
        }

        public void FullHost() => FullHost(null, null);

        public void FullHost(byte[]? saveData, int? gameMode)
        {
            string? validation_result = Validators.ValidatePlayerName(HostNameInput.text);
            if (validation_result is not null)
            {
                HostNameDisallowedReason.text = "[Name] " + validation_result;
                return;
            }

            validation_result = Validators.ValidatePassword(HostPasswordInput.text);
            if (validation_result is not null)
            {
                HostNameDisallowedReason.text = "[Password] " + validation_result;
                return;
            }

            HostSettings host_settings;
            if (saveData is null)
            {
                if (HostGamemodeSelector.CurrentlyShowingPos == 0) return;

                host_settings = new HostSettings(gamemodes[HostGamemodeSelector.CurrentlyShowing], HostPasswordInput.text, HostNameInput.text, null);
            }
            else
            {
                AbstractGameManagerData? game_data_selected = null;

                foreach (AbstractGameManagerData game_data in chessManager.GameManagersData)
                {
                    if (game_data.GetUID() == gameMode) game_data_selected = game_data;
                }

                if (game_data_selected is null)
                {
                    HostNameDisallowedReason.text = "Gamemode in save file no longer exists";
                    return;
                }

                host_settings = new HostSettings(game_data_selected, HostPasswordInput.text, HostNameInput.text, saveData);
            }

            

            bool half_success = chessManager.Host(host_settings);
            if (!half_success)
            {
                HostNameDisallowedReason.text = "Host failed. This can be caused by trying to restart a host too quickly - try waiting a couple of minutes";
                return;
            }

            HideAllScreens();

            HostStatusText.text = "Connecting to internal server...";
            HostScreen.SetActive(true);
            showingHostScreen = true;
            LobbyDisplay.gameObject.SetActive(true);
            HostStatusText.gameObject.SetActive(true);
            HostScreenDescriptionText.text = "Gamemode description:\n" + host_settings.GameMode.GetDescription();
        }

        public void HostConnectionSuccessful()
        {
            HostStatusText.text += " SUCCESS";
            HostStatusText.gameObject.SetActive(false);
            HostStartButton.gameObject.SetActive(true);
        }

        public void HostFailed(string reason)
        {
            HostStatusText.text += " FAILED\n" + reason;
        }

        public void CancelHost()
        {
            UpdateLobbyPlayerCardDisplay(new ConcurrentDictionary<int, ClientPlayerData>());
            HideAllScreens();
            chessManager.RestartNetworking();

            // ShuttingDownServerScreen.SetActive(true);
            // StartCoroutine(WaitForServerShutdown());
        }

        /*
        /// <summary>
        /// Shows a screen while waiting for the server port to be freed
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForServerShutdown()
        {
            yield return new WaitForSecondsRealtime(5);
            ShuttingDownServerScreen.SetActive(false);
        }
        */

        public void HostStartGame() => chessManager.HostStartGame();

        public void HostStartGameFailed(string reason)
        {
            HostStartFailText.text = reason;
            Debug.Log(reason);
        }
        #endregion

        #region Join
        public void JoinConfig()
        {
            if (showingHostSettings || showingHostScreen || showingJoinScreen || showingLocalSettings || showingLocalScreen) return;

            HideAllScreens();
            showingJoinSettings = true;
            JoinConfigScreen.SetActive(true);
        }

        public void FullJoin()
        {
            // Check for invalid player name
            string? validation_result = Validators.ValidatePlayerName(JoinNameInput.text);
            if (validation_result is not null)
            {
                JoinNameDisallowedReason.text = "Name: " + validation_result;
                return;
            }

            JoinSettings join_settings = new JoinSettings(JoinIpInput.text, JoinPasswordInput.text, JoinNameInput.text);

            // Join
            chessManager.Join(join_settings);

            HideAllScreens();

            // Configure screens
            JoinStatusText.text = "Connecting to server...";
            showingJoinSettings = false;
            JoinScreen.SetActive(true);
            showingJoinScreen = true;
            LobbyDisplay.gameObject.SetActive(true);
            JoinStatusText.gameObject.SetActive(true);
        }

        public void JoinConnectionSuccessful()
        {
            JoinStatusText.text += " SUCCESS\n" + "Recieving gamemode and savedata...";
        }

        public void OnGamemodeDataRecieve()
        {
            JoinStatusText.text += " SUCCESS";
            JoinStatusText.gameObject.SetActive(false);
            JoinScreenDescriptionText.text = "Gamemode description:\n" + chessManager.CurrentGameManager.GetDescription();
        }

        public void JoinFailed(string reason)
        {
            JoinStatusText.text += " FAILED\n" + reason;
        }

        public void ClientKicked(string reason)
        {
            JoinStatusText.text += "\nClient Kicked: " + reason;
        }

        public void CancelJoin()
        {
            HideAllScreens();
            UpdateLobbyPlayerCardDisplay(new()); // Empty player card display
            chessManager.RestartNetworking();
        }
        #endregion

        #region Local Play
        public void LocalPlayConfig()
        {
            if (showingJoinSettings || showingJoinScreen || showingHostScreen || showingHostSettings || showingLocalScreen) return;

            HideAllScreens();
            showingLocalSettings = true;
            LocalConfigScreen.SetActive(true);
        }

        public void OnLocalGamemodeSwitch()
        {
            if (LocalGamemodeSelector.CurrentlyShowingPos == 0) LocalConfigHelpText.text = "";
            LocalConfigHelpText.text = gamemodes[LocalGamemodeSelector.CurrentlyShowing].GetDescription();
        }

        public void LocalPlayShowGamemodeHelp()
        {
            if (LocalGamemodeSelector.CurrentlyShowingPos == 0) return; // No gamemode selected

            HelpSystem.OpenHelp(gamemodes[LocalGamemodeSelector.CurrentlyShowing].GetUID()); // Show help for current gamemode
            LocalConfigHelpText.text = gamemodes[LocalGamemodeSelector.CurrentlyShowing].GetDescription(); // Show backup description
        }

        public void LoadSaveAndFullLocalPlay()
        {
            if (LocalSaveInput.SelectedFile == string.Empty) return; // No save selected

            byte[] save_data = chessManager.LoadSave(LocalSaveInput.SelectedFile);
            int gamemode = SerialisationUtil.GetGamemodeUID(save_data); // Extract gamemode
            FullLocalPlay(save_data, gamemode);
        }

        public void FullLocalPlay() => FullLocalPlay(null, null); // Local play with no save

        public void FullLocalPlay(byte[]? saveData, int? gameMode)
        {
            HostSettings host_settings;
            if (saveData is null)
            {
                if (LocalGamemodeSelector.CurrentlyShowingPos == 0) return;

                host_settings = new HostSettings(gamemodes[LocalGamemodeSelector.CurrentlyShowing], "", "", null);
            }
            else
            {
                AbstractGameManagerData? game_data_selected = null;

                foreach (AbstractGameManagerData game_data in chessManager.GameManagersData)
                {
                    if (game_data.GetUID() == gameMode) game_data_selected = game_data;
                }

                if (game_data_selected is null)
                {
                    return;
                }

                host_settings = new HostSettings(game_data_selected, "", "", saveData);
            }

            chessManager.PrepLocal(host_settings); // Prepare for local play

            HideAllScreens();

            showingLocalScreen = true;
            LobbyDisplay.gameObject.SetActive(true);
            LocalScreen.SetActive(true);
            LocalScreenDescriptionText.text = "Gamemode description:\n" + host_settings.GameMode.GetDescription();
        }

        public void AddLocalPlayer() =>  chessManager.AddLocalPlayer();
        public void AddLocalAI() => chessManager.AddLocalAI();
        public void RemoveLocalPlayer() => chessManager.RemoveLocalPlayer();
        public void RemoveLocalAI() => chessManager.RemoveLocalAI();
        public void StartLocalGame()
        {
            int AI_turn_time = 20;
            // int.TryParse(AITurnTime.text, out AI_turn_time);
            string? output = chessManager.StartLocalGame(AI_turn_time);
            if (output is not null) LocalStartFailText.text = output; // Start failed
        }

        public void CancelLocalPlay()
        {
            UpdateLobbyPlayerCardDisplay(new()); // Empty display
            chessManager.ResetLocalSettings();
            HideAllScreens();
        }

        #endregion

        private void ForceUpdateLobby() => UpdateLobbyPlayerCardDisplay(chessManager.GetPlayerList());

        /// <summary>
        /// Updates player cards shown in a lobby
        /// </summary>
        /// <param name="playerData"></param>
        public void UpdateLobbyPlayerCardDisplay(ConcurrentDictionary<int, ClientPlayerData> playerData)
        {
            // Prevents errors caused by player data being sent too quickly
            if (chessManager.CurrentGameManager is null && playerData.Count != 0)
            {
                Invoke("ForceUpdateLobby", 1f); // Run again in 1s
                return;
            }

            List<PlayerCardController> to_remove = new List<PlayerCardController>();

            foreach (PlayerCardController player_card in playerCardControllers)
            {
                if (!playerData.ContainsKey(player_card.PlayerID))
                {
                    Destroy(player_card.gameObject);
                    to_remove.Add(player_card); // Queue removal
                }
                else // Update card
                {
                    ClientPlayerData player_data = playerData[player_card.PlayerID];
                    player_card.PlayerName = player_data.Name;
                    player_card.Team = player_data.Team;
                    player_card.PlayerOnTeam = player_data.PlayerInTeam;
                    player_card.UpdateFields();
                }
            }

            // Remove
            foreach (PlayerCardController player_card in to_remove) playerCardControllers.Remove(player_card);


            foreach (ClientPlayerData player_data in playerData.Values)
            {
                bool shown = false;
                foreach (PlayerCardController player_card in playerCardControllers)
                {
                    if (player_card.PlayerID == player_data.PlayerID)
                    {
                        shown = true;
                        break;
                    }
                }

                if (!shown) CreatePlayerCard(player_data); // Create missing cards
            }
        }

        /// <summary>
        /// Creates a new player card
        /// </summary>
        /// <param name="playerData"></param>
        private void CreatePlayerCard(ClientPlayerData playerData)
        {
            GameObject new_card = Instantiate(PlayerCardPrefab.gameObject);
            new_card.SetActive(true);
            PlayerCardController card_controller = new_card.GetComponent<PlayerCardController>();

            card_controller.PlayerID = playerData.PlayerID;
            card_controller.PlayerName = playerData.Name;
            card_controller.Team = playerData.Team;
            card_controller.PlayerOnTeam = playerData.PlayerInTeam;
            card_controller.ChessManager = chessManager;
            card_controller.MenuUIManager = this;

            new_card.transform.SetParent(PlayerCardPrefab.transform.parent);
            RectTransform rectTransform = new_card.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * 0.07f);

            card_controller.UpdateFields();

            playerCardControllers.Add(card_controller); 
        }
    }
}