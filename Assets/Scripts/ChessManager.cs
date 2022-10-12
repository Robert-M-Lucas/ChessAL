using System;
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
    public AbstractGameManager gameManager;
    public bool InGame = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        // Loads all game managers
        GameManagersData = Util.GetAllGameManagers();
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == BuildIndex.MainMenu)
        {
            menuUI = GetComponent<MenuUIManager>();
        }
        else // Main Scene
        {
            inputManager = GetComponent<InputManager>();
        }
        
    }

    public void Host()
    {
        var host_settings = menuUI.GetHostSettings();
        currentGameManager = host_settings.GameManager;
        NetworkManager.Host(host_settings);
    }

    public void Join()
    {
        var join_settings = menuUI.GetJoinSettings();
        NetworkManager.Join(join_settings);
    }

    public void HostSucceed()
    {
        gameManager = currentGameManager.Instantiate();
        StartGame();
    }

    public void JoinSucceed(AbstractGameManagerData gameManagerData)
    {
        currentGameManager = gameManagerData;
        gameManager = currentGameManager.Instantiate();
        StartGame();
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
}