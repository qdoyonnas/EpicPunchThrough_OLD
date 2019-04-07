using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Project/Techniques/Technique")]
public class TechniqueOptions// : ScriptableObject
{
    public string techniqueName;
    public string animatorControllerPath;
               
    public Agent.State state;
    public Agent.Action[] actions;

    public TriggerTechStrategy triggerStrategy;
    public ActivateTechStrategy activateStrategy;
    public StateChangeStrategy stateStrategy;
    public ActionValidateTechStrategy actionValidateStrategy;
    public UpdateTechStrategy updateStrategy;
    public ExitTechStrategy exitStrategy;

    public TechniqueOptions( string techniqueName, string animatorControllerPath, Agent.State state,
                Agent.Action[] actions, TriggerTechStrategy triggerStrategy, ActivateTechStrategy activateStrategy,
                StateChangeStrategy stateStrategy, ActionValidateTechStrategy actionValidateStrategy,
                UpdateTechStrategy updateStrategy, ExitTechStrategy exitStrategy )
    {
        this.techniqueName = techniqueName;
        this.animatorControllerPath = animatorControllerPath;
        
        this.state = state;
        this.actions = actions;
        
        this.triggerStrategy = triggerStrategy;
        this.activateStrategy = activateStrategy;
        this.stateStrategy = stateStrategy;
        this.actionValidateStrategy = actionValidateStrategy;
        this.updateStrategy = updateStrategy;
        this.exitStrategy = exitStrategy;
    }
}
