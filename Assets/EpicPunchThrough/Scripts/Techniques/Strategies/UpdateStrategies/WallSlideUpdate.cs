using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlideUpdate : UpdateTechStrategy
{
    float friction;

    public WallSlideUpdate( float friction )
    {
        this.friction = friction;
    }

    public void Update( Technique tech, GameManager.UpdateData data, float value )
    {
        tech.owner.HandlePhysics( data );

        tech.owner.physicsBody.SetVelocity( new Vector3(tech.owner.physicsBody.velocity.x, tech.owner.physicsBody.velocity.y * (1 - friction), 0) );
    }
}