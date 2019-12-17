using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

[Serializable]
public class TechStrategyOptions : ScriptableObject
{
    public virtual void InspectorDraw() {}
}

public class TriggerTechStrategyOptions : TechStrategyOptions
{
    public virtual TriggerTechStrategy GenerateStrategy()
    {
        return null;
    }
}
public class NoTriggerOptions : TriggerTechStrategyOptions
{
    public override TriggerTechStrategy GenerateStrategy()
    {
        return new NoTrigger();
    }
}

public class ActivateTechStrategyOptions: TechStrategyOptions
{
    public virtual ActivateTechStrategy GenerateStrategy()
    {
        return null;
    }
}
public class NoActivateOptions : ActivateTechStrategyOptions
{
    public override ActivateTechStrategy GenerateStrategy()
    {
        return new NoActivate();
    }
}

public class ActionValidateTechStrategyOptions: TechStrategyOptions
{
    public virtual ActionValidateTechStrategy GenerateStrategy()
    {
        return null;
    }
}
public class NoValidateOptions: ActionValidateTechStrategyOptions
{
    public override ActionValidateTechStrategy GenerateStrategy()
    {
        return new NoValidate();
    }
}
public class AllValidateOptions: ActionValidateTechStrategyOptions
{
    public override ActionValidateTechStrategy GenerateStrategy()
    {
        return new AllValidate();
    }
}

public class StateChangeStrategyOptions: TechStrategyOptions
{
    public virtual StateChangeStrategy GenerateStrategy()
    {
        return null;
    }
}
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
public class NoUpdateOptions: UpdateTechStrategyOptions
{
    public override UpdateTechStrategy GenerateStrategy()
    {
        return new NoUpdate();
    }
}

public class ExitTechStrategyOptions: TechStrategyOptions
{
    public virtual ExitTechStrategy GenerateStrategy()
    {
        return null;
    }
}
public class NoExitOptions : ExitTechStrategyOptions
{
    public override ExitTechStrategy GenerateStrategy()
    {
        return new NoExit();
    }
}