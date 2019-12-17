using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlipDirectionActivate : ActivateTechStrategy
{
    public override void Activate( Technique tech )
    {
        tech.owner.isFacingRight = !tech.owner.isFacingRight;
        tech.owner.TransitionTechnique(null, false);
    }
}

public class FlipDirectionActivateOptions : ActivateTechStrategyOptions
{
    public override ActivateTechStrategy GenerateStrategy()
    {
        return new FlipDirectionActivate();
    }
}