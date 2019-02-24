using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Technique
{
    public string name;
    protected Agent owner;
    protected RuntimeAnimatorController _animatorController;
    public RuntimeAnimatorController animatorController {
        get {
            return _animatorController;
        }
    }

    #region TechTrigger

    public struct TechTrigger {
        public readonly Agent.State state;
        public readonly Agent.Action[] sequence;

        public TechTrigger(Agent.State state, params Agent.Action[] actions)
        {
            this.state = state;
            sequence = actions;
        }
    }
    protected TechTrigger _techTrigger;
    public TechTrigger techTrigger {
        get {
            return _techTrigger;
        }
    }

    #endregion

    #region Strategies

    protected TriggerTechStrategy triggerStrategy = new NoTrigger();
    protected ActivateTechStrategy activateStrategy  = new NoActivate();
    protected ActionValidateTechStrategy validateStrategy = new NoValidate();
    protected UpdateTechStrategy updateStrategy = new NoUpdate();
    protected ExitTechStrategy exitStrategy = new NoExit();

    #endregion

    public Technique( Agent owner, string name, RuntimeAnimatorController animCtrl, TechTrigger techTrgr, 
        TriggerTechStrategy triggerStrategy, ActivateTechStrategy activateStrategy, 
        ActionValidateTechStrategy validateStrategy, UpdateTechStrategy updateStrategy, ExitTechStrategy exitStrategy )
    {
        if( owner == null || animCtrl == null ) { 
            Debug.LogError("Technique generated with empty arguments");
            return;
        }

        this.name = name;

        this.owner = owner;
        _techTrigger = techTrgr;
        _animatorController = animCtrl;

        this.triggerStrategy = triggerStrategy;
        this.activateStrategy = activateStrategy;
        this.validateStrategy = validateStrategy;
        this.updateStrategy = updateStrategy;
        this.exitStrategy = exitStrategy;

        owner.SubscribeToActionEvent(techTrigger.sequence[techTrigger.sequence.Length-1], OnTrigger);
    }

    #region Behaviour Methods

    /// <summary>
    /// Returns whether the technique allows the given action to take place during the techniques execution.
    /// May cause additional behaviour to take place at the same time.
    /// </summary>
    /// <param name="action">The action in question</param>
    /// <returns>Boolean indicating whether the action is allowed during the technique</returns>
    public virtual bool ValidateAction(Agent.Action action, bool state)
    {
        if( validateStrategy == null ) { return true; }

        return validateStrategy.Validate(owner, action, state);
    }

    public virtual void OnTrigger()
    {
        if( owner.ValidActiveTechnique()
            || ( techTrigger.state != Agent.State.Any 
                && owner.state != techTrigger.state ) ) 
        { return; }

        if( techTrigger.sequence.Length > 1 ) {
            for( int i = 1; i < techTrigger.sequence.Length; i++ ) {
                if( techTrigger.sequence[techTrigger.sequence.Length - i] != owner.ActionSequence[owner.ActionSequence.Length - i] ) {
                    return;
                }
            }
        }

        if( triggerStrategy == null || !triggerStrategy.Trigger(owner) ) { return; }
        owner.AddActivatingTechnique(this);
    }

    public virtual void Activate()
    {
        if( activateStrategy == null ) { return; }

        activateStrategy.Activate(owner);
    }

    public virtual void Update(GameManager.UpdateData data)
    {
        if( updateStrategy == null ) { return; }

        updateStrategy.Update(owner, data);
    }

    public virtual void Exit()
    {
        if( exitStrategy == null ) { return; }

        exitStrategy.Exit(owner);
    }

    #endregion

    #region Utility

    public override string ToString()
    {
        return string.Format("Tech: {0}: {1}", owner.gameObject.name, name);
    }

    public bool IsValid()
    {
        return owner != null;
    }

    #endregion
}
