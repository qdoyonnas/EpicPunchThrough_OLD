using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TriggerStrategy {
    bool Trigger(Agent agent);
}
public class NoTrigger: TriggerStrategy {
    public bool Trigger(Agent agent)
    {
        return true;
    }
}

public interface ValidateStrategy {
    bool Validate(Agent agent, Agent.Action action, bool state);
}
public class NoValidate: ValidateStrategy {
    public bool Validate(Agent agent, Agent.Action action, bool state)
    {
        return false;
    }
}

public interface UpdateStrategy {
    void Update(Agent agent, GameManager.UpdateData data);
}
public class NoUpdate: UpdateStrategy {
    public void Update(Agent agent, GameManager.UpdateData data)
    {
        return;
    }
}