using System;
using System.Collections.Generic;
using UnityEngine;
using Networking.Client;
using UnityEngine.SceneManagement;
using Gamemodes;

#nullable enable

// TODO: WIP - Remove pseudocode
/// <summary>
/// Communicates between AIManager, GameManager and NetworkManager
/// </summary>
public class ChessManager : MonoBehaviour
{
    public List<AbstractGameManagerData> GameManagersData = new List<AbstractGameManagerData>();

    public MenuUIManager menuUI = default!;

    public InputManager inputManager = default!;
    public VisualManager visualManager = default!;

    public AbstractGameManagerData currentGameManager = default!;
    private byte[] saveData = new byte[0];
    public AbstractGameManager GameManager = default!;
    public bool InGame = false;

    private NetworkManager networkManager = default!;

    private Queue<Action> monobehaviourActions = new Queue<Action>();

    int prevPlayer = -1;

    public bool MyTurn = false;

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
        // Loads all game managers
        GameManagersData = Util.GetAllGameManagers();
        SceneManager.sceneLoaded += OnSceneLoaded;
        networkManager = FindObjectOfType<NetworkManager>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == BuildIndex.MainMenu)
        {
            menuUI = FindObjectOfType<MenuUIManager>();
        }
        else // Main Scene
        {
            inputManager = FindObjectOfType<InputManager>();
            visualManager = FindObjectOfType<VisualManager>();
            InGame = true;

            // Later load from save data
            int team_start = 0;
            int player_in_team_start = 0;
            ClientPlayerData local_player = networkManager.GetPlayerList()[networkManager.GetLocalPlayerID()];
            if (local_player.Team == team_start && local_player.PlayerInTeam == player_in_team_start) { OnTurn(); }
        }

    }

    public void Host(HostSettings hostSettings)
    {
        currentGameManager = hostSettings.GameMode;
        networkManager.Host(hostSettings, PlayerListUpdate, OnGameStart);
    }

    public void Join(JoinSettings joinSettings)
    {
        networkManager.Join(joinSettings);
    }

    public void GameDataRecived(int gameMode, byte[] saveData)
    {
        foreach (AbstractGameManagerData g in GameManagersData)
        {
            if (g.GetUID() == gameMode)
            {
                currentGameManager = g;
                break;
            }
        }
        this.saveData = saveData;
        monobehaviourActions.Enqueue(menuUI.GamemodeDataRecieve);
    }

    public void HostSetTeam(int playerID, int team, int playerInTeam) => networkManager.HostSetTeam(playerID, team, playerInTeam);

    public void HostStartGame()
    {
        string? result = networkManager.HostStartGame();
        if (result is not null) HostStartGameFail(result);
    }


    #region UIUpdateOnInfo
    private void HostStartGameFail(string reason) => monobehaviourActions.Enqueue(() => { menuUI.HostStartGameFailed(reason); });
    public void HostSucceed() => monobehaviourActions.Enqueue(() => { menuUI.HostConnectionSuccessful(); });
    public void HostFailed(string reason) => monobehaviourActions.Enqueue(() => { menuUI.HostFailed(reason); });
    public void JoinSucceed() => monobehaviourActions.Enqueue(() => { menuUI.JoinConnectionSuccessful(); });
    public void JoinFailed(string reason) => monobehaviourActions.Enqueue(() => { menuUI.JoinFailed(reason); });
    public void ClientKicked(string reason) => monobehaviourActions.Enqueue(() => { menuUI.ClientKicked(reason); });
    public void PlayerListUpdate() => monobehaviourActions.Enqueue(() => { menuUI.UpdateLobbyDisplay(networkManager.GetPlayerList()); });
    #endregion
    
    public void OnGameStart() => monobehaviourActions.Enqueue(() => { LoadGame(); });

    private void LoadGame()
    {
        SceneManager.LoadScene(1); // Load main scene
        GameManager = currentGameManager.Instantiate(this);
        if (saveData.Length > 0) GameManager.LoadData(saveData);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(1); // Load menu scene
        InGame = false;
    }

    public int GetLocalPlayerID()
    {
        return networkManager.GetLocalPlayerID();
    }

    public int GetLocalPlayerTeam()
    {
        return networkManager.GetPlayerList()[GetLocalPlayerID()].Team;
    }

    public int GetPlayerByTeam(int team, int playerInTeam)
    {
        return networkManager.GetPlayerByTeam(team, playerInTeam);
    }

    public void OnTurn()
    {
        Debug.Log("On Turn");
        prevPlayer = GetLocalPlayerID();
        MyTurn = true;
        var possible_moves = GameManager.GetMoves();
        visualManager.SetPossibleMoves(possible_moves);
        inputManager.SetPossibleMoves(possible_moves);
    }

    public void OnForeignMoveUpdate(int nextPlayer, V2 from, V2 to) => monobehaviourActions.Enqueue(() => OnForeignMove(nextPlayer, from, to));

    public void OnForeignMove(int nextPlayer, V2 from, V2 to)
    {
        Debug.Log("Foreign Move");
        if (prevPlayer != GetLocalPlayerID()) GameManager.OnMove(from, to);

        visualManager.UpdateAllPieces();

        if (nextPlayer == GetLocalPlayerID()) OnTurn();
        else prevPlayer = nextPlayer;
    }

    public void GetLocalMove(V2 from, V2 to)
    {
        Debug.Log("Get Local Move");
        int next_player = GameManager.OnMove(from, to);
        OnLocalMove(next_player, from, to);
    }

    public void OnLocalMove(int nextPlayer, V2 from, V2 to)
    {
        Debug.Log("On Local Move");
        MyTurn = false;
        networkManager.OnLocalMove(nextPlayer, from, to);
    }

    public void Update()
    {
        while (monobehaviourActions.Count > 0)
        {
            monobehaviourActions.Dequeue().Invoke();
        }
    }
}