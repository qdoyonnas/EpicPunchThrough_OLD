using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TriggerTechStrategy {
    bool Trigger(Agent agent);
}
public class NoTrigger: TriggerTechStrategy {
    public bool Trigger(Agent agent)
    {
        return true;
    }
}

public interface ActivateTechStrategy {
    void Activate(Agent agent);
}
public class NoActivate : ActivateTechStrategy
{
    public void Activate( Agent agent )
    {
        return;
    }
}

public interface ActionValidateTechStrategy {
    bool Validate(Agent agent, Agent.Action action, bool state);
}
public class NoValidate: ActionValidateTechStrategy {
    public bool Validate(Agent agent, Agent.Action action, bool state)
    {
        return false;
    }
}

public interface UpdateTechStrategy {
    void Update(Agent agent, GameManager.UpdateData data);
}
public class NoUpdate: UpdateTechStrategy {
    public void Update(Agent agent, GameManager.UpdateData data)
    {
        return;
    }
}

public interface ExitTechStrategy {
    void Exit( Agent agent );
}
public class NoExit : ExitTechStrategy {
    public void Exit( Agent agent ) {
        return;
    }
}