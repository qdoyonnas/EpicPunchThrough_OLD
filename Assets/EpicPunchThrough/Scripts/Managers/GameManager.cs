using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Static

    private static GameManager instance;
    public static GameManager Instance
    {
        get {
            return instance;
        }
    }

    static string baseSceneName = "Base";
    public static bool BaseSceneIsLoaded()
    {
        return SceneManager.GetSceneByName(baseSceneName).isLoaded;
    }

    #endregion

    public GameManagerSettings settings;
    public CameraBase activeCamera;

    public bool DebugOn = false;

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
        PhysicsCollisionMatrix.Init();

        InputManager.Instance.Initialize(settings.inputSettings);
        MenuManager.Instance.Initialize(settings.menuSettings);
        SoundManager.Instance.Initialize(settings.soundSettings);
        AgentManager.Instance.Initialize(settings.agentSettings);
        PlayManager.Instance.Initialize(settings.playSettings);
        EnvironmentManager.Instance.Initialize(settings.environmentSettings);
        ParticleManager.Instance.Initialize(settings.particleSettings);
        PropsManager.Instance.Initialize(settings.propSettings);
    }

    public string GetManagerSceneName()
    {
        return gameObject.scene.name;
    }

    #region UpdateEvents

    public struct UpdateData
    {
        public readonly float timeScale;
        public readonly float deltaTime;

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