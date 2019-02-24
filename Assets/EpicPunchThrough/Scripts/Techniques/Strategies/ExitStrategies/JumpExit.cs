using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpExit : ExitTechStrategy
{
    public void Exit( Agent agent )
    {
        Debug.Log("In JumpExit.Exit");
        agent.rigidbody.velocity = new Vector3( agent.rigidbody.velocity.x, agent.rigidbody.velocity.y + 10f, 0 );
    }
}
