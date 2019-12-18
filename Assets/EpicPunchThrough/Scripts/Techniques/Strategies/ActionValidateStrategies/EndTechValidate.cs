using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EndTechValidate : ActionValidateTechStrategy
{
    public ActionState[] actionStates;

    public EndTechValidate(params ActionState[] actionStates)
    {
        this.actionStates = actionStates;
    }

    public override bool Validate( Technique tech, Agent.Action action, float value )
    {
        foreach( ActionState actionState in actionStates ) {
            if( actionState.action == action && actionState.state == (Mathf.Abs(value) > 0)  ) {
                tech.owner.TransitionTechnique(null, true);
                return true;
            }
        }

        return false;
    }
}

[Serializable]
public class ActionState {
    public Agent.Action action;
    public bool state;
}

[Serializable]
public class EndTechValidateOptions: ActionValidateTechStrategyOptions
{
    public ActionState[] actionStates = new ActionState[0];

    private bool show = true;

    public override void InspectorDraw()
    {
        EditorGUILayout.BeginHorizontal();
        show = EditorGUILayout.Foldout(show ,"Actions");
        int length = EditorGUILayout.IntField(actionStates.Length);
        EditorGUILayout.EndHorizontal();

        if( show ) {
            EditorGUILayout.BeginVertical();
            for( int i = 0; i < actionStates.Length; i++ ) {
                EditorGUILayout.BeginHorizontal();
                actionStates[i].action = (Agent.Action)EditorGUILayout.EnumPopup(actionStates[i].action);
                actionStates[i].state = EditorGUILayout.Toggle(actionStates[i].state);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        if( length != actionStates.Length ) {
            ActionState[] actions = actionStates;
            actionStates = new ActionState[length];
            for(int i = 0; i < length; i++) {
                if( i < actions.Length ) {
                    actionStates[i] = actions[i];
                } else {
                    actionStates[i] = new ActionState();
                }
            }
        }
    }

    public override ActionValidateTechStrategy GenerateStrategy()
    {
        return new EndTechValidate(actionStates);
    }
}