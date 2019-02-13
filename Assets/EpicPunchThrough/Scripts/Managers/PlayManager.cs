using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManager
{
    #region Settings

    [Serializable]
    public struct PlayManagerSettings
    {
        public float groundHeight;
    }

    #endregion

    #region Static

    private static PlayManager instance;
    public static PlayManager Instance
    {
        get {
            if( instance == null ) {
                instance = new PlayManager();
                instance.Initialize(new PlayManagerSettings());
            }
            return instance;
        }
    }

    #endregion

    public PlayManagerSettings settings;

    [SerializeField] private string gameSceneName = "Game";

    private bool isInitialized = false;
    public bool Initialize(PlayManagerSettings settings)
    {
        this.settings = settings;

        GameManager.Instance.stateChanged -= GameStateChanged;
        GameManager.Instance.stateChanged += GameStateChanged;
        GameManager.Instance.fixedUpdated -= DoUpdate;
        GameManager.Instance.fixedUpdated += DoUpdate;

        isInitialized = true;
        return isInitialized;
    }

    public Scene GetGameScene()
    {
        return SceneManager.GetSceneByName(gameSceneName);
    }

    bool Pause(bool isDown)
    {
        if( isDown ) {
            GameManager.Instance.SetState(GameManager.GameState.pause);

            return true;
        }

        return false;
    }

    void GameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        switch( newState ) {
            case GameManager.GameState.play:
                InputManager.Instance.CancelInput += Pause;

                if( !SceneManager.GetSceneByName(gameSceneName).isLoaded ) {
                    SceneManager.LoadScene(gameSceneName, LoadSceneMode.Additive);

                    SceneManager.sceneLoaded += SetupPlay;
                } else if( AgentManager.Instance.playerAgent == null ) {
                    SetupPlay(GetGameScene(), LoadSceneMode.Additive);
                }

                if( previousState != GameManager.GameState.play 
                    || previousState != GameManager.GameState.pause )
                {
                    GameManager.Instance.activeCamera.Fade(0, GameManager.Instance.settings.sceneTransitionFadeDuration, true);
                }

                break;
            case GameManager.GameState.pause:
                // coexist with pause menu
                // pause all actions
                InputManager.Instance.CancelInput -= Pause;

                break;
            default:
                InputManager.Instance.CancelInput -= Pause;

                if( ( previousState == GameManager.GameState.play
                    || previousState == GameManager.GameState.pause )
                    && SceneManager.GetSceneByName(gameSceneName).isLoaded ) 
                {
                    GameManager.Instance.activeCamera.Fade(1, 0);
                    SceneManager.UnloadSceneAsync(gameSceneName);

                }
                break;
        }
    }
    void SetupPlay(Scene scene, LoadSceneMode mode)
    {
        SpawnPlayer(GetGameScene(), LoadSceneMode.Additive);
    }
    void SpawnPlayer(Scene scene, LoadSceneMode mode)
    {
        AgentManager.SpawnData spawnData = new AgentManager.SpawnData();
        spawnData.name = "Player";
        spawnData.position = new Vector3(0, settings.groundHeight, 0);
        spawnData.team = 0;
        spawnData.type = AgentManager.AgentType.player;
        AgentManager.Instance.SpawnAgent(spawnData);

        SceneManager.sceneLoaded -= SpawnPlayer;
    }

    void DoUpdate(GameManager.UpdateData data)
    {
    }
}
