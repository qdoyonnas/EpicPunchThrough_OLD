﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Technique
{
    public string name;
    protected Agent _owner;
    public Agent owner {
        get {
            return _owner;
        }
    }
    protected RuntimeAnimatorController _animatorController;
    public RuntimeAnimatorController animatorController {
        get {
            return _animatorController;
        }
    }

    protected ParticleController _particleController;
    public ParticleController particleController {
        get {
            return _particleController;
        }
    }

    protected Dictionary<string, object> blackboard = new Dictionary<string, object>();

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
    protected StateChangeStrategy stateStrategy = new EndTechStateChange();
    protected ActionValidateTechStrategy validateStrategy = new NoValidate();
    protected UpdateTechStrategy updateStrategy = new NoUpdate();
    protected ExitTechStrategy exitStrategy = new NoExit();

    #endregion

    public Technique( Agent owner, string name, RuntimeAnimatorController animCtrl, ParticleController particleController, TechTrigger techTrgr, 
        TriggerTechStrategy triggerStrategy, ActivateTechStrategy activateStrategy, StateChangeStrategy stateStrategy,
        ActionValidateTechStrategy validateStrategy, UpdateTechStrategy updateStrategy, ExitTechStrategy exitStrategy )
    {
        if( owner == null || animCtrl == null ) { 
            Debug.LogError("Technique generated with empty arguments");
            return;
        }

        this.name = name;

        this._owner = owner;
        _techTrigger = techTrgr;
        _animatorController = animCtrl;
        _particleController = particleController;

        this.triggerStrategy = triggerStrategy;
        this.activateStrategy = activateStrategy;
        this.stateStrategy = stateStrategy;
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
    public virtual bool ValidateAction(Agent.Action action, float value)
    {
        if( validateStrategy == null ) { return true; }

        return validateStrategy.Validate(this, action, value);
    }
    public virtual void OnTrigger( float value )
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

        if( triggerStrategy == null || !triggerStrategy.Trigger(this, value) ) { return; }
        owner.AddActivatingTechnique(this);
    }
    public virtual void OnStateChange( Agent.State previousState, Agent.State newState )
    {
        if( stateStrategy == null ) { return; }

        stateStrategy.OnStateChange( this, previousState, newState );
    }
    public virtual void Activate()
    {
        if( activateStrategy == null ) { return; }

        activateStrategy.Activate(this);
    }
    public virtual void Update( GameManager.UpdateData data, float value )
    {
        if( updateStrategy == null ) { return; }

        updateStrategy.Update( this, data, value );
    }
    public virtual void Exit()
    {
        if( exitStrategy == null ) { return; }

        exitStrategy.Exit(this);
    }

    #endregion

    public void SetBlackboardData( string key, object data )
    {
        string l_key = key.ToLower();
        blackboard[l_key] = data;
    }
    public object GetBlackboardData( string key )
    {
        string l_key = key.ToLower();

        if( !blackboard.ContainsKey(l_key) ) {
            SetBlackboardData( l_key, null );
        }

        return blackboard[l_key];
    }
    public void ClearBlackboardData( string key )
    {
        string l_key = key.ToLower();
        blackboard.Remove(l_key);
    }

    #region Utility

    public override string ToString()
    {
        return string.Format("Tech: {0}: {1}", owner.gameObject.name, name);
    }

    public bool IsValid()
    {
        return owner != null;
    }

    public virtual ParticleEmitter CreateEmitter( string particleName, Vector3 position, float angle, Transform parent = null)
    {
        if( particleController == null ) { return null; }

        ParticleEmitter emitter = ParticleManager.Instance.CreateEmitter( position, angle, parent, particleController.GetParticles(particleName) );

        return emitter;
    }

    #endregion
}
