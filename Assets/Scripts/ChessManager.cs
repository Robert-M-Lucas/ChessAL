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
using System.Linq;
using AI;
using UnityEngine.Serialization;

#nullable enable

/// <summary>
/// Communicates between all other systems
/// 
/// Any method prefixed with 'M' must be run on the main thread
/// </summary>
public class ChessManager : MonoBehaviour
{
    // Managers
    private NetworkManager networkManager = default!;

    [SerializeField] private MenuUIManager menuUIManager = default!;

    [FormerlySerializedAs("InputManager")] [SerializeField] private InputManager inputManager = default!;
    [FormerlySerializedAs("VisualManager")] [SerializeField] private VisualManager visualManager = default!;

    // GameManager
    public List<AbstractGameManagerData> GameManagersData = new List<AbstractGameManagerData>();
    public AbstractGameManagerData CurrentGameManager = default!;
    public AbstractGameManager GameManager = default!;
    private byte[] saveData = new byte[0];
    
    // Turn
    public bool InGame { get; private set; } = false;
    public bool MyTurn { get; private set; } = false;
    // public bool AITurn { get; private set; } = false;
    public int CurrentPlayer { get; private set; } = -1;
    private int prevPlayer = -1;

    // Keeps track of ellapsed game time
    [FormerlySerializedAs("timerOffset")] [SerializeField] public long TimerOffset = 0;
    public Stopwatch Timer = new Stopwatch();

    // Local play
    [FormerlySerializedAs("localPlay")] [SerializeField] public bool LocalPlay = false;
    private ConcurrentDictionary<int, ClientPlayerData> localPlayerList = new ConcurrentDictionary<int, ClientPlayerData>();
    [FormerlySerializedAs("localAIPlayers")] [SerializeField] public List<int> LocalAIPlayers = new List<int>();
    private HostSettings localSettings = default!;
    
    // ReSharper disable once InconsistentNaming
    private int IDCounter = 0;

    /// <summary>
    /// A queue of actions to be excecuted on the main thread on the next frame
    /// </summary>
    private volatile Queue<Action> mainThreadActions = new Queue<Action>();

    #region Unity Methods

