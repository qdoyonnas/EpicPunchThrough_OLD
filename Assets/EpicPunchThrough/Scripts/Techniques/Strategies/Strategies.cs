using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TriggerTechStrategy {
    bool Trigger(Technique tech);
}
public class NoTrigger: TriggerTechStrategy {
    public bool Trigger(Technique tech)
    {
        return true;
    }
}

public interface ActivateTechStrategy {
    void Activate(Technique tech);
}
public class NoActivate : ActivateTechStrategy
{
    public void Activate( Technique tech )
    {
        return;
    }
}

public interface ActionValidateTechStrategy {
    bool Validate(Technique tech, Agent.Action action, float value);
}
public class NoValidate: ActionValidateTechStrategy {
    public bool Validate(Technique tech, Agent.Action action, float value)
    {
        return false;
    }
}
public class AllValidate: ActionValidateTechStrategy {
    public bool Validate(Technique tech, Agent.Action action, float value)
    {
        return true;
    }
}

public interface StateChangeStrategy {
    void OnStateChange( Technique tech, Agent.State previousState, Agent.State newState );
}
public class EndTechStateStrategy: StateChangeStrategy {
    public void OnStateChange( Technique tech, Agent.State previousState, Agent.State newState )
    {
        tech.owner.TransitionTechnique(null, false);
    }
}

public interface UpdateTechStrategy {
    void Update(Technique tech, GameManager.UpdateData data);
}
public class NoUpdate: UpdateTechStrategy {
    public void Update(Technique tech, GameManager.UpdateData data)
    {
        tech.owner.HandlePhysics( data );
        return;
    }
}

public interface ExitTechStrategy {
    void Exit( Technique tech );
}
public class NoExit : ExitTechStrategy {
    public void Exit( Technique tech ) {
        return;
    }
}