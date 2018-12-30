using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Settings

    [Serializable]
    public struct GameOptions
    {
	    public float gravity;
	    public float maximumSpeed;
	    public WorldBounds worldBounds;
        public MenuManager.MenuSettings menuSettings;
    }

    [Serializable]
    public struct WorldBounds
    {
	    public bool leftBound;
	    public bool rightBound;
	    public bool topBound;
	    public bool bottombound;
	    public float minX;
	    public float maxX;
	    public float width
	    {
		    get { return maxX - minX; }
	    }
	    public float minY;
	    public float maxY;
	    public float height
	    {
		    get { return maxY - minY; }
	    }
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

    #endregion

    public GameOptions settings;
    public CameraBase activeCamera;

    public enum GameState {
        init,
        menu,
        play,
        pause,
        exit
    } GameState state;

    public delegate void StateChangeAction(GameState previouseState, GameState newState);
    public event StateChangeAction stateChanged;
    public void SetState(GameState state)
    {
        GameState oldState = this.state;
        this.state = state;

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
		}
        
        if( activeCamera == null ) {
            activeCamera = GameObject.Find( "CameraBase" ).GetComponent<CameraBase>();
        }
        
        InitializeManagers();
	}

    void InitializeManagers()
    {
        MenuManager.Instance.Initialize(settings.menuSettings);
    }

    #region UpdateEvents

    public struct UpdateData
    {
        public float timeScale;
        public GameState state;

        public UpdateData(float timeScale, GameState state)
        {
            this.timeScale = timeScale;
            this.state = state;
        }
    }
    public delegate void UpdateAction(UpdateData data);

    public event UpdateAction updated;
    private void Update()
    {
        UpdateAction handler = updated;
        if( handler != null ) {
            handler(new UpdateData(1, state));
        }

        if( state == GameState.init ) {
            SetState(GameState.menu);
        }
    }

    public event UpdateAction fixedUpdated;
    void FixedUpdate()
	{
        UpdateAction handler = fixedUpdated;
        if( handler != null ) {
            handler(new UpdateData(1, state));
        }
	}

    #endregion
}