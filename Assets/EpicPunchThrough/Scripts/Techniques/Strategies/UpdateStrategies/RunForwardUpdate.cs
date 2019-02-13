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

    public void Update( Agent agent, GameManager.UpdateData data )
    {
        if( agent.rigidbody.velocity.magnitude > maxSpeed ) {
             return;
        }

        Vector3 velDelta = Vector3.right * (acceleration * Time.fixedDeltaTime);
        if( agent.isFacingRight ) {
            agent.rigidbody.velocity += velDelta;
        } else {
            agent.rigidbody.velocity -= velDelta;
        }
    }
}
