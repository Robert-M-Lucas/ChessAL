using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections.Concurrent;
using UnityEngine.UI;
using Networking.Client;
using Gamemodes;
using UnityEngine.Serialization;

#nullable enable

namespace MainMenu
{
    /// <summary>
    /// Manages the UI on the main menu
    /// </summary>
    public class MenuUIManager : MonoBehaviour
    {
        #region GameObject References
        [FormerlySerializedAs("HostConfigScreen")]
        [Header("Host")]
        [SerializeField] private GameObject hostConfigScreen = default!;
        private bool showingHostSettings = false;
        [FormerlySerializedAs("HostNameInput")] [SerializeField] private TMP_InputField hostNameInput = default!;
        [FormerlySerializedAs("HostNameDisallowedReason")] [SerializeField] private TMP_Text hostNameDisallowedReason = default!;
        [FormerlySerializedAs("HostPasswordInput")] [SerializeField] private TMP_InputField hostPasswordInput = default!;
        [FormerlySerializedAs("HostGamemodeSelector")] [SerializeField] private GamemodeSelector hostGamemodeSelector = default!;
        [FormerlySerializedAs("HostConfigHelpText")] [SerializeField] private TMP_Text hostConfigHelpText = default!;
        [FormerlySerializedAs("HostStatusText")] [SerializeField] private TMP_Text hostStatusText = default!;
        [FormerlySerializedAs("HostScreenDescriptionText")] [SerializeField] private TMP_Text hostScreenDescriptionText = default!;
        [FormerlySerializedAs("HostSaveInput")] [SerializeField] private SaveSelector hostSaveInput = default!;
        [FormerlySerializedAs("HostStartButton")] [SerializeField] private Button hostStartButton = default!;
        [FormerlySerializedAs("HostScreen")] [SerializeField] private GameObject hostScreen = default!;
        [FormerlySerializedAs("HostStartFailText")] [SerializeField] private TMP_Text hostStartFailText = default!;
        private bool showingHostScreen = false;

        [FormerlySerializedAs("JoinConfigScreen")]
        [Header("Join")]
        [SerializeField] private GameObject joinConfigScreen = default!;
        private bool showingJoinSettings = false;
        [FormerlySerializedAs("JoinIpInput")] [SerializeField] private TMP_InputField joinIpInput = default!;
        [FormerlySerializedAs("JoinNameInput")] [SerializeField] private TMP_InputField joinNameInput = default!;
        [FormerlySerializedAs("JoinNameDisallowedReason")] [SerializeField] private TMP_Text joinNameDisallowedReason = default!;
        [FormerlySerializedAs("JoinPasswordInput")] [SerializeField] private TMP_InputField joinPasswordInput = default!;
        [FormerlySerializedAs("JoinScreen")] [SerializeField] private GameObject joinScreen = default!;
        private bool showingJoinScreen = false;
        [FormerlySerializedAs("JoinStatusText")] [SerializeField] private TMP_Text joinStatusText = default!;
        [FormerlySerializedAs("JoinScreenDescriptionText")] [SerializeField] private TMP_Text joinScreenDescriptionText = default!;

        [FormerlySerializedAs("LocalConfigScreen")]
        [Header("Local")]
        [SerializeField] private GameObject localConfigScreen = default!;
        private bool showingLocalSettings = false;
        [FormerlySerializedAs("LocalGamemodeSelector")] [SerializeField] private GamemodeSelector localGamemodeSelector = default!;
        [FormerlySerializedAs("LocalConfigHelpText")] [SerializeField] private TMP_Text localConfigHelpText = default!;
        [FormerlySerializedAs("LocalScreenDescriptionText")] [SerializeField] private TMP_Text localScreenDescriptionText = default!;
        [FormerlySerializedAs("LocalSaveInput")] [SerializeField] private SaveSelector localSaveInput = default!;
        [FormerlySerializedAs("LocalScreen")] [SerializeField] private GameObject localScreen = default!;
        [FormerlySerializedAs("LocalStartFailText")] [SerializeField] private TMP_Text localStartFailText = default!;
        private bool showingLocalScreen = false;
        // ReSharper disable once NotAccessedField.Local
        [FormerlySerializedAs("AITurnTime")] [SerializeField] private TMP_InputField aiTurnTime = default!;

        [FormerlySerializedAs("LobbyDisplay")]
        [Header("Other")]
        [SerializeField] private GameObject lobbyDisplay = default!;
        [FormerlySerializedAs("PlayerCardPrefab")] [SerializeField] private PlayerCardController playerCardPrefab = default!;
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
            foreach (var game in gamemode_data)
            {
                gamemodes.Add(game.GetName(), game);
                hostGamemodeSelector.Options.Add(game.GetName());
                localGamemodeSelector.Options.Add(game.GetName());
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

            lobbyDisplay.SetActive(false);
            hostConfigScreen.SetActive(false);
            joinConfigScreen.SetActive(false);
            localConfigScreen.SetActive(false);
            hostScreen.SetActive(false);
            joinScreen.SetActive(false);
            localScreen.SetActive(false);
            hostStartFailText.text = string.Empty;
            localStartFailText.text = string.Empty;
            hostNameDisallowedReason.text = string.Empty;
            joinNameDisallowedReason.text = string.Empty;
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
            hostConfigScreen.SetActive(true);
        }

