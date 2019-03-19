using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlideUpdate : UpdateTechStrategy
{
    public void Update( Technique tech, GameManager.UpdateData data )
    {
        tech.owner.HandlePhysics( data );

        tech.owner.physicsBody.SetVelocity( new Vector3(tech.owner.physicsBody.velocity.x, 0, 0) );
    }
}