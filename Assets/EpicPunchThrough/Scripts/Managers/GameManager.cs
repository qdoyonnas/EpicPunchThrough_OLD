using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Settings

    [Serializable]
    public struct GameOptions
    {
        public float sceneTransitionFadeDuration;
	    public WorldBounds cameraBounds;
        public MenuManager.MenuSettings menuSettings;
        public InputManager.InputSettings inputSettings;
        public SoundManager.SoundManagerSettings soundSettings;
        public PlayManager.PlayManagerSettings playSettings;
        public AgentManager.AgentSettings agentSettings;
        public EnvironmentManager.EnvironmentSettings environmentSettings;
    }

    #endregion

    #region Static

    private static GameManager instance;
    public static GameManager Instance
    {
        get {
            if( instance == null ) {
                Debug.LogError("Call to GameManager Instance before initialization. Will have default settings.");
                instance = new GameObject().AddComponent<GameManager>();
            }
            return instance;
        }
    }

    static string baseSceneName = "Base";
    public static bool BaseSceneIsLoaded()
    {
        return SceneManager.GetSceneByName(baseSceneName).isLoaded;
    }

    #endregion

    public GameOptions settings;
    public CameraBase activeCamera;

    public enum GameState {
        init,
        menu,
        play,
        pause,
        exit
    } GameState _state;
    public GameState State {
        get {
            return _state;
        }
    }

    [SerializeField] GameState initialState = GameState.menu;

    public delegate void StateChangeAction(GameState previouseState, GameState newState);
    public event StateChangeAction stateChanged;
    public void SetState(GameState state)
    {
        GameState oldState = _state;
        _state = state;

        StateChangeAction handler = stateChanged;
        if( handler != null ) {
            handler(oldState, state);
        }
    }
    
	void Awake()
	{
		if( instance == null ) {
			instance = this;
		} else {
			//XXX: hack unity singleton
			Debug.LogError("Two instances of GameManager. Destroying.");
            Destroy(gameObject);
            return;
		}
        
        if( activeCamera == null ) {
            activeCamera = GameObject.Find( "CameraBase" ).GetComponent<CameraBase>();
            if( activeCamera == null ) {
                Debug.LogError("GameManager did not find CameraBase. ActiveCamera not set");
            }
        }
        
        InitializeManagers();
	}

    void InitializeManagers()
    {
        InputManager.Instance.Initialize(settings.inputSettings);
        MenuManager.Instance.Initialize(settings.menuSettings);
        SoundManager.Instance.Initialize(settings.soundSettings);
        AgentManager.Instance.Initialize(settings.agentSettings);
        PlayManager.Instance.Initialize(settings.playSettings);
        EnvironmentManager.Instance.Initialize(settings.environmentSettings);
    }

    public string GetManagerSceneName()
    {
        return gameObject.scene.name;
    }

    #region UpdateEvents

    public struct UpdateData
    {
        public float timeScale;
        public float deltaTime;

        public UpdateData(float timeScale, float deltaTime)
        {
            this.timeScale = timeScale;
            this.deltaTime = deltaTime;
        }
    }
    public delegate void UpdateAction(UpdateData data);

    public event UpdateAction updated;
    private void Update()
    {
        UpdateAction handler = updated;
        if( handler != null ) {
            handler(new UpdateData(1, Time.deltaTime));
        }

        if( _state == GameState.init ) {
            activeCamera.Fade(1, 0);
            SetState(initialState);
        }
    }

    public event UpdateAction fixedUpdated;
    void FixedUpdate()
	{
        UpdateAction handler = fixedUpdated;
        if( handler != null ) {
            handler(new UpdateData(1, Time.fixedDeltaTime));
        }
	}

    #endregion
}