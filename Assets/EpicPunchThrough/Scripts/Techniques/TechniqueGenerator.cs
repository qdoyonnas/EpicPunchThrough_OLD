﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechniqueGenerator
{
    #region Static

    private static TechniqueGenerator instance;
    public static TechniqueGenerator Instance
    {
        get {
            if( instance == null ) {
                instance = new TechniqueGenerator();
            }
            return instance;
        }
    }

    #endregion

    public string animationControllerFolderPath = "AnimationControllers/";
    public string baseAnimationControllerPath = "Base/BaseCharacter";

    public struct TechniqueOptions {
        public string animationControllerPath;

        public Actor.State state;
        public List<Actor.Action> actions;

        public TriggerStrategy triggerStrategy;
        public ValidateStrategy validateStrategy;
        public UpdateStrategy updateStrategy;
    }
    public void GenerateTechnique( Actor actor, TechniqueOptions options )
    {
        RuntimeAnimatorController animController = RetrieveAnimatorController(options.animationControllerPath);
        if( animController == null ) {
            return;
        }
        Technique.TechTrigger techTrigger = GenerateTechTrigger(options);

        TriggerStrategy triggerStrategy = options.triggerStrategy == null ? new NoTrigger() : options.triggerStrategy;
        ValidateStrategy validateStrategy = options.validateStrategy == null ? new NoValidate() : options.validateStrategy;
        UpdateStrategy updateStrategy = options.updateStrategy == null ? new NoUpdate() : options.updateStrategy;

        Technique tech = new Technique(actor, animController, techTrigger, 
                                triggerStrategy, validateStrategy, updateStrategy);
        actor.AddTechnique(tech);
    }

    #region Generator Helper Functions

    RuntimeAnimatorController RetrieveAnimatorController( string path )
    {
        string controllerPath;
        if( path == null || path == string.Empty ) {
            controllerPath = animationControllerFolderPath + baseAnimationControllerPath;
        } else {
            controllerPath = animationControllerFolderPath + path;
        }
        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(controllerPath);
        if( controller == null ) {
            Debug.LogError("TechniqueGenerator could not find animator controller at: '" + controllerPath + "'");
        }

        return controller;
    }

    Technique.TechTrigger GenerateTechTrigger( TechniqueOptions options )
    {
        Technique.TechTrigger techTrigger = new Technique.TechTrigger(options.state);
        if( options.actions == null || options.actions.Count == 0 ) {
            Debug.LogError("Technique generated with no trigger actions. Probably undersired behaviour.");
            techTrigger.sequence.Add(Actor.Action.AttackForward);
        } else {
            techTrigger.sequence = new List<Actor.Action>(options.actions);
        }


        return techTrigger;
    }

    #endregion

    public void AddBaseMovementTechniques(Actor actor)
    {
        TechniqueOptions options = new TechniqueOptions();
        options.state = Actor.State.Grounded;
        options.actions = new List<Actor.Action>();
        options.actions.Add(Actor.Action.MoveForward);
        options.updateStrategy = new RunForwardUpdate(3f);

        GenerateTechnique( actor, options );

        options.actions.Clear();
        options.actions.Add(Actor.Action.MoveBack);
        options.updateStrategy = null;

        GenerateTechnique( actor, options );
    }
}
