using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

[CustomEditor(typeof(Follow))]
[CanEditMultipleObjects]
public class FollowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Follow script = (Follow)target;

        script.target = (Transform)EditorGUILayout.ObjectField("Target", script.target, typeof(Transform), true);

        script.followX = EditorGUILayout.Toggle("Follow X", script.followX);
        script.followY = EditorGUILayout.Toggle("Follow Y", script.followY);
        script.followZ = EditorGUILayout.Toggle("Follow Z", script.followZ);
        script.followAngle = EditorGUILayout.Toggle("Follow Angle", script.followAngle);
        
        script.onFixedUpdate = EditorGUILayout.Toggle("On Fixed Update", script.onFixedUpdate);

        script.positionTrack = (Follow.TrackType)EditorGUILayout.EnumPopup("Position Track", script.positionTrack);
        switch( script.positionTrack ) {
            case Follow.TrackType.Instant:
                break;
            case Follow.TrackType.Tween:
                EditorGUILayout.LabelField("Move Tween Options", EditorStyles.boldLabel);
                script.moveEase = (Ease)EditorGUILayout.EnumPopup("Move Ease", script.moveEase);
                script.moveTime = EditorGUILayout.FloatField("Move Time", script.moveTime);
                break;
            case Follow.TrackType.Dynamic:
                EditorGUILayout.LabelField("Move Dynamic Options", EditorStyles.boldLabel);
                script.acceleration = EditorGUILayout.FloatField("Move Acceleration", script.acceleration);
                break;
        }

        script.rotationTrack = (Follow.TrackType)EditorGUILayout.EnumPopup("Rotation Track", script.rotationTrack);
        switch( script.rotationTrack ) {
            case Follow.TrackType.Instant:
                break;
            case Follow.TrackType.Tween:
                EditorGUILayout.LabelField("Rotate Tween Options", EditorStyles.boldLabel);
                script.rotateEase = (Ease)EditorGUILayout.EnumPopup("Rotation Ease", script.rotateEase);
                script.rotateTime = EditorGUILayout.FloatField("Rotate Time", script.rotateTime);
                break;
            case Follow.TrackType.Dynamic:
                EditorGUILayout.LabelField("Rotate Dynamic Options", EditorStyles.boldLabel);
                script.rotationAcceleration = EditorGUILayout.FloatField("Rotation Acceleration", script.rotationAcceleration);
                break;
        }

    }
}
