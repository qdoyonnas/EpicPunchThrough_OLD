using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTechValidate : ActionValidateTechStrategy
{
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

    public bool Validate( Agent agent, Agent.Action action, bool state )
    {
        foreach( ActionState actionState in actionStates ) {
            if( actionState.action == action && actionState.state == state  ) {
                agent.TransitionTechnique(null, true);
                return true;
            }
        }

        return false;
    }
}