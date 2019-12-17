using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChargeUpdate : UpdateTechStrategy
{
    float frictionMultiplier;

    double chargeRate;
    double minimumCharge;
    double maximumCharge;

    public ChargeUpdate( float frictionMultiplier, double chargeRate, double minimumCharge, double maximumCharge )
    {
        this.frictionMultiplier = frictionMultiplier;

        this.chargeRate = chargeRate;
        this.minimumCharge = minimumCharge;
        this.maximumCharge = maximumCharge;
    }

    public override void Update( Technique tech, GameManager.UpdateData data, float value )
    {
        double charge = (tech.GetBlackboardData("charge") as double?) ?? 0.0;
        if( charge < minimumCharge ) {
            tech.SetBlackboardData("charge", minimumCharge);
        } else if( charge < maximumCharge ) {
            tech.SetBlackboardData("charge", charge + (chargeRate * data.deltaTime));
        }

        if( tech.owner.slideParticle != null ) { tech.owner.slideParticle.enabled = true; }
        tech.owner.HandlePhysics( data, tech.owner.physicsBody.frictionCoefficients * frictionMultiplier );
    }
}

public class ChargeUpdateOptions : UpdateTechStrategyOptions
{
    float frictionMultiplier;
    double chargeRate;
    double minimumCharge;
    double maximumCharge;

    public override void InspectorDraw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Friction Multiplier");
        float friction = EditorGUILayout.FloatField(frictionMultiplier);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Charge Rate");
        double charge = EditorGUILayout.DoubleField(chargeRate);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Minimum Charge");
        double min = EditorGUILayout.DoubleField(minimumCharge);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Maximum Charge");
        double max = EditorGUILayout.DoubleField(maximumCharge);
        EditorGUILayout.EndHorizontal();

        if( friction != frictionMultiplier ) {
            frictionMultiplier = friction;
            EditorUtility.SetDirty(this);
        }
        if( charge != chargeRate ) {
            chargeRate = charge;
            EditorUtility.SetDirty(this);
        }
        if( min != minimumCharge ) {
            minimumCharge = min;
            EditorUtility.SetDirty(this);
        }
        if( max != maximumCharge ) {
            maximumCharge = max;
            EditorUtility.SetDirty(this);
        }
    }

    public override UpdateTechStrategy GenerateStrategy()
    {
        return new ChargeUpdate(frictionMultiplier, chargeRate, minimumCharge, maximumCharge);
    }
}
