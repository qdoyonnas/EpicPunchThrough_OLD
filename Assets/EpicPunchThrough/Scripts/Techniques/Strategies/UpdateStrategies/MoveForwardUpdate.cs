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

    public void Update( Technique tech, GameManager.UpdateData data, float value )
    {
        if( tech.owner.physicsBody.velocity.magnitude > maxSpeed ) { return; }

        if( value < 0 && tech.owner.isFacingRight ) { tech.owner.isFacingRight = false; }
        else if( value > 0 && !tech.owner.isFacingRight ) { tech.owner.isFacingRight = true; }
        Vector3 velDelta = Vector3.right * (acceleration * data.deltaTime * value);

        tech.owner.physicsBody.AddVelocity(velDelta);
        tech.owner.HandlePhysics( data );
    }
}
