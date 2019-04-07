using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(menuName = "Project/Effects/ParticleController")]
public class ParticleController : ScriptableObject
{
    [Serializable]
    public class ParticleDictionary : SerializableDictionaryBase<string, GameObject> { }
    [SerializeField] protected ParticleDictionary particleSystems;

    public GameObject GetParticles(string name)
    {
        if( !particleSystems.ContainsKey(name) ) { return null; }
        GameObject particles = particleSystems[name];
        if( particles == null || particles.GetComponent<ParticleSystem>() == null ) { return null; }

        return particles;
    }
}
