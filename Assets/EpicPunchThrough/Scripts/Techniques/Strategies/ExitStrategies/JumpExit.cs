using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpExit : ExitTechStrategy
{
    public void Exit( Technique tech )
    {
        Vector3 jumpVector = tech.owner.aimDirection * 10f;

        tech.owner.physicsBody.AddVelocity(jumpVector);
    }
}
