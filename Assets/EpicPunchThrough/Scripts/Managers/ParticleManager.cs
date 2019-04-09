using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RotaryHeart.Lib.SerializableDictionary;

public class ParticleManager
{
    #region Static

    private static ParticleManager instance;
    public static ParticleManager Instance
    {
        get {
            if( instance == null ) {
                instance = new ParticleManager();
                instance.Initialize(ScriptableObject.CreateInstance<ParticleSettings>());
            }
            return instance;
        }
    }

    #endregion

    public ParticleSettings settings;
    public List<ParticleEmitter> emitters = new List<ParticleEmitter>();

    private Transform particlesObject; 

    public bool Initialize( ParticleSettings settings )
    {
        this.settings = settings;

        GameManager.Instance.fixedUpdated += DoUpdate;

        return true;
    }

    private void DoUpdate( GameManager.UpdateData data )
    {
        for( int i = emitters.Count-1; i >= 0; i-- ) {
            emitters[i].DoUpdate( data );
        }
    }

    #region Emitter Methods

    public void AddEmitter( ParticleEmitter emitter )
    {
        emitters.Add(emitter);
    }

    public ParticleEmitter CreateEmitter( Vector3 position, float angle, Transform parent, GameObject particleSystem)
    {
        if( particleSystem == null ) { return null; }

        ParticleEmitter particle = new GameObject("Emitter").AddComponent<ParticleEmitter>();
        particle.transform.position = position;
        particle.transform.localEulerAngles = new Vector3(0, 0, angle);
        if( parent != null ) {
            particle.transform.parent = parent;
        } else {
            particlesObject = GetParticlesObject();
            particle.transform.parent = particlesObject;
        }

        particle.Init(particleSystem);

        return particle;
    }
    public Transform GetParticlesObject()
    {
        if( particlesObject != null && particlesObject.gameObject.activeInHierarchy ) { return particlesObject; }

        Scene gameScene = PlayManager.Instance.GetGameScene();
        if( !gameScene.IsValid() || !gameScene.isLoaded ) { Debug.LogError("AgentManager received Spawn request when Game Scene is not loaded"); return null; }

        GameObject[] roots = gameScene.GetRootGameObjects();
        foreach( GameObject root in roots ) {
            if( root.name == "ParticleEmitters" ) {
                particlesObject = root.transform;
                return particlesObject;
            }
        }

        particlesObject = new GameObject("ParticleEmitters").transform;
        SceneManager.MoveGameObjectToScene(particlesObject.gameObject, gameScene);
        return particlesObject;
    }

    public void RemoveEmitter( ParticleEmitter emitter )
    {
        emitters.Remove(emitter);
    }

    #endregion
}
