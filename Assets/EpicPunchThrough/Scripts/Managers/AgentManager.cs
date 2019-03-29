﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AgentManager
{
    #region Static

    private static AgentManager instance;
    public static AgentManager Instance
    {
        get {
            if( instance == null ) {
                instance = new AgentManager();
                instance.Initialize(ScriptableObject.CreateInstance<AgentSettings>());
            }
            return instance;
        }
    }

    #endregion

    public AgentSettings settings;

    private List<Agent> agents = new List<Agent>();
    public Agent playerAgent;

    private Transform agentsObject;

    private bool isInitialized = false;
    public bool Initialize(AgentSettings settings)
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

    #region Agent Methods

    public void RegisterAgent(Agent agent)
    {
        agents.Add(agent);
    }
    public void UnregisterAgent(Agent agent)
    {
        agents.Remove(agent);
    }

    public enum AgentType {
        player,
        fighter,
        boss
    }
    public struct AgentSpawnData
    {
        public readonly Vector3 position;
        public readonly string name;
        public readonly AgentType type;
        public readonly int team;

        public AgentSpawnData( Vector3 position, string name, AgentType type, int team )
        {
            this.position = position;
            this.name = name;
            this.type = type;
            this.team = team;
        }
    }
    public void SpawnAgent(AgentSpawnData data)
    {
        GetAgentsObject();

        GameObject agentObject = GameObject.Instantiate(settings.baseCharacterPrefab, data.position, Quaternion.identity);
        agentObject.transform.parent = agentsObject;

        Agent agent;
        switch( data.type ) {
            case AgentType.player:
                agent = agentObject.AddComponent<PlayerAgent>();
                break;
            default:
                agent = agentObject.AddComponent<Agent>();
                break;
        }
        agent.Init(settings.baseCharacterController, data.team);
        agent.SetName(data.name);
        TechniqueGenerator.Instance.AddBaseMovementTechniques(agent);
    }
    public Transform GetAgentsObject()
    {
        if( agentsObject != null && agentsObject.gameObject.activeInHierarchy ) { return agentsObject; }

        Scene gameScene = PlayManager.Instance.GetGameScene();
        if( !gameScene.IsValid() || !gameScene.isLoaded ) { Debug.LogError("AgentManager received Spawn request when Game Scene is not loaded"); return null; }

        GameObject[] roots = gameScene.GetRootGameObjects();
        foreach( GameObject root in roots ) {
            if( root.name == "Agents" ) {
                agentsObject = root.transform;
                return agentsObject;
            }
        }

        agentsObject = new GameObject("Agents").transform;
        SceneManager.MoveGameObjectToScene(agentsObject.gameObject, gameScene);
        return agentsObject;
    }

    #endregion

    private void DoUpdate( GameManager.UpdateData data )
    {
        for( int i = agents.Count-1; i >= 0; i-- ) {
            agents[i].DoUpdate(data);
        }
    }
}
