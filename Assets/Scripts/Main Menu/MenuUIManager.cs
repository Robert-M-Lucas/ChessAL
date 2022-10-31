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
        public GameObject HostConfigScreen = default!;
        private bool showingHostSettings = false;
        public TMP_InputField HostNameInput = default!;
        public TMP_Text HostNameDisallowedReason = default!;
        public TMP_InputField HostPasswordInput = default!;
        public TMP_Dropdown HostGamemodeDropdown = default!;
        public TMP_Text HostConfigHelpText = default!;
        public TMP_Text HostStatusText = default!;
        public TMP_Text HostScreenDescriptionText = default!;
        public TMP_InputField HostSavePathInput = default!;
        public Button HostStartButton = default!;
        public GameObject HostScreen = default!;
        private bool showingHostScreen = false;

        [Header("Join")]
        public GameObject JoinConfigScreen = default!;
        private bool showingJoinSettings = false;
        public TMP_InputField JoinIpInput = default!;
        public TMP_InputField JoinNameInput = default!;
        public TMP_Text JoinNameDisallowedReason = default!;
        public TMP_InputField JoinPasswordInput = default!;
        public GameObject JoinScreen = default!;
        private bool showingJoinScreen = false;
        public TMP_Text JoinStatusText = default!;
        public TMP_Text JoinScreenDescriptionText = default!;

        [Header("Local")]
        public GameObject LocalConfigScreen = default!;
        private bool showingLocalSettings = false;
        public TMP_Dropdown LocalGamemodeDropdown = default!;
        public TMP_Text LocalConfigHelpText = default!;
        public TMP_Text LocalScreenDescriptionText = default!;
        public TMP_InputField LocalSavePathInput = default!;
        public GameObject LocalScreen = default!;
        private bool showingLocalScreen = false;

        [Header("Other")]
        public GameObject LobbyDisplay = default!;
        public PlayerCardController PlayerCardPrefab = default!;

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
                HostGamemodeDropdown.options.Add(new TMP_Dropdown.OptionData(game.GetName()));
                LocalGamemodeDropdown.options.Add(new TMP_Dropdown.OptionData(game.GetName()));
            }
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
        }

        public void OpenSaveFolder() => SaveSystem.OpenSaveFolder();

        public void ShowGamemodeHelp()
        {
            HelpSystem.OpenHelp(chessManager.CurrentGameManager.GetUID());
        }

        #region Host
        public void HostConfig()
        {
            if (showingJoinSettings || showingJoinScreen || showingHostScreen || showingLocalSettings || showingLocalScreen) return;

            HideAllScreens();
            showingHostSettings = true;
            HostConfigScreen.SetActive(true);
        }

        public void HostShowGamemodeHelp()
        {
            if (HostGamemodeDropdown.value == 0) return;

            HelpSystem.OpenHelp(gamemodes[HostGamemodeDropdown.options[HostGamemodeDropdown.value].text].GetUID());
            HostConfigHelpText.text = gamemodes[HostGamemodeDropdown.options[HostGamemodeDropdown.value].text].GetDescription();
        }

        public void LoadSaveAndFullHost()
        {
            byte[] save_data = chessManager.LoadSave(HostSavePathInput.text);
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
                if (HostGamemodeDropdown.value == 0) return;

                host_settings = new HostSettings(gamemodes[HostGamemodeDropdown.options[HostGamemodeDropdown.value].text], HostPasswordInput.text, HostNameInput.text, null);
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

            

            chessManager.Host(host_settings);

            HideAllScreens();

            HostStatusText.text = "Connecting to internal server...";
            HostScreen.SetActive(true);
            showingHostScreen = true;
            LobbyDisplay.gameObject.SetActive(true);
            HostStatusText.gameObject.SetActive(true);
            HostScreenDescriptionText.text = host_settings.GameMode.GetDescription();
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
        }

        public void HostStartGame() => chessManager.HostStartGame();

        public void HostStartGameFailed(string reason)
        {
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
            string? validation_result = Validators.ValidatePlayerName(JoinNameInput.text);
            if (validation_result is not null)
            {
                JoinNameDisallowedReason.text = "Name: " + validation_result;
                return;
            }

            JoinSettings join_settings = new JoinSettings(JoinIpInput.text, JoinPasswordInput.text, JoinNameInput.text);

            chessManager.Join(join_settings);

            HideAllScreens();

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
            JoinScreenDescriptionText.text = chessManager.CurrentGameManager.GetDescription();
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
            UpdateLobbyPlayerCardDisplay(new ConcurrentDictionary<int, ClientPlayerData>());
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

        public void LocalPlayShowGamemodeHelp()
        {
            if (LocalGamemodeDropdown.value == 0) return;

            HelpSystem.OpenHelp(gamemodes[LocalGamemodeDropdown.options[LocalGamemodeDropdown.value].text].GetUID());
            LocalConfigHelpText.text = gamemodes[LocalGamemodeDropdown.options[LocalGamemodeDropdown.value].text].GetDescription();
        }

        public void LoadSaveAndFullLocalPlay()
        {
            byte[] save_data = chessManager.LoadSave(LocalSavePathInput.text);
            int gamemode = SerialisationUtil.GetGamemodeUID(save_data);
            FullLocalPlay(save_data, gamemode);
        }

        public void FullLocalPlay() => FullLocalPlay(null, null);

        public void FullLocalPlay(byte[]? saveData, int? gameMode)
        {
            HostSettings host_settings;
            if (saveData is null)
            {
                if (LocalGamemodeDropdown.value == 0) return;

                host_settings = new HostSettings(gamemodes[LocalGamemodeDropdown.options[LocalGamemodeDropdown.value].text], "", "", null);
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

            chessManager.PrepLocal(host_settings);

            HideAllScreens();

            showingLocalScreen = true;
            LobbyDisplay.gameObject.SetActive(true);
            LocalScreen.SetActive(true);
            LocalScreenDescriptionText.text = host_settings.GameMode.GetDescription();
        }

        public void AddLocalPlayer() =>  chessManager.AddLocalPlayer();
        public void AddLocalAI() => chessManager.AddLocalAI();
        public void RemoveLocalPlayer() => chessManager.RemoveLocalPlayer();
        public void RemoveLocalAI() => chessManager.RemoveLocalAI();
        public void StartLocalGame() => chessManager.StartLocalGame();

        public void CancelLocalPlay()
        {
            UpdateLobbyPlayerCardDisplay(new ConcurrentDictionary<int, ClientPlayerData>());
            chessManager.ResetLocalSettings();
            HideAllScreens();
        }

        #endregion

        public void UpdateLobbyPlayerCardDisplay(ConcurrentDictionary<int, ClientPlayerData> playerData)
        {
            List<PlayerCardController> to_remove = new List<PlayerCardController>();

            foreach (PlayerCardController player_card in playerCardControllers)
            {
                if (!playerData.ContainsKey(player_card.PlayerID))
                {
                    Destroy(player_card.gameObject);
                    to_remove.Add(player_card);
                }
                else
                {
                    ClientPlayerData player_data = playerData[player_card.PlayerID];
                    player_card.PlayerName = player_data.Name;
                    player_card.Team = player_data.Team;
                    player_card.PlayerOnTeam = player_data.PlayerInTeam;
                    player_card.UpdateFields();
                }
            }

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

                if (!shown) CreatePlayerCard(player_data);
            }
        }
        private void CreatePlayerCard(ClientPlayerData playerData)
        {
            GameObject new_card = Instantiate(PlayerCardPrefab.gameObject);
            PlayerCardController card_controller = new_card.GetComponent<PlayerCardController>();
            card_controller.PlayerID = playerData.PlayerID;
            card_controller.PlayerName = playerData.Name;
            card_controller.Team = playerData.Team;
            card_controller.PlayerOnTeam = playerData.PlayerInTeam;
            card_controller.ChessManager = chessManager;
            card_controller.MenuUIManager = this;

            new_card.transform.SetParent(PlayerCardPrefab.transform.parent);

            card_controller.UpdateFields();

            playerCardControllers.Add(card_controller);

            new_card.SetActive(true);
        }
    }
}