using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardUpdate : UpdateTechStrategy
{
    float maxSpeed;
    float acceleration;

    public MoveForwardUpdate(float maxSpeed, float acceleration)
    {
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
    }

    public void Update( Technique tech, GameManager.UpdateData data )
    {
        if( tech.owner.physicsBody.velocity.magnitude > maxSpeed ) { return; }

        Vector3 velDelta = Vector3.right * (acceleration * data.deltaTime);
        if( !tech.owner.isFacingRight ) { velDelta = -velDelta; }

        tech.owner.physicsBody.AddVelocity(velDelta);
        tech.owner.HandlePhysics( data );
    }
}
