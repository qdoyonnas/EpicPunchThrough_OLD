using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class TechStrategy {
    public virtual bool InspectorDraw()
    {
        return false;
    }
}

public abstract class TriggerTechStrategy: TechStrategy {
    public abstract bool Trigger(Technique tech, float value );
}
public class NoTrigger: TriggerTechStrategy {
    public override bool Trigger(Technique tech, float value )
    {
        return true;
    }
}

public abstract class ActivateTechStrategy: TechStrategy {
    public abstract void Activate(Technique tech);
}
public class NoActivate : ActivateTechStrategy
{
    public override void Activate( Technique tech )
    {
        return;
    }
}

public abstract class ActionValidateTechStrategy: TechStrategy {
    public abstract bool Validate(Technique tech, Agent.Action action, float value);
}
public class NoValidate: ActionValidateTechStrategy {
    public override bool Validate(Technique tech, Agent.Action action, float value)
    {
        return false;
    }
}
public class AllValidate: ActionValidateTechStrategy {
    public override bool Validate(Technique tech, Agent.Action action, float value)
    {
        return true;
    }
}

public abstract class StateChangeStrategy: TechStrategy {
    public abstract void OnStateChange( Technique tech, Agent.State previousState, Agent.State newState );
}
public class EndTechStateChange: StateChangeStrategy {
    public override void OnStateChange( Technique tech, Agent.State previousState, Agent.State newState )
    {
        tech.owner.TransitionTechnique(null, false);
    }
}

public abstract class UpdateTechStrategy: TechStrategy {
    public abstract void Update(Technique tech, GameManager.UpdateData data, float value );
}
public class NoUpdate: UpdateTechStrategy {
    public override void Update(Technique tech, GameManager.UpdateData data, float value )
    {
        tech.owner.HandlePhysics( data );
        return;
    }
}

public abstract class ExitTechStrategy: TechStrategy {
    public abstract void Exit( Technique tech );
}
public class NoExit : ExitTechStrategy {
    public override void Exit( Technique tech ) {
        return;
    }
}