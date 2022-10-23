using System;
using System.Collections.Generic;
using UnityEngine;
using Networking.Client;
using UnityEngine.SceneManagement;
using Gamemodes;
using Game;
using MainMenu;

#nullable enable

/// <summary>
/// Communicates between all other systems
/// </summary>
public class ChessManager : MonoBehaviour
{
    private NetworkManager networkManager = default!;

    public MenuUIManager menuUI = default!;

    public InputManager InputManager = default!;
    public VisualManager VisualManager = default!;


    public List<AbstractGameManagerData> GameManagersData = new List<AbstractGameManagerData>();
    public AbstractGameManagerData CurrentGameManager = default!;
    public AbstractGameManager GameManager = default!;
    private byte[] saveData = new byte[0];
    
    public bool InGame = false;

    private int prevPlayer = -1;

    public bool MyTurn = false;

    /// <summary>
    /// A queue of actions to be excecuted on the main thread on the next frame
    /// </summary>
    private Queue<Action> monobehaviourActions = new Queue<Action>();

    // Called once
    private void OnEnable()
    {
        DontDestroyOnLoad(this);

        // Loads all game managers
        GameManagersData = Util.GetAllGameManagers();
        SceneManager.sceneLoaded += OnSceneLoaded;

        networkManager = FindObjectOfType<NetworkManager>();
    }

