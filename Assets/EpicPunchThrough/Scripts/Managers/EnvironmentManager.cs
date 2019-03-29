using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentManager
{
    #region Static

    private static EnvironmentManager instance;
    public static EnvironmentManager Instance
    {
        get {
            if( instance == null ) {
                instance = new EnvironmentManager();
                instance.Initialize(ScriptableObject.CreateInstance<EnvironmentSettings>());
            }
            return instance;
        }
    }

    #endregion

    public EnvironmentSettings settings;

    public delegate void EnvironmentChangeDelegate(Environment previousEnvironment, Environment nextEnvironment);
    public event EnvironmentChangeDelegate environmentChanged;

    Environment currentEnvironment;

    bool isInitialized = false;
    public bool Initialize( EnvironmentSettings settings )
    {
        this.settings = settings;
        if( isInitialized ) { return false; }

        return isInitialized = true;
    }

    public void LoadEnvironment( Scene scene, string environmentName )
    {
        if( currentEnvironment != null && currentEnvironment.name == environmentName ) { return; }

        Environment environment = GetEnvironment(environmentName);
        if( environment == null || environment.sceneName == string.Empty ) { return; }

        if( currentEnvironment != null ) {
            SceneManager.UnloadSceneAsync( currentEnvironment.sceneName );
        }
        if( Utilities.DoesSceneExist(environment.sceneName) ) {
            if( !SceneManager.GetSceneByName( environment.sceneName ).isLoaded ) {
                SceneManager.LoadScene( environment.sceneName, LoadSceneMode.Additive );
            }
        }
        
        Physics.gravity = environment.gravity;

        EnvironmentChangeDelegate handler = environmentChanged;
        if( handler != null ) {
            handler(currentEnvironment, environment);
        }
        currentEnvironment = environment;
    }

    public Environment GetEnvironment()
    {
        return currentEnvironment;
    }
    public Environment GetEnvironment( string environmentName )
    {
        return settings.environments[environmentName];
    }
}
