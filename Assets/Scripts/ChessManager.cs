using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

// TODO: WIP - Remove pseudocode
/// <summary>
/// Communicates between AIManager, GameManager and NetworkManager
/// </summary>
public class ChessManager : MonoBehaviour
{
    public List<AbstractGameManagerData> GameManagersData = new List<AbstractGameManagerData>();

    public MenuUIManager menuUI;

    public InputManager inputManager;

    public AbstractGameManagerData currentGameManager;
    private byte[] saveData = new byte[0];
    public AbstractGameManager gameManager;
    public bool InGame = false;

    private NetworkManager networkManager;

    private Queue<Action> monobehaviourActions = new Queue<Action>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        // Loads all game managers
        GameManagersData = Util.GetAllGameManagers();
        SceneManager.sceneLoaded += OnSceneLoaded;
        networkManager = FindObjectOfType<NetworkManager>();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
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
        }

    }

    public void Host(HostSettings hostSettings)
    {
        currentGameManager = hostSettings.GameMode;
        networkManager.Host(hostSettings);
    }
    public void Join(JoinSettings joinSettings) => networkManager.Join(joinSettings, ClientKicked, GameDataRecived);
    public void HostSucceed() => monobehaviourActions.Enqueue(() => { menuUI.HostConnectionSuccessful(); });
    public void HostFailed(string reason) => monobehaviourActions.Enqueue(() => { menuUI.HostFailed(reason); });
    public void JoinSucceed() => monobehaviourActions.Enqueue(() => { menuUI.JoinConnectionSuccessful(); });
    public void JoinFailed(string reason) => monobehaviourActions.Enqueue(() => { menuUI.JoinFailed(reason); });
    public void ClientKicked(string reason) => monobehaviourActions.Enqueue(() => { menuUI.ClientKicked(reason); });
    public void GameDataRecived(int gameMode, byte[] saveData)
    {
        foreach (AbstractGameManagerData g in GameManagersData)
        {
            if (g.GetUID() == gameMode)
            {
                Debug.Log(g.GetName());
                currentGameManager = g;
                break;
            }
        }
        this.saveData = saveData;
        monobehaviourActions.Enqueue(menuUI.GamemodeDataRecieve);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1); // Load main scene
        InGame = true;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(1); // Load menu scene
        InGame = false;
    }

    public void OnTurn()
    {
        inputManager.UpdatePossibleMoves(gameManager.GetMoves());
    }
    
    public void Update()
    {
        while (monobehaviourActions.Count > 0)
        {
            monobehaviourActions.Dequeue().Invoke();
        }
    }
}