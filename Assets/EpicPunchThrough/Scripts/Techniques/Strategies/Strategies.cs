using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TriggerStrategy {
    bool Trigger(Actor actor);
}
public class NoTrigger: TriggerStrategy {
    public bool Trigger(Actor actor)
    {
        return true;
    }
}

public interface ValidateStrategy {
    bool Validate(Actor actor, Actor.Action action, bool state);
}
public class NoValidate: ValidateStrategy {
    public bool Validate(Actor actor, Actor.Action action, bool state)
    {
        return false;
    }
}

public interface UpdateStrategy {
    void Update(Actor actor, GameManager.UpdateData data);
}
public class NoUpdate: UpdateStrategy {
    public void Update(Actor actor, GameManager.UpdateData data)
    {
        return;
    }
}