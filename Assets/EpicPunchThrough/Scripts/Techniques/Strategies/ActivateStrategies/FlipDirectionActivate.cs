using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipDirectionActivate : ActivateTechStrategy
{
    public void Activate( Technique tech )
    {
        tech.owner.isFacingRight = !tech.owner.isFacingRight;
    }
}