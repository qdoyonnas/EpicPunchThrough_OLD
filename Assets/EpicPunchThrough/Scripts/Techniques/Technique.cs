using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technique
{
    protected Actor owner;
    protected RuntimeAnimatorController _animatorController;
    public RuntimeAnimatorController animatorController {
        get {
            return _animatorController;
        }
    }

    public struct TechTrigger {
        public Actor.State state;
        public List<Actor.Action> sequence;

        public TechTrigger(Actor.State state)
        {
            this.state = state;
            sequence = new List<Actor.Action>();
        }
    }
    protected TechTrigger techTrigger;

    protected TriggerStrategy triggerStrategy;
    protected ValidateStrategy validateStrategy;
    protected UpdateStrategy updateStrategy;

    public Technique( Actor owner, RuntimeAnimatorController animCtrl, TechTrigger techTrgr, 
        TriggerStrategy triggerStrategy, ValidateStrategy validateStrategy, UpdateStrategy updateStrategy )
    {
        if( owner == null || animCtrl == null ) { 
            Debug.LogError("Technique generated with empty arguments");
            return;
        }

        this.owner = owner;
        techTrigger = techTrgr;
        _animatorController = animCtrl;

        this.triggerStrategy = triggerStrategy;
        this.validateStrategy = validateStrategy;
        this.updateStrategy = updateStrategy;

        owner.SubscribeToActionEvent(techTrigger.sequence[techTrigger.sequence.Count-1], OnTrigger);
    }

    #region Behaviour Methods

    /// <summary>
    /// Returns whether the technique allows the given action to take place during the techniques execution.
    /// May cause additional behaviour to take place at the same time.
    /// </summary>
    /// <param name="action">The action in question</param>
    /// <returns>Boolean indicating whether the action is allowed during the technique</returns>
    public virtual bool ValidateAction(Actor.Action action, bool state)
    {
        return validateStrategy.Validate(owner, action, state);
    }

    public virtual void Update(GameManager.UpdateData data)
    {
        updateStrategy.Update(owner, data);
    }
    public virtual void OnTrigger()
    {
        if( !( owner.activeTechnique != null
            || owner.state == techTrigger.state 
            || techTrigger.state == Actor.State.Any ) ) 
        { return; }

        // Check action sequence

        if( !triggerStrategy.Trigger(owner) ) { return; }
        owner.TransitionTechnique(this, false);
    }

    #endregion
}
