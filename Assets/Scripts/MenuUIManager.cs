using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections.Concurrent;
using UnityEngine.UI;

/// <summary>
/// Manages the UI on the main menu
/// </summary>
public class MenuUIManager : MonoBehaviour
{
    #region GameObject References
    public GameObject HostConfig;
    private bool showingHostSettings;
    public TMP_Text HostNameInput;
    public TMP_Text HostPasswordInput;
    public TMP_Dropdown HostGamemodeDropdown;
    public TMP_Text HostStatusText;
    public Button HostStartButton;

    public GameObject HostScreen;
    private bool showingHostScreen;

    public GameObject JoinConfig;
    private bool showingJoinSettings;
    public TMP_Text JoinIpInput;
    public TMP_Text JoinNameInput;
    public TMP_Text JoinPasswordInput;

    public GameObject JoinScreen;
    private bool showingJoinScreen;
    public TMP_Text JoinStatusText;

    public TMP_Text LobbyDisplay;
    #endregion

    private ChessManager chessManager;

    private Dictionary<string, AbstractGameManagerData> gamemodes = new Dictionary<string, AbstractGameManagerData>();

    // Start is called before the first frame update
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

    public void HostStartGame() => chessManager.HostStartGame();

    public void Join()
    {
        if (showingHostSettings || showingHostScreen || showingJoinScreen) return;

        showingJoinSettings = true;
        JoinConfig.SetActive(true);
    }

    public void FullJoin()
    {
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
        LobbyDisplay.text = "";
        foreach (ClientPlayerData player in playerData.Values)
        {
            LobbyDisplay.text += $"[{player.PlayerID}]{player.Name} - [{player.PlayerInTeam}:{player.Team}]\n";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
