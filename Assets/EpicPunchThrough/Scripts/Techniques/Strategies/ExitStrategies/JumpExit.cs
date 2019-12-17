using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JumpExit : ExitTechStrategy
{
    float jumpMultiplier;

    public JumpExit( float jumpMultiplier )
    {
        this.jumpMultiplier = jumpMultiplier;
    }

    public override void Exit( Technique tech )
    {
        double charge = (tech.GetBlackboardData("charge") as double?) ?? 0.0;
        Vector3 jumpVector = tech.owner.aimDirection * ((float)charge * jumpMultiplier);

        tech.owner.physicsBody.AddVelocity(jumpVector);
        tech.owner.onLayer = 1;
        Transform rightFootAnchor = tech.owner.GetAnchor("FootR");
        Transform leftFootAnchor = tech.owner.GetAnchor("FootL");
        Vector3 emitterPosition = ( rightFootAnchor.position + leftFootAnchor.position ) / 2f;
        
        float mult = ((float)charge * jumpMultiplier) / 15f;
        tech.CreateEmitter("launch", emitterPosition, Vector3.SignedAngle(Vector3.up, jumpVector, Vector3.forward) )
            .Expand(mult)
            .Accelerate(mult);

        tech.SetBlackboardData("charge", 0f);
    }
}

public class JumpExitOptions : ExitTechStrategyOptions
{
    float jumpMultiplier;

    public override void InspectorDraw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Jump Multiplier");
        float multi = EditorGUILayout.FloatField(jumpMultiplier);
        EditorGUILayout.EndHorizontal();

        if( multi != jumpMultiplier ) {
            jumpMultiplier = multi;
            EditorUtility.SetDirty(this);
        }
    }

    public override ExitTechStrategy GenerateStrategy()
    {
        return new JumpExit(jumpMultiplier);
    }
}
