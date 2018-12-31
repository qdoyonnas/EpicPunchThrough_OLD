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
                instance.Initialize(new MenuSettings());
            }
            return instance;
        }
    }

    #endregion

    public MenuSettings settings;
    private List<Menu> menues = new List<Menu>();

    private bool isInitialized = false;
    public bool Initialize(MenuSettings settings)
    {
        this.settings = settings;

        GameManager.Instance.stateChanged -= GameStateChanged;
        GameManager.Instance.stateChanged += GameStateChanged;
        GameManager.Instance.fixedUpdated -= DoUpdate;
        GameManager.Instance.fixedUpdated += DoUpdate;

        isInitialized = true;
        return isInitialized;
    }

    public void RegisterMenu(Menu menu)
    {
        menues.Add(menu);
    }
    public void UnregisterMenu(Menu menu)
    {
        menues.Remove(menu);
    }
    public Menu GetMenu(string name)
    {
        foreach( Menu menu in menues ) {
            if( menu.gameObject.name == name ) {
                return menu;
            }
        }

        return null;
    }

    void GameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        switch( newState ) {
            case GameManager.GameState.menu:
                if( previousState == GameManager.GameState.init ) {
                    if( !SceneManager.GetSceneByName("Menu").isLoaded ) { SceneManager.LoadScene("Menu", LoadSceneMode.Additive); }

                    GameManager.Instance.activeCamera.Fade(1, 0);
                    GameManager.Instance.activeCamera.Fade(0, settings.fadeInDuration);
                    GameManager.Instance.activeCamera.EndedFade += (x) => {
                        Menu introBar = GetMenu("IntroBar");
                        if( introBar != null ) {
                            introBar.TransitionIn();
                        } else {
                            Debug.LogError("MenuManager could not find IntroBar");
                        }
                    };
                }
                break;
            case GameManager.GameState.pause:
                // show pause menu
                break;
        }
    }
    
    void DoUpdate(GameManager.UpdateData data)
    {
        if( data.state != GameManager.GameState.menu && data.state != GameManager.GameState.pause ) { return; }

        foreach( Menu menu in menues ) {
            menu.DoUpdate(data);
        }
    }
}
