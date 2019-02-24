using System.Collections;
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

    public string animatorControllerFolderPath = "AnimatorControllers/";
    public string baseAnimatorControllerPath = "Base/BaseCharacter";

    public class TechniqueOptions {
        public string animatorControllerPath;
               
        public Agent.State state;
        public Agent.Action[] actions;
               
        public TriggerStrategy triggerStrategy;
        public ActivateStrategy activateStrategy;
        public ActionValidateStrategy actionValidateStrategy;
        public UpdateStrategy updateStrategy;

        public TechniqueOptions( string animatorControllerPath, Agent.State state, Agent.Action[] actions, TriggerStrategy triggerStrategy, ActivateStrategy activateStrategy,
                    ActionValidateStrategy actionValidateStrategy, UpdateStrategy updateStrategy )
        {
            this.animatorControllerPath = animatorControllerPath;
            this.state = state;
            this.actions = actions;
            this.triggerStrategy = triggerStrategy;
            this.activateStrategy = activateStrategy;
            this.actionValidateStrategy = actionValidateStrategy;
            this.updateStrategy = updateStrategy;
        }
    }
    public void GenerateTechnique( Agent agent, TechniqueOptions options )
    {
        RuntimeAnimatorController animController = RetrieveAnimatorController(options.animatorControllerPath);
        if( animController == null ) {
            return;
        }
        Technique.TechTrigger techTrigger = GenerateTechTrigger(options);

        TriggerStrategy triggerStrategy = options.triggerStrategy == null ? new NoTrigger() : options.triggerStrategy;
        ActivateStrategy activateStrategy = options.activateStrategy == null ? new NoActivate() : options.activateStrategy;
        ActionValidateStrategy actionValidateStrategy = options.actionValidateStrategy == null ? new NoValidate() : options.actionValidateStrategy;
        UpdateStrategy updateStrategy = options.updateStrategy == null ? new NoUpdate() : options.updateStrategy;

        Technique tech = new Technique(agent, animController, techTrigger, 
                                triggerStrategy, activateStrategy, actionValidateStrategy, updateStrategy);
        agent.AddTechnique(tech);
    }

    #region Generator Helper Functions

    RuntimeAnimatorController RetrieveAnimatorController( string path )
    {
        string controllerPath;
        if( path == null || path == string.Empty ) {
            controllerPath = animatorControllerFolderPath + baseAnimatorControllerPath;
        } else {
            controllerPath = animatorControllerFolderPath + path;
        }
        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(controllerPath);
        if( controller == null ) {
            Debug.LogError("TechniqueGenerator could not find animator controller at: '" + controllerPath + "'");
        }

        return controller;
    }

    Technique.TechTrigger GenerateTechTrigger( TechniqueOptions options )
    {
        Technique.TechTrigger techTrigger;
        if( options.actions == null || options.actions.Length == 0 ) {
            Debug.LogError("Technique generated with no trigger actions. Probably undersired behaviour.");
            techTrigger = new Technique.TechTrigger(options.state);
        } else {
            techTrigger = new Technique.TechTrigger(options.state, options.actions);
        }

        return techTrigger;
    }

    #endregion

    public void AddBaseMovementTechniques(Agent agent)
    {
        TechniqueOptions options = new TechniqueOptions(
            "Base/BasicMove",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.MoveForward },
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveForward, false),
                                new EndTechValidate.ActionState(Agent.Action.MoveBack, true) ),
            new RunForwardUpdate(10f, 6f)
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Base/BasicMove",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.MoveBack },
            null,
            new FlipDirectionActivate(),
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveForward, false),
                                new EndTechValidate.ActionState(Agent.Action.MoveBack, true) ),
            new RunForwardUpdate(10f, 6f)
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Base/BasicJump",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.Jump },
            null,
            null,
            null,
            new JumpUpdate()
        );
        GenerateTechnique( agent, options );
    }
}
