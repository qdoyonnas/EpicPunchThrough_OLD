using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager
{
    #region Settings

    [Serializable]
    public struct MenuSettings
    {
        public bool useController;
        public string[] openingMenuNames;
        public string pauseMenuName;
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
    [SerializeField] private EventSystem _eventSystem;
    public EventSystem eventSystem {
        get {
            return _eventSystem;
        }
    }

    [SerializeField] private string menuSceneName = "Menu";

    private bool isInitialized = false;
    public bool Initialize(MenuSettings settings)
    {
        this.settings = settings;

        if( _eventSystem == null ) {
            GameObject eventObject = GameObject.Find("EventSystem");
            if( eventObject != null ) {
                _eventSystem = eventObject.GetComponent<EventSystem>();
            }
            if( _eventSystem == null ) {
                Debug.LogError("MenuManager could not find EventSystem");
            }
        }

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
            if( menu.gameObject.name.ToLower() == name.ToLower() ) {
                return menu;
            }
        }

        return null;
    }

    void GameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        Menu pauseMenu;


        switch( newState ) {
            case GameManager.GameState.menu:
                if( !SceneManager.GetSceneByName(menuSceneName).isLoaded ) { SceneManager.LoadScene(menuSceneName, LoadSceneMode.Additive); }

                GameManager.Instance.activeCamera.Fade(0, GameManager.Instance.settings.sceneTransitionFadeDuration, true, () => {
                    foreach( string menuName in settings.openingMenuNames ) {
                        Menu loadMenu = GetMenu(menuName);
                        if( loadMenu != null ) {
                            loadMenu.TransitionIn();
                        } else {
                            Debug.LogError("MenuManager could not find '" + menuName + "' menu");
                        }
                    }
                });
                
                break;
            case GameManager.GameState.pause:
                pauseMenu = GetMenu(settings.pauseMenuName);
                if( pauseMenu != null ) {
                    pauseMenu.TransitionIn();
                } else {
                    Debug.LogError("MenuManager could not find '" + settings.pauseMenuName + "' menu");
                }

                break;
            default:
                if( previousState == GameManager.GameState.menu
                    && SceneManager.GetSceneByName(menuSceneName).isLoaded )
                { 
                    GameManager.Instance.activeCamera.Fade(1, 0);
                    SceneManager.UnloadSceneAsync(menuSceneName);
                } else if( previousState == GameManager.GameState.pause ) {
                    pauseMenu = GetMenu(settings.pauseMenuName);
                    if( pauseMenu != null && pauseMenu.inFocus ) {
                        pauseMenu.TransitionOut();
                    }
                }
                break;
        }
    }
    
    void DoUpdate(GameManager.UpdateData data)
    {
        if( GameManager.Instance.State != GameManager.GameState.menu && GameManager.Instance.State != GameManager.GameState.pause ) { return; }

        foreach( Menu menu in menues ) {
            menu.DoUpdate(data);
        }
    }
}
