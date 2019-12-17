using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EndTechValidate : ActionValidateTechStrategy
{
    [Serializable]
    public struct ActionState {
        public Agent.Action action;
        public bool state;

        public ActionState(Agent.Action action, bool state)
        {
            this.action = action;
            this.state = state;
        }
    }

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

public class EndTechValidateOptions: ActionValidateTechStrategyOptions
{
    [SerializeField]
    public EndTechValidate.ActionState[] actionStates;

    private SerializedObject serialized;

    public override void InspectorDraw()
    {
        if( serialized == null ) {
            serialized = new SerializedObject(this);
        }

        EditorGUILayout.PropertyField(serialized.FindProperty("actionStates"), true);

        serialized.ApplyModifiedProperties();
    }

    public override ActionValidateTechStrategy GenerateStrategy()
    {
        return new EndTechValidate(actionStates);
    }
}