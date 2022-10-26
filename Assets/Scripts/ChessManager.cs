using System;
using System.Collections.Generic;
using UnityEngine;
using Networking.Client;
using UnityEngine.SceneManagement;
using Gamemodes;
using Game;
using MainMenu;
using System.Collections.Concurrent;

#nullable enable

/// <summary>
/// Communicates between all other systems
/// </summary>
public class ChessManager : MonoBehaviour
{
    // Managers
    private NetworkManager networkManager = default!;

    public MenuUIManager menuUI = default!;

    public InputManager InputManager = default!;
    public VisualManager VisualManager = default!;

    // GameManager
    public List<AbstractGameManagerData> GameManagersData = new List<AbstractGameManagerData>();
    public AbstractGameManagerData CurrentGameManager = default!;
    public AbstractGameManager GameManager = default!;
    private byte[] saveData = new byte[0];
    
    // Turn
    public bool InGame = false;
    public bool MyTurn = false;
    private int currentPlayer = -1;
    private int prevPlayer = -1;

    // EXPERIMENTAL
    private bool localPlay = false;
    private ConcurrentDictionary<int, ClientPlayerData> localPlayerList = new ConcurrentDictionary<int, ClientPlayerData>();

    /// <summary>
    /// A queue of actions to be excecuted on the main thread on the next frame
    /// </summary>
    private Queue<Action> mainThreadActions = new Queue<Action>();

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
            if (MyTurn) OnTurn();

            VisualManager.OnTurn(GetPlayerList()[currentPlayer].Team, GetPlayerList()[currentPlayer].PlayerOnTeam, MyTurn);
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
        mainThreadActions.Enqueue(menuUI.GamemodeDataRecieve);
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
    private void HostStartGameFail(string reason) => mainThreadActions.Enqueue(() => { menuUI.HostStartGameFailed(reason); });
    public void HostSucceed() => mainThreadActions.Enqueue(() => { menuUI.HostConnectionSuccessful(); });
    public void HostFailed(string reason) => mainThreadActions.Enqueue(() => { menuUI.HostFailed(reason); });
    public void JoinSucceed() => mainThreadActions.Enqueue(() => { menuUI.JoinConnectionSuccessful(); });
    public void JoinFailed(string reason) => mainThreadActions.Enqueue(() => { menuUI.JoinFailed(reason); });
    public void ClientKicked(string reason) => mainThreadActions.Enqueue(() => { menuUI.ClientKicked(reason); });
    public void PlayerListUpdate() => mainThreadActions.Enqueue(() => { menuUI.UpdateLobbyDisplay(GetPlayerList()); });
    #endregion

    // Handles Scene Changes and Gamemode Loading
    #region Game Loading
    /// <summary>
    /// Called when the game is started
    /// </summary>
    public void OnGameStart() => mainThreadActions.Enqueue(() => { LoadGame(); });

    /// <summary>
    /// Loads the game
    /// </summary>
    private void LoadGame()
    {
        SceneManager.LoadScene(1); // Load main scene
        GameManager = CurrentGameManager.Instantiate(this); // Instantiate GameManager
        if (saveData.Length > 0)
        {
            SerialisationData data = SerialisationUtil.Deconstruct(saveData); // Load save data

            if (GetPlayerByTeam(data.TeamTurn, data.PlayerOnTeamTurn) == GetLocalPlayerID()) MyTurn = true;
            else MyTurn = false;
            GameManager.LoadData(data);
            currentPlayer = GetPlayerByTeam(data.TeamTurn, data.PlayerOnTeamTurn);
        }
        else
        {
            int team_start = 0;
            int player_in_team_start = 0;
            ClientPlayerData local_player = GetPlayerList()[networkManager.GetLocalPlayerID()];
            if (local_player.Team == team_start && local_player.PlayerOnTeam == player_in_team_start) MyTurn = true;
            else MyTurn = false;
            currentPlayer = GetPlayerByTeam(team_start, player_in_team_start);
        }
    }

    /// <summary>
    /// Exits the game
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
    public int GetLocalPlayerID()
    {
        if (!localPlay) return networkManager.GetLocalPlayerID();
        else return currentPlayer;
    }
    public int GetLocalPlayerTeam()
    {
        if (!localPlay) return GetPlayerList()[GetLocalPlayerID()].Team;
        else return localPlayerList[GetLocalPlayerID()].Team;
    }
    public int GetPlayerByTeam(int team, int playerInTeam) => networkManager.GetPlayerByTeam(team, playerInTeam);
    public ConcurrentDictionary<int, ClientPlayerData> GetPlayerList()
    {
        if (!localPlay)
        {
            return networkManager.GetPlayerList();
        }
        else
        {
            return localPlayerList;
        }
    }
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
    public void OnForeignMoveUpdate(int nextPlayer, V2 from, V2 to) => mainThreadActions.Enqueue(() => OnForeignMove(nextPlayer, from, to));

    /// <summary>
    /// Handles a foreign move update. A negative next player indicates a team win
    /// </summary>
    /// <param name="nextPlayer"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void OnForeignMove(int nextPlayer, V2 from, V2 to)
    {
        if (prevPlayer != GetLocalPlayerID() && !localPlay) GameManager.OnMove(from, to);
        

        VisualManager.OnMove(from, to);
        
        VisualManager.UpdateAllPieces();

        if (nextPlayer < 0)
        {
            VisualManager.OnTeamWin(-(nextPlayer + 1));
            return;
        }

        currentPlayer = nextPlayer;

        if (nextPlayer == GetLocalPlayerID() || localPlay)
        {
            VisualManager.OnTurn(GetPlayerList()[nextPlayer].Team, GetPlayerList()[nextPlayer].PlayerOnTeam, true);
            OnTurn();
        }
        else
        {
            VisualManager.OnTurn(GetPlayerList()[nextPlayer].Team, GetPlayerList()[nextPlayer].PlayerOnTeam, false);
            prevPlayer = nextPlayer;
        }
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
        if (!localPlay)
        {
            networkManager.OnLocalMove(nextPlayer, from, to);
        }
        else
        {
            OnForeignMove(nextPlayer, from, to);
        }
    }
    #endregion

    // Handles game saves
    #region Save Systems
    public string? Save(string fileName)
    {
        SerialisationData data = GameManager.GetData();
        ClientPlayerData current_player = GetPlayerList()[currentPlayer];
        data.TeamTurn = current_player.Team;
        data.PlayerOnTeamTurn = current_player.PlayerOnTeam;
        Debug.Log(data.PieceData.Count);
        return SaveSystem.Save(SerialisationUtil.Construct(data), fileName);
    }

    public byte[] Load(string fileName)
    {
        return SaveSystem.Load(fileName);
    }
    #endregion

    public void Update()
    {
        /* 
         * Most unity functions can only be called from the main thread so this 
         * goes through queued functions to run them from the main thread on the next frame 
         */
        while (mainThreadActions.Count > 0)
        {
            mainThreadActions.Dequeue().Invoke();
        }
    }
}