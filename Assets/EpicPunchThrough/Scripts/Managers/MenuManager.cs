using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager
{
    #region Settings

    [Serializable]
    public struct MenuSettings
    {
        public bool useController;
        public float fadeInDuration;
    }

    #endregion

    #region Static

    private static MenuManager instance;
    public static MenuManager Instance
    {
        get {
            if( instance == null ) {
                instance = new MenuManager();
            }
            return instance;
        }
    }

    #endregion

    public MenuSettings settings;
    public Menu[] menues;

    private bool isInitialized = false;

    public bool Initialize(MenuSettings settings)
    {
        this.settings = settings;

        GameManager.Instance.stateChanged += GameStateChanged;
        GameManager.Instance.updated += DoUpdate;

        isInitialized = true;
        return isInitialized;
    }

    void GameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        switch( newState ) {
            case GameManager.GameState.menu:
                if( previousState == GameManager.GameState.init ) {
                    if( !SceneManager.GetSceneByName("Menu").isLoaded ) { SceneManager.LoadScene("Menu", LoadSceneMode.Additive); }

                    GameManager.Instance.activeCamera.Fade(1, 0);
                    GameManager.Instance.activeCamera.Fade(0, settings.fadeInDuration);
                }
                break;
            case GameManager.GameState.pause:
                // show pause menu
                break;
        }
    }

    void DoUpdate(GameManager.UpdateData data)
    {
        switch( data.state ) {
            case GameManager.GameState.menu:
                // update game menu
                break;
            case GameManager.GameState.pause:
                // update pause menu
                break;
        }
    }
}