    // Called once
    private void OnEnable()
    {
        // Keep alive between scenes
        DontDestroyOnLoad(this);

        // Loads all game managers
        GameManagersData = Util.GetAllGameManagers();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Called once
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    public void OnApplicationQuit() => StopNetworking();

    // Called every frame
    public void Update()
    {
        /* 
         * Most unity functions can only be called from the main thread so this 
         * goes through functions queued by other threads to run them 
         * on the main thread during the next frame 
         */

        var temp_main_thread_actions = mainThreadActions;
        mainThreadActions = new Queue<Action>();

        while (temp_main_thread_actions.Count > 0)
        {
            temp_main_thread_actions.Dequeue().Invoke();
        }
    }

    #endregion

    /// <summary>
    /// Called every scene change
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == BuildIndex.MAIN_MENU)
        {
            menuUIManager = FindObjectOfType<MenuUIManager>();
        }
        else // Game Scene
        {
            inputManager = FindObjectOfType<InputManager>();
            visualManager = FindObjectOfType<VisualManager>();
            visualManager.ChessManager = this;

            InGame = true;

            // Initial updates
            if (MyTurn) MOnTurn();
            visualManager.OnTurn(GetPlayerList()[CurrentPlayer].Team, GetPlayerList()[CurrentPlayer].PlayerInTeam, MyTurn, LocalAIPlayers.Contains(CurrentPlayer));
        }

    }

    // Handles the main menu and configuring the game
    #region Main Menu & Game Config
    /// <summary>
    /// Hosts a game
    /// </summary>
    /// <param name="hostSettings"></param>
    public bool Host(HostSettings hostSettings)
    {
        CurrentGameManager = hostSettings.GameMode;
        var half_success = networkManager.Host(hostSettings, PlayerListUpdate, OnGameStart);
        return half_success;
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
    /// <param name="incomingSaveData"></param>
    public void GameDataReceived(int gameMode, byte[] incomingSaveData)
    {
        // Set game manager and save data
        CurrentGameManager = GameManagersData.Find(o => o.GetUID() == gameMode);
        saveData = incomingSaveData;

        mainThreadActions.Enqueue(menuUIManager.OnGamemodeDataRecieve);
    }

    public int FindNextNonFullTeam(int currentTeam, TeamSize[] teamSizes)
    {
        if (!LocalPlay)
        {
            return networkManager.FindNextNonFullTeam(currentTeam, teamSizes);
        }

        while (true)
        {
            currentTeam++;
            if (currentTeam >= teamSizes.Length) return -1;

            if (teamSizes[currentTeam].Max > localPlayerList.Count(o => o.Value.Team == currentTeam)) return currentTeam;
        }
    }

    /// <summary>
    /// Sets the team and playerOnTeam of a player (Available to host only)
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="team"></param>
    /// <param name="playerOnTeam"></param>
    public void HostSetTeam(int playerID, int team, int playerOnTeam)
    {
        if (!LocalPlay)
        {
            // Must go through network manager if game isn't local
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
        var result = networkManager.HostStartGame();
        if (result is not null) HostStartGameFail(result); // Failed
    }

    /// <summary>
    /// Initialises settings for local play
    /// </summary>
    /// <param name="settings"></param>
    public void PrepLocal(HostSettings settings)
    {
        LocalPlay = true;
        localSettings = settings;
        CurrentGameManager = settings.GameMode;
        saveData = settings.SaveData;
    }

    public void StopNetworking() => networkManager.Stop();

    /// <summary>
    /// Restarts networking system
    /// </summary>
    public void RestartNetworking()
    {
        StopNetworking();
        var new_network_manager = Instantiate(networkManager.gameObject);
        DestroyImmediate(networkManager.gameObject);
        networkManager = new_network_manager.GetComponent<NetworkManager>();
    }

    /// <summary>
    /// Calls the callback with ping as the parameter
    /// </summary>
    /// <param name="callback"></param>
    public void GetPing(Action<int> callback) => networkManager.GetPing(callback);

    /// <summary>
    /// Starts a local game
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public string? StartLocalGame(int AITurnTime)
    {
        // Check team composition
        var team_validation_result = Validators.ValidateTeams(localPlayerList.Values.ToList(), localSettings);
        if (team_validation_result is not null) return team_validation_result;

        // AIManager.MAX_SEARCH_TIME = AI_turn_time;

        Destroy(networkManager.gameObject); // Network manager not needed for local
        MLoadGame();
        return null; // No error
    }

    public void AddLocalPlayer() 
    {
        var team_sizes = CurrentGameManager.GetTeamSizes();
        var max_players = team_sizes.Sum(teamSize => teamSize.Max);
        if (localPlayerList.Count >= max_players) return;

        localPlayerList[IDCounter] = new ClientPlayerData(IDCounter, "Local Player", -1, -1);
        IDCounter++;
        menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
    }

    public void AddLocalAI() 
    {
        var team_sizes = CurrentGameManager.GetTeamSizes();
        var max_players = team_sizes.Sum(teamSize => teamSize.Max);
        if (localPlayerList.Count >= max_players) return;

        localPlayerList[IDCounter] = new ClientPlayerData(IDCounter, "AI Player", -1, -1);
        LocalAIPlayers.Add(IDCounter);
        IDCounter++;
        menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
    }

    public void RemoveLocalPlayer()
    {
        var client_list = localPlayerList.Values.ToList();

        for (var i = 0; i < client_list.Count(); i++)
        {
            if (LocalAIPlayers.Contains(client_list[i].PlayerID)) continue;
            localPlayerList.Remove(client_list[i].PlayerID, out _);
            menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
            return;
        }
    }

    public void RemoveLocalAI()
    {
        List<ClientPlayerData> client_list = localPlayerList.Values.ToList();

        for (var i = 0; i < client_list.Count(); i++)
        {
            if (LocalAIPlayers.Contains(client_list[i].PlayerID))
            {
                localPlayerList.Remove(client_list[i].PlayerID, out _);
                menuUIManager.UpdateLobbyPlayerCardDisplay(localPlayerList);
                return;
            }
        }
    }

    /*
    public void RemoveLocalPlayer(int playerID)
    {
        if (localPlayerList.ContainsKey(playerID)) localPlayerList.Remove(playerID, out _);
        if (LocalAIPlayers.Contains(playerID)) LocalAIPlayers.Remove(playerID);
    }
    */

    /// <summary>
    /// Resets local play settings
    /// </summary>
    public void ResetLocalSettings()
    {
        localPlayerList = new ConcurrentDictionary<int, ClientPlayerData>();
        LocalAIPlayers = new List<int>();
        IDCounter = 0;
        saveData = new byte[0];
        localSettings = default!;
        LocalPlay = false;
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
    public void PlayerListUpdate() 
    {
        if (!InGame)
            mainThreadActions.Enqueue(() => { menuUIManager.UpdateLobbyPlayerCardDisplay(GetPlayerList()); });
    }
    #endregion

    // Handles Scene Changes and Gamemode Loading
    #region Game Loading
    /// <summary>
    /// Called when the game is started
    /// </summary>
    public void OnGameStart() => mainThreadActions.Enqueue(() => { MLoadGame(); });

    /// <summary>
    /// Loads the game
    /// </summary>
    private void MLoadGame()
    {
        SceneManager.LoadScene(1); // Load main scene
        if (saveData.Length > 0)
        {
            var data = SerialisationUtil.Deconstruct(saveData); // Load save data

            if (!LocalPlay)
            {
                if (GetPlayerByTeam(data.TeamTurn, data.PlayerOnTeamTurn) == GetLocalPlayerID()) MyTurn = true;
                else MyTurn = false;
            }
            else MyTurn = true;
   
            CurrentPlayer = (int)GetPlayerByTeam(data.TeamTurn, data.PlayerOnTeamTurn)!;
            TimerOffset = data.ElapsedTime; // Initialise timer to savegame's timer

            GameManager = CurrentGameManager.Instantiate(GetLiveGameData()); // Instantiate GameManager
            GameManager.LoadData(data);
        }
        else
        {
            var team_start = 0;
            var player_in_team_start = 0;
            if (!LocalPlay)
            {
                var local_player = GetPlayerList()[GetLocalPlayerID()];
                if (local_player.Team == team_start && local_player.PlayerInTeam == player_in_team_start) MyTurn = true;
                else MyTurn = false;
            }
            else MyTurn = true;

            CurrentPlayer = (int)GetPlayerByTeam(team_start, player_in_team_start)!;
            GameManager = CurrentGameManager.Instantiate(GetLiveGameData()); // Instantiate GameManager
        }
        Timer.Start();
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    public void ExitGame()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Remove on scene loaded call on scene change
        AIManager.Reset();
        if (!LocalPlay)
        {
            DestroyImmediate(networkManager.gameObject);
            networkManager.Stop();
        }
        DestroyImmediate(this.gameObject);
        DestroyImmediate(this);
        InGame = false;
        SceneManager.LoadScene(0); // Load menu scene
    }
    #endregion

    // Fetches various data from the NetworkManager
    #region NetworkManager Encapsulations
    public bool IsHost() => networkManager.IsHost;
    public int GetLocalPlayerID()
    {
        if (!LocalPlay) return networkManager.GetLocalPlayerID();
        else return CurrentPlayer;
    }
    public int GetLocalPlayerTeam()
    {
        if (!LocalPlay) return GetPlayerList()[GetLocalPlayerID()].Team;
        else return localPlayerList[GetLocalPlayerID()].Team;
    }
    public int? GetPlayerByTeam(int team, int playerInTeam)
    {
        var player_list = GetPlayerList();

        foreach (var player_data in player_list.Values)
        {
            if (player_data.Team == team && player_data.PlayerInTeam == playerInTeam) return player_data.PlayerID;
        }

        return null;
    }

    public ConcurrentDictionary<int, ClientPlayerData> GetPlayerList()
    {
        if (!LocalPlay)
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

    public LiveGameData GetLiveGameData()
    {
        var game_data = new LiveGameData(this)
        {
            LocalPlayerID = GetLocalPlayerID(),
            LocalPlayerTeam = GetLocalPlayerTeam(),
            CurrentPlayer = CurrentPlayer
        };

        return game_data;
    }
    
    /// <summary>
    /// Called when it's the local players turn
    /// </summary>
    public void MOnTurn()
    {
        prevPlayer = GetLocalPlayerID();

        MyTurn = true;

        var game_data = GetLiveGameData();

        var possible_moves = GameManager.GetMoves(game_data, fastMode: false);
        if (possible_moves.Count == 0)
        {
            // Runs on no moves event
            MOnLocalMove(GameManager.OnNoMoves(game_data), new V2(0, 0), new V2(0, 0), false);
            return;
        }

        // AI turn
        if (LocalAIPlayers.Contains(CurrentPlayer))
        {
            // Clear possible moves
            visualManager.SetPossibleMoves(new List<Move>());
            inputManager.SetPossibleMoves(new List<Move>());

            // Start AI
            AIManager.SearchMove(possible_moves, game_data, GameManager);

            // Start checking AI progress
            mainThreadActions.Enqueue(() => MCheckAIDone());
        }
        else
        {
            visualManager.SetPossibleMoves(possible_moves);
            inputManager.SetPossibleMoves(possible_moves);
        }

    }

    /// <summary>
    /// Checks if the AI is completed and updates progress counter if not
    /// </summary>
    public void MCheckAIDone()
    {
        var move = AIManager.GetMove();
        if (move is null)
        {
            // Update progress
            visualManager.ShowAIInfo(true, AIManager.Progress);

            // Check again
            mainThreadActions.Enqueue(() => MCheckAIDone());
        }
        else // Found move
        {

            // Hide AI progress
            visualManager.ShowAIInfo(false, 0);

            // Apply move
            var next_player = GameManager.OnMove((Move)move, GetLiveGameData());
            mainThreadActions.Enqueue(() => MOnLocalMove(next_player, ((Move)move).From, ((Move)move).To, true));
        }
    }

    /// <summary>
    /// Queues OnForeignMove as it can only be run from the main thread
    /// </summary>
    /// <param name="nextPlayer"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void OnForeignMoveUpdate(int nextPlayer, V2 from, V2 to) => mainThreadActions.Enqueue(() => MOnForeignMove(nextPlayer, from, to));

    /// <summary>
    /// Handles a foreign move update. A negative next player indicates a team win
    /// </summary>
    /// <param name="nextPlayer"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="applyMove"></param>
    public void MOnForeignMove(int nextPlayer, V2 from, V2 to, bool applyMove = true)
    {
        if (prevPlayer != GetLocalPlayerID() && !LocalPlay && applyMove) GameManager.OnMove(new Move(from, to), GetLiveGameData()); // Make sure move was not made locally

        if (applyMove)
        {
            // Show and make move sound
            visualManager.OnMove(from, to);
            visualManager.UpdateAllPieces(new Move(from, to));
            SoundManager.GetInstance().PlayPieceMoveSound();
        }

        // If game is over
        if (nextPlayer < 0)
        {
            // Decode winning team and show win
            visualManager.OnTeamWin(Gamemodes.GUtil.TurnDecodeTeam(nextPlayer));
            return;
        }

        CurrentPlayer = nextPlayer;

        if (nextPlayer == GetLocalPlayerID() || LocalPlay) // My Turn next
        {
            visualManager.OnTurn(GetPlayerList()[nextPlayer].Team, GetPlayerList()[nextPlayer].PlayerInTeam, true, LocalAIPlayers.Contains(CurrentPlayer));

            MOnTurn();
        }
        else
        {
            visualManager.OnTurn(GetPlayerList()[nextPlayer].Team, GetPlayerList()[nextPlayer].PlayerInTeam, false, LocalAIPlayers.Contains(CurrentPlayer));
            prevPlayer = nextPlayer;
        }
    }

    /// <summary>
    /// Handles a local move update
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void DoLocalMove(V2 from, V2 to)
    {
        var next_player = GameManager.OnMove(new Move(from, to), GetLiveGameData());
        MOnLocalMove(next_player, from, to, true);
    }

    /// <summary>
    /// Handles a local move update with the next player's turn
    /// </summary>
    /// <param name="nextPlayer"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="applyMove"></param>
    public void MOnLocalMove(int nextPlayer, V2 from, V2 to, bool applyMove)
    {
        MyTurn = false;
        if (!LocalPlay)
        {
            networkManager.OnLocalMove(nextPlayer, from, to);
        }
        else
        {
            MOnForeignMove(nextPlayer, from, to, applyMove);
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
        var data = GameManager.GetData();
        var current_player = GetPlayerList()[CurrentPlayer];
        data.TeamTurn = current_player.Team;
        data.PlayerOnTeamTurn = current_player.PlayerInTeam;
        data.ElapsedTime = Timer.ElapsedMilliseconds + TimerOffset;
        return SaveSystem.Save(SerialisationUtil.Construct(data), fileName);
    }

    /// <summary>
    /// Loads the bytes of the specified save file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public byte[] LoadSave(string fileName) => SaveSystem.Load(fileName);

    #endregion
}