﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager
{
    #region Settings

    [Serializable]
    public struct ActorSettings
    {
        public bool useController;
        public float fadeInDuration;
        public string[] openingMenuNames;
    }

    #endregion

    #region Static

    private static ActorManager instance;
    public static ActorManager Instance
    {
        get {
            if( instance == null ) {
                instance = new ActorManager();
                instance.Initialize(new ActorSettings());
            }
            return instance;
        }
    }

    #endregion
    public ActorSettings settings;

    private List<Actor> actors = new List<Actor>();

    private bool isInitialized = false;
    public bool Initialize(ActorSettings settings)
    {
        this.settings = settings;

        GameManager.Instance.stateChanged += GameStateChanged;
        GameManager.Instance.fixedUpdated += DoUpdate;

        return true;
    }

    void GameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        switch( newState ) {
            case GameManager.GameState.play:

                break;
            default:

                break;
        }
    }

    private void DoUpdate( GameManager.UpdateData data )
    {
        for( int i = actors.Count; i >= 0; i-- ) {
            actors[i].DoUpdate(data);
        }
    }
}
