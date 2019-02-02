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

        InputManager.Instance.CancelInput -= Pause;
        InputManager.Instance.CancelInput += Pause;

        isInitialized = true;
        return isInitialized;
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
                if( !SceneManager.GetSceneByName(gameSceneName).isLoaded ) {
                    SceneManager.LoadScene(gameSceneName, LoadSceneMode.Additive);
                    GameManager.Instance.activeCamera.Fade(0, GameManager.Instance.settings.sceneTransitionFadeDuration, true);
                }

                break;
            case GameManager.GameState.pause:
                // coexist with pause menu
                // pause all actions
                break;
            default:
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
    
    void DoUpdate(GameManager.UpdateData data)
    {
    }
}
