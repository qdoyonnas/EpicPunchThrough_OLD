using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunForwardUpdate : UpdateTechStrategy
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
        Vector3 velDelta = Vector3.right * (acceleration * data.deltaTime);
        float directionalVelocity = agent.rigidbody.velocity.x + Mathf.Abs(agent.rigidbody.velocity.y);

        if( agent.isFacingRight ) {
            if( directionalVelocity >= maxSpeed ) { return; }
            agent.rigidbody.velocity += velDelta;
        } else {
            if( directionalVelocity <= -maxSpeed ) { return; }
            agent.rigidbody.velocity -= velDelta;
        }
    }
}
