using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : MonoBehaviour
{
    ParticleSystem _particleSystem;
    new public ParticleSystem particleSystem {
         get {
            return _particleSystem;
        }
    }

    #region Modules

    ParticleSystemRenderer particleRenderer;
    ParticleSystem.MainModule mainModule;
    ParticleSystem.CollisionModule collisionModule;
    ParticleSystem.ColorBySpeedModule colorBySpeedModule;
    ParticleSystem.ColorOverLifetimeModule colorOverTimeModule;
    ParticleSystem.CustomDataModule customDataModule;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.ExternalForcesModule externalForcesModule;
    ParticleSystem.ForceOverLifetimeModule forceOverTimeModule;
    ParticleSystem.InheritVelocityModule inheritVelocityModule;
    ParticleSystem.LightsModule lightsModule;
    ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverTimeModule;
    ParticleSystem.NoiseModule noiseModule;
    ParticleSystem.RotationBySpeedModule rotationBySpeedModule;
    ParticleSystem.SizeBySpeedModule sizeBySpeedModule;
    ParticleSystem.SizeOverLifetimeModule sizeOverTimeModule;
    ParticleSystem.TrailModule trailModule;
    ParticleSystem.TriggerModule triggerModule;
    ParticleSystem.ShapeModule shapeModule;
    ParticleSystem.VelocityOverLifetimeModule velOverTimeModule;

    #endregion

    float lifeStamp;

    private void Awake()
    {
        CreateParticleSystem();
    }
    protected virtual void CreateParticleSystem()
    {
        _particleSystem = gameObject.AddComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        // Modules - for ease of access
        mainModule = _particleSystem.main;
        collisionModule = _particleSystem.collision; 
        colorBySpeedModule = _particleSystem.colorBySpeed;
        colorOverTimeModule = _particleSystem.colorOverLifetime;
        customDataModule = _particleSystem.customData;
        emissionModule = _particleSystem.emission;
        externalForcesModule = _particleSystem.externalForces;
        forceOverTimeModule = _particleSystem.forceOverLifetime;
        inheritVelocityModule = _particleSystem.inheritVelocity;
        lightsModule = _particleSystem.lights;
        limitVelocityOverTimeModule = _particleSystem.limitVelocityOverLifetime;
        noiseModule = _particleSystem.noise;
        rotationBySpeedModule = _particleSystem.rotationBySpeed;
        sizeBySpeedModule = _particleSystem.sizeBySpeed;
        sizeOverTimeModule = _particleSystem.sizeOverLifetime;
        trailModule = _particleSystem.trails;
        triggerModule = _particleSystem.trigger;
        shapeModule = _particleSystem.shape;
        velOverTimeModule = _particleSystem.velocityOverLifetime;

        // Default overrides
        mainModule.startSpeed = 0f;

        shapeModule.shapeType = ParticleSystemShapeType.Box;
        shapeModule.boxThickness = new Vector3(1, 1, 0);

        velOverTimeModule.enabled = true;
        velOverTimeModule.zMultiplier = 0f;
    }

    private void OnDestroy()
    {
        ParticleManager.Instance.RemoveEmitter(this);
    }

    public void Init()
    {
        /*lifeStamp = Time.time + options.lifeTime;
        particleRenderer.material = Resources.Load<Material>(options.material);
        SetPreset(options.preset);*/

        ParticleManager.Instance.AddEmitter(this);
    }

    public ParticleEmitter SetPreset( string preset )
    {
        

        return this;
    }
    protected virtual void SetDefaultPreset()
    {
        mainModule.startSpeed = 0f;

        shapeModule.shapeType = ParticleSystemShapeType.Box;
        shapeModule.boxThickness = new Vector3(1, 1, 0);

        velOverTimeModule.enabled = true;
        velOverTimeModule.zMultiplier = 0f;
    }

    public void DoUpdate( GameManager.UpdateData data )
    {
        if( Time.time >= lifeStamp ) {
            Destroy(gameObject);
            return;
        }
    }
}