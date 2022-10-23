using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections.Concurrent;
using UnityEngine.UI;
using System;
using Networking.Client;
using Gamemodes;

#nullable enable

namespace MainMenu
{
    /// <summary>
    /// Manages the UI on the main menu
    /// </summary>
    public class MenuUIManager : MonoBehaviour
    {
        #region GameObject References
        public GameObject HostConfig = default!;
        private bool showingHostSettings;
        public TMP_InputField HostNameInput = default!;
        public TMP_Text HostNameDisallowedReason = default!;
        public TMP_InputField HostPasswordInput = default!;
        public TMP_Dropdown HostGamemodeDropdown = default!;
        public TMP_Text HostStatusText = default!;
        public Button HostStartButton = default!;
        /*
        public TMP_InputField HostPlayerIDInput = default!;
        public TMP_InputField HostTeamInput = default!;
        public TMP_InputField HostPlayerInTeamInput = default!;
        */

        public GameObject HostScreen = default!;
        private bool showingHostScreen;

        public GameObject JoinConfig = default!;
        private bool showingJoinSettings;
        public TMP_InputField JoinIpInput = default!;
        public TMP_InputField JoinNameInput = default!;
        public TMP_Text JoinNameDisallowedReason = default!;
        public TMP_InputField JoinPasswordInput = default!;

        public GameObject JoinScreen = default!;
        private bool showingJoinScreen;
        public TMP_Text JoinStatusText = default!;

        public GameObject LobbyDisplay = default!;
        public PlayerCardController PlayerCardPrefab = default!;
        private List<PlayerCardController> playerCardControllers = new List<PlayerCardController>();
        #endregion

        private ChessManager chessManager = default!;

        private Dictionary<string, AbstractGameManagerData> gamemodes = new Dictionary<string, AbstractGameManagerData>();

        public MenuUIManager()
        {

        }

        void Start()
        {
            chessManager = FindObjectOfType<ChessManager>();
            List<AbstractGameManagerData> gamemode_data = Util.GetAllGameManagers();
            foreach (AbstractGameManagerData game in gamemode_data)
            {
                gamemodes.Add(game.GetName(), game);
                HostGamemodeDropdown.options.Add(new TMP_Dropdown.OptionData(game.GetName()));
            }
        }

        public void Host()
        {
            if (showingJoinSettings || showingJoinScreen || showingHostScreen) return;
            showingHostSettings = true;
            HostConfig.SetActive(true);
        }

        public void FullHost()
        {
            if (HostGamemodeDropdown.value == 0) return;

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

            HostSettings host_settings = new HostSettings(gamemodes[HostGamemodeDropdown.options[HostGamemodeDropdown.value].text], HostPasswordInput.text, HostNameInput.text, null);

            chessManager.Host(host_settings);

            HostStatusText.text = "Connecting to internal server...";
            HostConfig.SetActive(false);
            showingHostSettings = false;
            HostScreen.SetActive(true);
            showingHostScreen = true;
            LobbyDisplay.gameObject.SetActive(true);
            HostStatusText.gameObject.SetActive(true);
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
            HostConfig.SetActive(false);
            showingHostSettings = false;
            HostScreen.SetActive(false);
            showingHostScreen = false;
            LobbyDisplay.gameObject.SetActive(false);
            HostStartButton.gameObject.SetActive(false);
        }

        /*
        public void HostSetTeam()
        {
            try
            {
                chessManager.HostSetTeam(int.Parse(HostPlayerIDInput.text), int.Parse(HostTeamInput.text), int.Parse(HostPlayerInTeamInput.text));
            }
            catch (FormatException) { }
        }
        */

        public void HostStartGame() => chessManager.HostStartGame();

        public void HostStartGameFailed(string reason)
        {
            Debug.Log(reason);
        }

        public void Join()
        {
            if (showingHostSettings || showingHostScreen || showingJoinScreen) return;

            showingJoinSettings = true;
            JoinConfig.SetActive(true);
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

            JoinStatusText.text = "Connecting to server...";
            JoinConfig.SetActive(false);
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

        public void GamemodeDataRecieve()
        {
            JoinStatusText.text += " SUCCESS";
            JoinStatusText.gameObject.SetActive(false);
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
            JoinConfig.SetActive(false);
            showingJoinSettings = false;
            JoinScreen.SetActive(false);
            showingJoinScreen = false;
            LobbyDisplay.gameObject.SetActive(false);
        }

        public void UpdateLobbyDisplay(ConcurrentDictionary<int, ClientPlayerData> playerData)
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
                    player_card.PlayerOnTeam = player_data.PlayerOnTeam;
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
            card_controller.PlayerOnTeam = playerData.PlayerOnTeam;
            card_controller.ChessManager = chessManager;
            card_controller.MenuUIManager = this;

            new_card.transform.SetParent(PlayerCardPrefab.transform.parent);

            card_controller.UpdateFields();

            playerCardControllers.Add(card_controller);

            new_card.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}