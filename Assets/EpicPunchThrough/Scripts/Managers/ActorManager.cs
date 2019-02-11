using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActorManager
{
    #region Settings

    [Serializable]
    public struct ActorSettings
    {
        public bool useController;
        public GameObject baseCharacterPrefab;
        public RuntimeAnimatorController baseCharacterController;
        public int actionSequenceLength;
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
    public Actor playerActor;

    private Transform actorsObject;

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

    #region Actor Methods

    public void RegisterActor(Actor actor)
    {
        actors.Add(actor);
    }
    public void UnregisterActor(Actor actor)
    {
        actors.Remove(actor);
    }

    public enum ActorType {
        player,
        fighter,
        boss
    }
    public struct SpawnData
    {
        public Vector3 position;
        public string name;
        public ActorType type;
        public int team;
    }
    public void SpawnActor(SpawnData data)
    {
        GetActorsObject();

        GameObject actorObject = GameObject.Instantiate(settings.baseCharacterPrefab, data.position, Quaternion.identity);
        actorObject.transform.parent = actorsObject;

        Actor actor;
        switch( data.type ) {
            case ActorType.player:
                actor = actorObject.AddComponent<PlayerActor>();
                break;
            default:
                actor = actorObject.AddComponent<Actor>();
                break;
        }
        actor.Init(settings.baseCharacterController, data.team);
        TechniqueGenerator.Instance.AddBaseMovementTechniques(actor);
    }
    public Transform GetActorsObject()
    {
        if( actorsObject != null && actorsObject.gameObject.activeInHierarchy ) { return actorsObject; }

        Scene gameScene = PlayManager.Instance.GetGameScene();
        if( !gameScene.IsValid() || !gameScene.isLoaded ) { Debug.LogError("ActorManager received Spawn request when Game Scene is not loaded"); return null; }

        GameObject[] roots = gameScene.GetRootGameObjects();
        foreach( GameObject root in roots ) {
            if( root.name == "Actors" ) {
                actorsObject = root.transform;
                return actorsObject;
            }
        }

        actorsObject = new GameObject("Actors").transform;
        return actorsObject;
    }

    #endregion

    private void DoUpdate( GameManager.UpdateData data )
    {
        for( int i = actors.Count-1; i >= 0; i-- ) {
            actors[i].DoUpdate(data);
        }
    }
}
