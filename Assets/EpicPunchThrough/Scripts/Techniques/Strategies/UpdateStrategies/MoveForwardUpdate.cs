using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MoveForwardUpdate : UpdateTechStrategy
{
    float maxSpeed;
    float acceleration;
    
    public MoveForwardUpdate(float maxSpeed, float acceleration)
    {
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
    }

    public override void Update( Technique tech, GameManager.UpdateData data, float value )
    {
        if( Mathf.Abs(tech.owner.physicsBody.velocity.x) < maxSpeed
            || Mathf.Sign(value) != Mathf.Sign(tech.owner.physicsBody.velocity.x) )
        {
            Vector3 velDelta = (tech.owner.isFacingRight ? Vector3.right : Vector3.left) * (acceleration * data.deltaTime * value);

            tech.owner.physicsBody.AddVelocity(velDelta);
        }

        tech.owner.HandlePhysics( data );
        tech.owner.HandleAnimation();
    }
}

[Serializable]
public class MoveForwardUpdateOptions : UpdateTechStrategyOptions {
    public float maxSpeed;
    public float acceleration;

    public override void InspectorDraw()
    {
        maxSpeed = EditorGUILayout.FloatField("Max Speed", maxSpeed);
        acceleration = EditorGUILayout.FloatField("Acceleration", acceleration);
    }

    public override UpdateTechStrategy GenerateStrategy()
    {
        return new MoveForwardUpdate(maxSpeed, acceleration);
    }
}
