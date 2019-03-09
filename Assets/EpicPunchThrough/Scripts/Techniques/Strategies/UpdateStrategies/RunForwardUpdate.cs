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
        if( agent.physicsBody.velocity.magnitude > maxSpeed ) { return; }

        Vector3 velDelta = Vector3.right * (acceleration * data.deltaTime);
        if( !agent.isFacingRight ) { velDelta = -velDelta; }

        agent.physicsBody.AddVelocity(velDelta);
    }
}
