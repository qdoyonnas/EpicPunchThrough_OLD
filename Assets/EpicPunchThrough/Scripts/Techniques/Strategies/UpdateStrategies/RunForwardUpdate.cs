using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunForwardUpdate : UpdateStrategy
{
    float maxSpeed;
    float acceleration;

    public RunForwardUpdate(float maxSpeed, float acceleration)
    {
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
    }

    public void Update( Actor actor, GameManager.UpdateData data )
    {
        if( actor.rigidbody.velocity.magnitude > maxSpeed ) {
             return;
        }

        Vector3 velDelta = Vector3.right * (acceleration * Time.fixedDeltaTime);
        if( actor.isFacingRight ) {
            actor.rigidbody.velocity += velDelta;
        } else {
            actor.rigidbody.velocity -= velDelta;
        }
    }
}
