using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunForwardUpdate : UpdateStrategy
{
    float speed;

    public RunForwardUpdate(float speed)
    {
        this.speed = speed;
    }

    public void Update( Actor actor, GameManager.UpdateData data )
    {
        Debug.Log("IN RunForwardUpdate.Update");

        if( actor.isFacingRight ) {
            actor.rigidbody.velocity = new Vector3(speed, 0, 0);
        } else {
            actor.rigidbody.velocity = new Vector3(-speed, 0, 0);
        }
    }
}
