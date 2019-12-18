using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

[Serializable]
public class TechStrategyOptions : ScriptableObject
{
    private void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;
    }

    public virtual void InspectorDraw() {}
}

[Serializable]
public class TriggerTechStrategyOptions : TechStrategyOptions
{
    public virtual TriggerTechStrategy GenerateStrategy()
    {
        return null;
    }
}
[Serializable]
public class NoTriggerOptions : TriggerTechStrategyOptions
{
    public override TriggerTechStrategy GenerateStrategy()
    {
        return new NoTrigger();
    }
}

[Serializable]
public class ActivateTechStrategyOptions: TechStrategyOptions
{
    public virtual ActivateTechStrategy GenerateStrategy()
    {
        return null;
    }
}
[Serializable]
public class NoActivateOptions : ActivateTechStrategyOptions
{
    public override ActivateTechStrategy GenerateStrategy()
    {
        return new NoActivate();
    }
}

[Serializable]
public class ActionValidateTechStrategyOptions: TechStrategyOptions
{
    public virtual ActionValidateTechStrategy GenerateStrategy()
    {
        return null;
    }
}
[Serializable]
public class NoValidateOptions: ActionValidateTechStrategyOptions
{
    public override ActionValidateTechStrategy GenerateStrategy()
    {
        return new NoValidate();
    }
}
[Serializable]
public class AllValidateOptions: ActionValidateTechStrategyOptions
{
    public override ActionValidateTechStrategy GenerateStrategy()
    {
        return new AllValidate();
    }
}

[Serializable]
public class StateChangeStrategyOptions: TechStrategyOptions
{
    public virtual StateChangeStrategy GenerateStrategy()
    {
        return null;
    }
}
[Serializable]
public class EndTechStateChangeOptions: StateChangeStrategyOptions
{
    public override StateChangeStrategy GenerateStrategy()
    {
        return new EndTechStateChange();
    }
}

[Serializable]
public class UpdateTechStrategyOptions: TechStrategyOptions
{
    public virtual UpdateTechStrategy GenerateStrategy()
    {
        return null;
    }
}
[Serializable]
public class NoUpdateOptions: UpdateTechStrategyOptions
{
    public override UpdateTechStrategy GenerateStrategy()
    {
        return new NoUpdate();
    }
}

[Serializable]
public class ExitTechStrategyOptions: TechStrategyOptions
{
    public virtual ExitTechStrategy GenerateStrategy()
    {
        return null;
    }
}
[Serializable]
public class NoExitOptions : ExitTechStrategyOptions
{
    public override ExitTechStrategy GenerateStrategy()
    {
        return new NoExit();
    }
}