    /// <summary>
    /// Called every scene change
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == BuildIndex.MainMenu)
        {
            menuUI = FindObjectOfType<MenuUIManager>();
        }
        else // Main Scene
        {
            InputManager = FindObjectOfType<InputManager>();
            VisualManager = FindObjectOfType<VisualManager>();
            InGame = true;

            // Later load from save data
            int team_start = 0;
            int player_in_team_start = 0;
            ClientPlayerData local_player = networkManager.GetPlayerList()[networkManager.GetLocalPlayerID()];
            if (local_player.Team == team_start && local_player.PlayerOnTeam == player_in_team_start) { OnTurn(); }
        }

    }

    // Handles the main menu and configuring the game
    #region Main Menu & Game Config
    /// <summary>
    /// Hosts a game
    /// </summary>
    /// <param name="hostSettings"></param>
    public void Host(HostSettings hostSettings)
    {
        CurrentGameManager = hostSettings.GameMode;
        networkManager.Host(hostSettings, PlayerListUpdate, OnGameStart);
    }

    /// <summary>
    /// Joins a game
    /// </summary>
    /// <param name="joinSettings"></param>
    public void Join(JoinSettings joinSettings)
    {
        networkManager.Join(joinSettings);
    }

    /// <summary>
    /// Handles incoming game data
    /// </summary>
    /// <param name="gameMode"></param>
    /// <param name="saveData"></param>
    public void GameDataRecived(int gameMode, byte[] saveData)
    {
        foreach (AbstractGameManagerData g in GameManagersData)
        {
            if (g.GetUID() == gameMode)
            {
                CurrentGameManager = g;
                break;
            }
        }
        this.saveData = saveData;
        monobehaviourActions.Enqueue(menuUI.GamemodeDataRecieve);
    }

    /// <summary>
    /// Sets the team and playerOnTeam of a player (Available to host only)
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="team"></param>
    /// <param name="playerOnTeam"></param>
    public void HostSetTeam(int playerID, int team, int playerOnTeam) => networkManager.HostSetTeam(playerID, team, playerOnTeam);

    /// <summary>
    /// Starts the game
    /// </summary>
    public void HostStartGame()
    {
        string? result = networkManager.HostStartGame();
        if (result is not null) HostStartGameFail(result);
    }
    #endregion

    // UI updates called from outside the main thread
    #region UIUpdateOnInfo
    private void HostStartGameFail(string reason) => monobehaviourActions.Enqueue(() => { menuUI.HostStartGameFailed(reason); });
    public void HostSucceed() => monobehaviourActions.Enqueue(() => { menuUI.HostConnectionSuccessful(); });
    public void HostFailed(string reason) => monobehaviourActions.Enqueue(() => { menuUI.HostFailed(reason); });
    public void JoinSucceed() => monobehaviourActions.Enqueue(() => { menuUI.JoinConnectionSuccessful(); });
    public void JoinFailed(string reason) => monobehaviourActions.Enqueue(() => { menuUI.JoinFailed(reason); });
    public void ClientKicked(string reason) => monobehaviourActions.Enqueue(() => { menuUI.ClientKicked(reason); });
    public void PlayerListUpdate() => monobehaviourActions.Enqueue(() => { menuUI.UpdateLobbyDisplay(networkManager.GetPlayerList()); });
    #endregion

    // Handles Scene Changes and Gamemode Loading
    #region Game Loading
    /// <summary>
    /// Called when the game is started
    /// </summary>
    public void OnGameStart() => monobehaviourActions.Enqueue(() => { LoadGame(); });

    /// <summary>
    /// Loads the game
    /// </summary>
    private void LoadGame()
    {
        SceneManager.LoadScene(1); // Load main scene
        GameManager = CurrentGameManager.Instantiate(this); // Instantiate GameManager
        if (saveData.Length > 0) GameManager.LoadData(saveData); // Load save data
    }

    /// <summary>
    /// Exits the game (WIP)
    /// </summary>
    public void ExitGame()
    {
        InGame = false;
        networkManager.Shutdown();
        Destroy(networkManager.gameObject);
        Destroy(this.gameObject);
        SceneManager.LoadScene(0); // Load menu scene
    }
    #endregion

    // Fetches various data from the NetworkManager
    #region NetworkManager Encapsulations
    public bool IsHost() => networkManager.IsHost;
    public int GetLocalPlayerID() => networkManager.GetLocalPlayerID();
    public int GetLocalPlayerTeam() => networkManager.GetPlayerList()[GetLocalPlayerID()].Team;
    public int GetPlayerByTeam(int team, int playerInTeam) => networkManager.GetPlayerByTeam(team, playerInTeam);
    #endregion

    // Handles local and foreign moves
    #region Move Logic
    /// <summary>
    /// Run when it's the local player's turn
    /// </summary>
    public void OnTurn()
    {
        prevPlayer = GetLocalPlayerID();

        MyTurn = true;

        var possible_moves = GameManager.GetMoves();
        if (possible_moves.Count == 0)
        {
            OnLocalMove(GameManager.OnNoMoves(), new V2(0, 0), new V2(0, 0));
            return;
        }

        VisualManager.SetPossibleMoves(possible_moves);
        InputManager.SetPossibleMoves(possible_moves);
    }

    /// <summary>
    /// Queues OnForeignMove as it can only be run from the main thread
    /// </summary>
    /// <param name="nextPlayer"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void OnForeignMoveUpdate(int nextPlayer, V2 from, V2 to) => monobehaviourActions.Enqueue(() => OnForeignMove(nextPlayer, from, to));

    /// <summary>
    /// Handles a foreign move update. A negative next player indicates a team win
    /// </summary>
    /// <param name="nextPlayer"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void OnForeignMove(int nextPlayer, V2 from, V2 to)
    {
        if (prevPlayer != GetLocalPlayerID()) GameManager.OnMove(from, to);

        VisualManager.UpdateAllPieces();

        if (nextPlayer < 0)
        {
            VisualManager.OnTeamWin(-(nextPlayer + 1));
            return;
        }

        if (nextPlayer == GetLocalPlayerID()) OnTurn();
        else prevPlayer = nextPlayer;
    }

    /// <summary>
    /// Handles a local move update
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void GetLocalMove(V2 from, V2 to)
    {
        int next_player = GameManager.OnMove(from, to);
        OnLocalMove(next_player, from, to);
    }

    /// <summary>
    /// Handles a local move update with the next player's turn
    /// </summary>
    /// <param name="nextPlayer"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void OnLocalMove(int nextPlayer, V2 from, V2 to)
    {
        MyTurn = false;
        networkManager.OnLocalMove(nextPlayer, from, to);
    }
    #endregion

    public void Update()
    {
        /* 
         * Most unity functions can only be called from the main thread so this 
         * goes through queued functions to run them from the main thread on the next frame 
         */
        while (monobehaviourActions.Count > 0)
        {
            monobehaviourActions.Dequeue().Invoke();
        }
    }
}