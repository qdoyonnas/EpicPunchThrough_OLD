using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipDirectionActivate : ActivateStrategy
{
    public void Activate( Agent agent )
    {
        agent.isFacingRight = !agent.isFacingRight;
    }
}