        public void OnHostGamemodeSwitch()
        {
            if (hostGamemodeSelector.CurrentlyShowingPos == 0) hostConfigHelpText.text = "";
            hostConfigHelpText.text = gamemodes[hostGamemodeSelector.CurrentlyShowing].GetDescription();
        }

        public void HostShowGamemodeHelp()
        {
            if (hostGamemodeSelector.CurrentlyShowingPos == 0) return;

            HelpSystem.OpenHelp(gamemodes[hostGamemodeSelector.CurrentlyShowing].GetUID());
            hostConfigHelpText.text = gamemodes[hostGamemodeSelector.CurrentlyShowing].GetDescription();
        }

        public void LoadSaveAndFullHost()
        {
            if (hostSaveInput.SelectedFile == string.Empty) return;

            var save_data = chessManager.LoadSave(hostSaveInput.SelectedFile);
            var gamemode = SerialisationUtil.GetGamemodeUID(save_data);
            FullHost(save_data, gamemode);
        }

        public void FullHost() => FullHost(null, null);

        public void FullHost(byte[]? saveData, int? gameMode)
        {
            var validation_result = Validators.ValidatePlayerName(hostNameInput.text);
            if (validation_result is not null)
            {
                hostNameDisallowedReason.text = "[Name] " + validation_result;
                return;
            }

            validation_result = Validators.ValidatePassword(hostPasswordInput.text);
            if (validation_result is not null)
            {
                hostNameDisallowedReason.text = "[Password] " + validation_result;
                return;
            }

            HostSettings host_settings;
            if (saveData is null)
            {
                if (hostGamemodeSelector.CurrentlyShowingPos == 0) return;

                host_settings = new HostSettings(gamemodes[hostGamemodeSelector.CurrentlyShowing], hostPasswordInput.text, hostNameInput.text, null);
            }
            else
            {
                AbstractGameManagerData? game_data_selected = null;

                foreach (var game_data in chessManager.GameManagersData)
                {
                    if (game_data.GetUID() == gameMode) game_data_selected = game_data;
                }

                if (game_data_selected is null)
                {
                    hostNameDisallowedReason.text = "Gamemode in save file no longer exists";
                    return;
                }

                host_settings = new HostSettings(game_data_selected, hostPasswordInput.text, hostNameInput.text, saveData);
            }

            

            var half_success = chessManager.Host(host_settings);
            if (!half_success)
            {
                hostNameDisallowedReason.text = "Host failed. This can be caused by trying to restart a host too quickly - try waiting a couple of minutes";
                return;
            }

            HideAllScreens();

            hostStatusText.text = "Connecting to internal server...";
            hostScreen.SetActive(true);
            showingHostScreen = true;
            lobbyDisplay.gameObject.SetActive(true);
            hostStatusText.gameObject.SetActive(true);
            hostScreenDescriptionText.text = "Gamemode description:\n" + host_settings.GameMode.GetDescription();
        }

        public void HostConnectionSuccessful()
        {
            hostStatusText.text += " SUCCESS";
            hostStatusText.gameObject.SetActive(false);
            hostStartButton.gameObject.SetActive(true);
        }

        public void HostFailed(string reason)
        {
            hostStatusText.text += " FAILED\n" + reason;
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
            hostStartFailText.text = reason;
            Debug.Log(reason);
        }
        #endregion

        #region Join
        public void JoinConfig()
        {
            if (showingHostSettings || showingHostScreen || showingJoinScreen || showingLocalSettings || showingLocalScreen) return;

            HideAllScreens();
            showingJoinSettings = true;
            joinConfigScreen.SetActive(true);
        }

        public void FullJoin()
        {
            // Check for invalid player name
            var validation_result = Validators.ValidatePlayerName(joinNameInput.text);
            if (validation_result is not null)
            {
                joinNameDisallowedReason.text = "Name: " + validation_result;
                return;
            }

            var join_settings = new JoinSettings(joinIpInput.text, joinPasswordInput.text, joinNameInput.text);

            // Join
            chessManager.Join(join_settings);

            HideAllScreens();

            // Configure screens
            joinStatusText.text = "Connecting to server...";
            showingJoinSettings = false;
            joinScreen.SetActive(true);
            showingJoinScreen = true;
            lobbyDisplay.gameObject.SetActive(true);
            joinStatusText.gameObject.SetActive(true);
        }

