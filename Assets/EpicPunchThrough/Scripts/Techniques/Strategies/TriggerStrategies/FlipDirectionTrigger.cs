using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipDirectionTrigger : TriggerStrategy
{
    public bool Trigger( Agent agent )
    {
        agent.isFacingRight = !agent.isFacingRight;

        return true;
    }
}
