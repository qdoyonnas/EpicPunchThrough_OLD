using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AttackExit : ExitTechStrategy
{
    public float damageMultiplier;
    public float forceMultiplier;
    public float lungeMultiplier;

    public AttackExit( float damageMultiplier, float forceMultiplier, float lungeMultiplier )
    {
        this.damageMultiplier = damageMultiplier;
        this.forceMultiplier = forceMultiplier;
        this.lungeMultiplier = lungeMultiplier;
    }

    public override void Exit( Technique tech )
    {
        Debug.Log("In AttackExit");
    }
}

[System.Serializable]
public class AttackExitOptions : ExitTechStrategyOptions
{
    public float damageMultiplier;
    public float forceMultiplier;
    public float lungeMultiplier;

    public override void InspectorDraw()
    {
        damageMultiplier = EditorGUILayout.FloatField("Damage Multiplier", damageMultiplier);
        forceMultiplier = EditorGUILayout.FloatField("Force Multiplier", forceMultiplier);
        lungeMultiplier = EditorGUILayout.FloatField("Lunge Multiplier", lungeMultiplier);
    }

    public override ExitTechStrategy GenerateStrategy()
    {
        return new AttackExit(damageMultiplier, forceMultiplier, lungeMultiplier);
    }
}
