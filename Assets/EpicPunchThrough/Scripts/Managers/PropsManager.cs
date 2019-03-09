using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PropsManager : MonoBehaviour
{
    #region Settings

    [Serializable]
    public class PropsManagerSettings
    {
    }

    #endregion

    #region Static

    private static PropsManager instance;
    public static PropsManager Instance
    {
        get {
            if( instance == null ) {
                instance = new PropsManager();
                instance.Initialize(new PropsManagerSettings());
            }
            return instance;
        }
    }

    #endregion

    public PropsManagerSettings settings;

    private Transform propsObject;
    private List<Prop> props =  new List<Prop>();

    private bool isInitialized = false;
    public bool Initialize(PropsManagerSettings settings)
    {
        this.settings = settings;


        isInitialized = true;
        return isInitialized;
    }

    #region Prop Methods

    public void RegisterProp( Prop prop )
    {
        props.Add(prop);
    }
    public void UnregisterProp( Prop prop )
    {
        props.Remove(prop);
    }

    public struct PropSpawnData
    {
        public readonly Vector3 position;
        public readonly GameObject prefab;

        public PropSpawnData( Vector3 position, GameObject prefab )
        {
            this.position = position;
            this.prefab = prefab;
        }
    }
    public void SpawnProp( PropSpawnData data )
    {
        GetPropsObject();

        GameObject selectedPrefab = data.prefab;
        if( selectedPrefab == null ) {
            if( EnvironmentManager.Instance.GetEnvironment().propPrefabs.Length == 0 ) { return; }

            int selection = UnityEngine.Random.Range(0, EnvironmentManager.Instance.GetEnvironment().propPrefabs.Length-1);
            if( selectedPrefab = EnvironmentManager.Instance.GetEnvironment().propPrefabs[selection] ) {
                Debug.LogError("Random prop selection failed due to unexpected error");
                return;
            }
        }

        Prop spawnedProp = Instantiate<GameObject>(selectedPrefab, data.position, Quaternion.identity, propsObject).GetComponent<Prop>();
    }
    public Transform GetPropsObject()
    {
        if( propsObject != null && propsObject.gameObject.activeInHierarchy ) { return propsObject; }

        Scene gameScene = PlayManager.Instance.GetGameScene();
        if( !gameScene.IsValid() || !gameScene.isLoaded ) { Debug.LogError("PropsManager received Spawn request when Game Scene is not loaded"); return null; }

        GameObject[] roots = gameScene.GetRootGameObjects();
        foreach( GameObject root in roots ) {
            if( root.name == "Props" ) {
                propsObject = root.transform;
                return propsObject;
            }
        }

        propsObject = new GameObject("Props").transform;
        SceneManager.MoveGameObjectToScene(propsObject.gameObject, gameScene);
        return propsObject;
    }

    #endregion

    private void DoUpdate( GameManager.UpdateData data )
    {
        for( int i = props.Count - 1; i >= 0; i-- ) {
            props[i].DoUpdate( data );
        }
    }
}
