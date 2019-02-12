using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTechValidate : ValidateStrategy
{
    public struct ActionState {
        public Actor.Action action;
        public bool state;

        public ActionState(Actor.Action action, bool state)
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

    public bool Validate( Actor actor, Actor.Action action, bool state )
    {
        foreach( ActionState actionState in actionStates ) {
            if( actionState.action == action && actionState.state == state  ) {
                actor.TransitionTechnique(null, true);
                return true;
            }
        }

        return false;
    }
}