        public void JoinConnectionSuccessful()
        {
            joinStatusText.text += " SUCCESS\n" + "Recieving gamemode and savedata...";
        }

        public void OnGamemodeDataRecieve()
        {
            joinStatusText.text += " SUCCESS";
            joinStatusText.gameObject.SetActive(false);
            joinScreenDescriptionText.text = "Gamemode description:\n" + chessManager.CurrentGameManager.GetDescription();
        }

        public void JoinFailed(string reason)
        {
            joinStatusText.text += " FAILED\n" + reason;
        }

        public void ClientKicked(string reason)
        {
            joinStatusText.text += "\nClient Kicked: " + reason;
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
            localConfigScreen.SetActive(true);
        }

        public void OnLocalGamemodeSwitch()
        {
            if (localGamemodeSelector.CurrentlyShowingPos == 0) localConfigHelpText.text = "";
            else localConfigHelpText.text = gamemodes[localGamemodeSelector.CurrentlyShowing].GetDescription();
        }

        public void LocalPlayShowGamemodeHelp()
        {
            if (localGamemodeSelector.CurrentlyShowingPos == 0) return; // No gamemode selected

            HelpSystem.OpenHelp(gamemodes[localGamemodeSelector.CurrentlyShowing].GetUID()); // Show help for current gamemode
            localConfigHelpText.text = gamemodes[localGamemodeSelector.CurrentlyShowing].GetDescription(); // Show backup description
        }

        public void LoadSaveAndFullLocalPlay()
        {
            if (localSaveInput.SelectedFile == string.Empty) return; // No save selected

            var save_data = chessManager.LoadSave(localSaveInput.SelectedFile);
            var gamemode = SerialisationUtil.GetGamemodeUID(save_data); // Extract gamemode
            FullLocalPlay(save_data, gamemode);
        }

        public void FullLocalPlay() => FullLocalPlay(null, null); // Local play with no save

        public void FullLocalPlay(byte[]? saveData, int? gameMode)
        {
            HostSettings host_settings;
            if (saveData is null)
            {
                if (localGamemodeSelector.CurrentlyShowingPos == 0) return;

                host_settings = new HostSettings(gamemodes[localGamemodeSelector.CurrentlyShowing], "", "", null);
            }
            else
            {
                AbstractGameManagerData? game_data_selected = null;

                foreach (var game_data in chessManager.GameManagersData)
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
            lobbyDisplay.gameObject.SetActive(true);
            localScreen.SetActive(true);
            localScreenDescriptionText.text = "Gamemode description:\n" + host_settings.GameMode.GetDescription();
        }

        public void AddLocalPlayer() =>  chessManager.AddLocalPlayer();
        public void AddLocalAI() => chessManager.AddLocalAI();
        public void RemoveLocalPlayer() => chessManager.RemoveLocalPlayer();
        public void RemoveLocalAI() => chessManager.RemoveLocalAI();
        public void StartLocalGame()
        {
            var ai_turn_time = 20;
            // int.TryParse(AITurnTime.text, out AI_turn_time);
            var output = chessManager.StartLocalGame(ai_turn_time);
            if (output is not null) localStartFailText.text = output; // Start failed
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
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (chessManager.CurrentGameManager is null && playerData.Count != 0)
            {
                Invoke("ForceUpdateLobby", 1f); // Run again in 1s
                return;
            }

            List<PlayerCardController> to_remove = new List<PlayerCardController>();

            foreach (var player_card in playerCardControllers)
            {
                if (!playerData.ContainsKey(player_card.PlayerID))
                {
                    Destroy(player_card.gameObject);
                    to_remove.Add(player_card); // Queue removal
                }
                else // Update card
                {
                    var player_data = playerData[player_card.PlayerID];
                    player_card.PlayerName = player_data.Name;
                    player_card.Team = player_data.Team;
                    player_card.PlayerOnTeam = player_data.PlayerInTeam;
                    player_card.UpdateFields();
                }
            }

            // Remove
            foreach (var player_card in to_remove) playerCardControllers.Remove(player_card);


            foreach (var player_data in playerData.Values)
            {
                var shown = false;
                foreach (var player_card in playerCardControllers)
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
            var new_card = Instantiate(playerCardPrefab.gameObject, playerCardPrefab.transform.parent, true);
            new_card.SetActive(true);
            var card_controller = new_card.GetComponent<PlayerCardController>();

            card_controller.PlayerID = playerData.PlayerID;
            card_controller.PlayerName = playerData.Name;
            card_controller.Team = playerData.Team;
            card_controller.PlayerOnTeam = playerData.PlayerInTeam;
            card_controller.ChessManager = chessManager;
            card_controller.MenuUIManager = this;

            var rect_transform = new_card.GetComponent<RectTransform>();
            rect_transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * 0.07f);

            card_controller.UpdateFields();

            playerCardControllers.Add(card_controller); 
        }
    }
}