using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WallSlideUpdate : UpdateTechStrategy
{
    float frictionMultiplier;
    float gravityMultiplier;

    public WallSlideUpdate( float frictionMultiplier, float gravityMultiplier )
    {
        this.frictionMultiplier = frictionMultiplier;
        this.gravityMultiplier = gravityMultiplier;
    }

    public override void Update( Technique tech, GameManager.UpdateData data, float value )
    {
        Vector3? friction = new Vector3();
        Vector3? gravity = null;
        if( tech.owner.physicsBody.velocity.y >= 0 ) {
            gravity = EnvironmentManager.Instance.GetEnvironment().gravity * gravityMultiplier;
            tech.owner.animator.SetBool("WallRunning", true);
        } else {
            friction = tech.owner.physicsBody.frictionCoefficients * frictionMultiplier;
            tech.owner.animator.SetBool("WallRunning", false);
        }
        
        tech.owner.HandlePhysics( data, friction, gravity );
    }
}

public class WallSlideUpdateOptions : UpdateTechStrategyOptions
{
    float frictionMultiplier;
    float gravityMultiplier;

    public override void InspectorDraw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Friction Multiplier");
        float friction = EditorGUILayout.FloatField(frictionMultiplier);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Gravity Multiplier");
        float gravity = EditorGUILayout.FloatField(gravityMultiplier);
        EditorGUILayout.EndHorizontal();

        if( friction != frictionMultiplier ) {
            frictionMultiplier = friction;
            EditorUtility.SetDirty(this);
        }
        if( gravity != gravityMultiplier ) {
            gravityMultiplier = gravity;
            EditorUtility.SetDirty(this);
        }
    }

    public override UpdateTechStrategy GenerateStrategy()
    {
        return new WallSlideUpdate(frictionMultiplier, gravityMultiplier);
    }
}