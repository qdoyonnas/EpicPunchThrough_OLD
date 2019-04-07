using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlipDirectionActivate : ActivateTechStrategy
{
    public void Activate( Technique tech )
    {
        tech.owner.isFacingRight = !tech.owner.isFacingRight;
    }

    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("FlipDirectionActivate Fields");
    }
}