using System;
using System.Collections.Generic;
using UnityEngine;
using Networking.Client;
using UnityEngine.SceneManagement;
using Gamemodes;
using Game;
using MainMenu;
using System.Collections.Concurrent;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;
using System.Threading;
using AI;

#nullable enable

/// <summary>
/// Communicates between all other systems
/// </summary>
public class ChessManager : MonoBehaviour
{
    // Managers
    private NetworkManager networkManager = default!;

    public MenuUIManager menuUIManager = default!;

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
    public int CurrentPlayer = -1;
    private int prevPlayer = -1;

    // Keeps track of ellapsed game time
    public long TimerOffset = 0;
    public Stopwatch Timer = new Stopwatch();

    // Local play
    public bool localPlay = false;
    private ConcurrentDictionary<int, ClientPlayerData> localPlayerList = new ConcurrentDictionary<int, ClientPlayerData>();
    public List<int> LocalAIPlayers = new List<int>();
    private HostSettings localSettings = default!;
    private int IDCounter = 0;

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
            menuUIManager = FindObjectOfType<MenuUIManager>();
        }
        else // Game Scene
        {
            InputManager = FindObjectOfType<InputManager>();
            VisualManager = FindObjectOfType<VisualManager>();
            VisualManager.ChessManager = this;
            InGame = true;
            if (MyTurn) OnTurn();

            VisualManager.OnTurn(GetPlayerList()[CurrentPlayer].Team, GetPlayerList()[CurrentPlayer].PlayerInTeam, MyTurn);
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
        mainThreadActions.Enqueue(menuUIManager.OnGamemodeDataRecieve);
    }

    /// <summary>
    /// Sets the team and playerOnTeam of a player (Available to host only)
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="team"></param>
    /// <param name="playerOnTeam"></param>
    public void HostSetTeam(int playerID, int team, int playerOnTeam)
    {
        if (!localPlay)
        {
            networkManager.HostSetTeam(playerID, team, playerOnTeam);
        }
        else
        {
            localPlayerList[playerID].Team = team;
            localPlayerList[playerID].PlayerInTeam = playerOnTeam;
            menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
        }
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void HostStartGame()
    {
        string? result = networkManager.HostStartGame();
        if (result is not null) HostStartGameFail(result);
    }

    /// <summary>
    /// Initialises settings for local play
    /// </summary>
    /// <param name="settings"></param>
    public void PrepLocal(HostSettings settings)
    {
        localPlay = true;
        localSettings = settings;
        CurrentGameManager = settings.GameMode;
        saveData = settings.SaveData;
    }

    /// <summary>
    /// Restarts networking system
    /// </summary>
    public void RestartNetworking()
    {
        networkManager.Stop();
        GameObject new_network_manager = Instantiate(networkManager.gameObject);
        Destroy(networkManager.gameObject);
        networkManager = new_network_manager.GetComponent<NetworkManager>();
    }

    public void GetPing(Action<int> callback) => networkManager.GetPing(callback);

    /// <summary>
    /// Starts a local game
    /// </summary>
    public void StartLocalGame()
    {
        if (Validators.ValidateTeams(localPlayerList.Values.ToList(), localSettings) is not null) return;

        Destroy(networkManager.gameObject);
        LoadGame();
    }

    public void AddLocalPlayer() 
    {
        localPlayerList[IDCounter] = new ClientPlayerData(IDCounter, "Local Player", -1, -1);
        IDCounter++;
        menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
    }
    public void AddLocalAI() 
    {
        localPlayerList[IDCounter] = new ClientPlayerData(IDCounter, "AI Player", -1, -1);
        LocalAIPlayers.Add(IDCounter);
        IDCounter++;
        menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
    }

    public void RemoveLocalPlayer()
    {
        List<ClientPlayerData> client_list = localPlayerList.Values.ToList();

        for (int i = 0; i < client_list.Count(); i++)
        {
            if (!LocalAIPlayers.Contains(client_list[i].PlayerID))
            {
                localPlayerList.Remove(client_list[i].PlayerID, out _);
                menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
                return;
            }
        }
    }

    public void RemoveLocalAI()
    {
        List<ClientPlayerData> client_list = localPlayerList.Values.ToList();

        for (int i = 0; i < client_list.Count(); i++)
        {
            if (LocalAIPlayers.Contains(client_list[i].PlayerID))
            {
                localPlayerList.Remove(client_list[i].PlayerID, out _);
                menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
                return;
            }
        }
    }

    public void RemoveLocalPlayer(int playerID)
    {
        if (localPlayerList.ContainsKey(playerID)) localPlayerList.Remove(playerID, out _);
        if (LocalAIPlayers.Contains(playerID)) LocalAIPlayers.Remove(playerID);
    }

    public void ResetLocalSettings()
    {
        localPlayerList = new ConcurrentDictionary<int, ClientPlayerData>();
        LocalAIPlayers = new List<int>();
        IDCounter = 0;
        saveData = new byte[0];
        localSettings = default!;
        localPlay = false;
    }

    #endregion

    // UI updates called from outside the main thread
    #region UIUpdateOnInfo
    private void HostStartGameFail(string reason) => mainThreadActions.Enqueue(() => { menuUIManager.HostStartGameFailed(reason); });
    public void HostSucceed() => mainThreadActions.Enqueue(() => { menuUIManager.HostConnectionSuccessful(); });
    public void HostFailed(string reason) => mainThreadActions.Enqueue(() => { menuUIManager.HostFailed(reason); });
    public void JoinSucceed() => mainThreadActions.Enqueue(() => { menuUIManager.JoinConnectionSuccessful(); });
    public void JoinFailed(string reason) => mainThreadActions.Enqueue(() => { menuUIManager.JoinFailed(reason); });
    public void ClientKicked(string reason) => mainThreadActions.Enqueue(() => { menuUIManager.ClientKicked(reason); });
    public void PlayerListUpdate() => mainThreadActions.Enqueue(() => { menuUIManager.UpdateLobbyPlayerCardDisplay(GetPlayerList()); });
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
        Debug.Log(CurrentGameManager);
        GameManager = CurrentGameManager.Instantiate(this); // Instantiate GameManager
        if (saveData.Length > 0)
        {
            SerialisationData data = SerialisationUtil.Deconstruct(saveData); // Load save data

            if (!localPlay)
            {
                if (GetPlayerByTeam(data.TeamTurn, data.PlayerOnTeamTurn) == GetLocalPlayerID()) MyTurn = true;
                else MyTurn = false;
            }
            else MyTurn = true;

            GameManager.LoadData(data);
            CurrentPlayer = GetPlayerByTeam(data.TeamTurn, data.PlayerOnTeamTurn);
            TimerOffset = data.EllapsedTime;
        }
        else
        {
            int team_start = 0;
            int player_in_team_start = 0;
            if (!localPlay)
            {
                ClientPlayerData local_player = GetPlayerList()[GetLocalPlayerID()];
                if (local_player.Team == team_start && local_player.PlayerInTeam == player_in_team_start) MyTurn = true;
                else MyTurn = false;
            }
            else  MyTurn = true;

            CurrentPlayer = GetPlayerByTeam(team_start, player_in_team_start);
        }
        Timer.Start();
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    public void ExitGame()
    {
        if (!localPlay)
        {
            Destroy(networkManager.gameObject);
            networkManager.Stop();
        }
        Destroy(this.gameObject);
        InGame = false;
        SceneManager.LoadScene(0); // Load menu scene
    }
    #endregion

    // Fetches various data from the NetworkManager
    #region NetworkManager Encapsulations
    public bool IsHost() => networkManager.IsHost;
    public int GetLocalPlayerID()
    {
        if (!localPlay) return networkManager.GetLocalPlayerID();
        else return CurrentPlayer;
    }
    public int GetLocalPlayerTeam()
    {
        if (!localPlay) return GetPlayerList()[GetLocalPlayerID()].Team;
        else return localPlayerList[GetLocalPlayerID()].Team;
    }
    public int GetPlayerByTeam(int team, int playerInTeam)
    {
        var playerList = GetPlayerList();

        foreach (ClientPlayerData player_data in playerList.Values)
        {
            if (player_data.Team == team && player_data.PlayerInTeam == playerInTeam) return player_data.PlayerID;
        }

        throw new Exception("Player not found");
    }

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

        // AI turn
        if (LocalAIPlayers.Contains(CurrentPlayer))
        {
            Move move = AIManager.GetMove(possible_moves);
            int next_player = GameManager.OnMove(move.From, move.To);
            mainThreadActions.Enqueue(() => OnLocalMove(next_player, move.From, move.To));
            return;
        }
        else
        {
            VisualManager.SetPossibleMoves(possible_moves);
            InputManager.SetPossibleMoves(possible_moves);
        }

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

        CurrentPlayer = nextPlayer;

        if (nextPlayer == GetLocalPlayerID() || localPlay)
        {
            VisualManager.OnTurn(GetPlayerList()[nextPlayer].Team, GetPlayerList()[nextPlayer].PlayerInTeam, true);

            OnTurn();
        }
        else
        {
            VisualManager.OnTurn(GetPlayerList()[nextPlayer].Team, GetPlayerList()[nextPlayer].PlayerInTeam, false);
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
    /// <summary>
    /// Saves the current game under a specified file name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>Null if successful or a string error</returns>
    public string? Save(string fileName)
    {
        SerialisationData data = GameManager.GetData();
        ClientPlayerData current_player = GetPlayerList()[CurrentPlayer];
        data.TeamTurn = current_player.Team;
        data.PlayerOnTeamTurn = current_player.PlayerInTeam;
        data.EllapsedTime = Timer.ElapsedMilliseconds + TimerOffset;
        return SaveSystem.Save(SerialisationUtil.Construct(data), fileName);
    }

    /// <summary>
    /// Loads the bytes of the specified save file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public byte[] LoadSave(string fileName)
    {
        return SaveSystem.Load(fileName);
    }
    #endregion

    public void Update()
    {
        /* 
         * Most unity functions can only be called from the main thread so this 
         * goes through functions queued by other threads to run them 
         * ont the main thread during the next frame 
         */
        while (mainThreadActions.Count > 0)
        {
            mainThreadActions.Dequeue().Invoke();
        }
    }
}