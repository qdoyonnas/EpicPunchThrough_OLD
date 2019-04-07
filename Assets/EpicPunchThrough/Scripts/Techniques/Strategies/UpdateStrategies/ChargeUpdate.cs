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

    public void Update( Technique tech, GameManager.UpdateData data, float value )
    {
        double charge = (tech.GetBlackboardData("charge") as double?) ?? 0.0;
        if( charge < minimumCharge ) {
            tech.SetBlackboardData("charge", minimumCharge);
        } else if( charge < maximumCharge ) {
            tech.SetBlackboardData("charge", charge + (chargeRate * data.deltaTime));
        }

        tech.owner.HandlePhysics( data, tech.owner.physicsBody.frictionCoefficients * frictionMultiplier );
    }

    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("ChargeUpdate Fields");
    }
}
