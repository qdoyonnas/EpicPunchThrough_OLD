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

    public void Update( Technique tech, GameManager.UpdateData data, float value )
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

    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("WallSlideUpdate Fields");
    }
}