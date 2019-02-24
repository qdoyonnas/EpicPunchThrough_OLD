using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technique
{
    protected Agent owner;
    protected RuntimeAnimatorController _animatorController;
    public RuntimeAnimatorController animatorController {
        get {
            return _animatorController;
        }
    }

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

    protected TriggerStrategy triggerStrategy;
    protected ActivateStrategy activateStrategy;
    protected ActionValidateStrategy validateStrategy;
    protected UpdateStrategy updateStrategy;

    public Technique( Agent owner, RuntimeAnimatorController animCtrl, TechTrigger techTrgr, 
        TriggerStrategy triggerStrategy, ActivateStrategy activateStrategy, ActionValidateStrategy validateStrategy, UpdateStrategy updateStrategy )
    {
        if( owner == null || animCtrl == null ) { 
            Debug.LogError("Technique generated with empty arguments");
            return;
        }

        this.owner = owner;
        _techTrigger = techTrgr;
        _animatorController = animCtrl;

        this.triggerStrategy = triggerStrategy;
        this.activateStrategy = activateStrategy;
        this.validateStrategy = validateStrategy;
        this.updateStrategy = updateStrategy;

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
        return validateStrategy.Validate(owner, action, state);
    }

    public virtual void OnTrigger()
    {
        if( owner.activeTechnique != null
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

        if( !triggerStrategy.Trigger(owner) ) { return; }
        owner.AddActivatingTechnique(this);
    }

    public virtual void Activate()
    {
        activateStrategy.Activate(owner);
    }

    public virtual void Update(GameManager.UpdateData data)
    {
        updateStrategy.Update(owner, data);
    }


    #endregion
}
