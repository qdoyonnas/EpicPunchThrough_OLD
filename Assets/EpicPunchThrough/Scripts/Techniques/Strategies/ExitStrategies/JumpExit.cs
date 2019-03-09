using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpExit : ExitTechStrategy
{
    public void Exit( Agent agent )
    {
        Vector3 jumpVector = agent.aimDirection * 10f;

        agent.rigidbody.velocity = agent.rigidbody.velocity + jumpVector;
    }
}
