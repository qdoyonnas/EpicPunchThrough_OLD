using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public interface TriggerTechStrategy {
    bool Trigger(Technique tech, float value );
    void InspectorDraw();
}
public class NoTrigger: TriggerTechStrategy {
    public bool Trigger(Technique tech, float value )
    {
        return true;
    }
    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("NoTrigger Fields");
    }
}

public interface ActivateTechStrategy {
    void Activate(Technique tech);
    void InspectorDraw();
}
public class NoActivate : ActivateTechStrategy
{
    public void Activate( Technique tech )
    {
        return;
    }
    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("NoActivate Fields");
    }
}

public interface ActionValidateTechStrategy {
    bool Validate(Technique tech, Agent.Action action, float value);
    void InspectorDraw();
}
public class NoValidate: ActionValidateTechStrategy {
    public bool Validate(Technique tech, Agent.Action action, float value)
    {
        return false;
    }
    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("NoValidate Fields");
    }
}
public class AllValidate: ActionValidateTechStrategy {
    public bool Validate(Technique tech, Agent.Action action, float value)
    {
        return true;
    }
    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("AllValidate Fields");
    }
}

public interface StateChangeStrategy {
    void OnStateChange( Technique tech, Agent.State previousState, Agent.State newState );
    void InspectorDraw();
}
public class EndTechStateChange: StateChangeStrategy {
    public void OnStateChange( Technique tech, Agent.State previousState, Agent.State newState )
    {
        tech.owner.TransitionTechnique(null, false);
    }
    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("EndTechStateChange Fields");
    }
}

public interface UpdateTechStrategy {
    void Update(Technique tech, GameManager.UpdateData data, float value );
    void InspectorDraw();
}
public class NoUpdate: UpdateTechStrategy {
    public void Update(Technique tech, GameManager.UpdateData data, float value )
    {
        tech.owner.HandlePhysics( data );
        return;
    }
    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("NoUpdate Fields");
    }
}

public interface ExitTechStrategy {
    void Exit( Technique tech );
    void InspectorDraw();
}
public class NoExit : ExitTechStrategy {
    public void Exit( Technique tech ) {
        return;
    }
    public void InspectorDraw()
    {
        EditorGUILayout.LabelField("NoExit Fields");
    }
}