using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipDirectionTrigger : TriggerStrategy
{
    public bool Trigger( Actor actor )
    {
        actor.isFacingRight = !actor.isFacingRight;

        return true;
    }
}
