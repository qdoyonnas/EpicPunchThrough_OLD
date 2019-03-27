using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpExit : ExitTechStrategy
{
    float jumpMultiplier;

    public JumpExit( float jumpMultiplier )
    {
        this.jumpMultiplier = jumpMultiplier;
    }

    public void Exit( Technique tech )
    {
        double charge = (tech.GetBlackboardData("charge") as double?) ?? 0.0;
        Vector3 jumpVector = tech.owner.aimDirection * ((float)charge * jumpMultiplier);

        tech.owner.physicsBody.AddVelocity(jumpVector);

        tech.SetBlackboardData("charge", 0f);
    }
}
