using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentManager
{
    #region Settings

    [Serializable]
    public struct EnvironmentSettings
    {
        public Environment[] environments;
    }
    public EnvironmentSettings settings;

    #endregion

    #region Static

    private static EnvironmentManager instance;
    public static EnvironmentManager Instance
    {
        get {
            if( instance == null ) {
                instance = new EnvironmentManager();
                instance.Initialize(new EnvironmentSettings());
            }
            return instance;
        }
    }

    #endregion

    // Key == order // 0 back -> front
    Dictionary<int, SpriteScroll[]> spriteScrolls;

    bool isInitialized = false;
    public bool Initialize( EnvironmentSettings settings )
    {
        this.settings = settings;
        if( isInitialized ) { return false; }

        spriteScrolls = new Dictionary<int, SpriteScroll[]>();

        return isInitialized = true;
    }

    public void ChangeEnvironment( Scene scene, string environmentName )
    {
        spriteScrolls.Clear();

        
    }

    public Environment GetEnvironment( string environmentName )
    {
        foreach( Environment env in settings.environments ) {
            if( env.name == environmentName ) {
                return env;
            }
        }

        return null;
    }
}
