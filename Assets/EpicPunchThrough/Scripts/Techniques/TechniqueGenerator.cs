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
        public string name;
        public string animatorControllerPath;
               
        public Agent.State state;
        public Agent.Action[] actions;
               
        public TriggerTechStrategy triggerStrategy;
        public ActivateTechStrategy activateStrategy;
        public ActionValidateTechStrategy actionValidateStrategy;
        public UpdateTechStrategy updateStrategy;
        public ExitTechStrategy exitStrategy;

        public TechniqueOptions( string name, string animatorControllerPath, Agent.State state, Agent.Action[] actions, TriggerTechStrategy triggerStrategy, ActivateTechStrategy activateStrategy,
                    ActionValidateTechStrategy actionValidateStrategy, UpdateTechStrategy updateStrategy, ExitTechStrategy exitStrategy )
        {
            this.name = name;
            this.animatorControllerPath = animatorControllerPath;
            this.state = state;
            this.actions = actions;
            this.triggerStrategy = triggerStrategy;
            this.activateStrategy = activateStrategy;
            this.actionValidateStrategy = actionValidateStrategy;
            this.updateStrategy = updateStrategy;
            this.exitStrategy = exitStrategy;
        }
    }

    public void GenerateTechnique( Agent agent, TechniqueOptions options )
    {
        RuntimeAnimatorController animController = RetrieveAnimatorController(options.animatorControllerPath);
        if( animController == null ) {
            return;
        }
        Technique.TechTrigger techTrigger = GenerateTechTrigger(options);

        TriggerTechStrategy triggerStrategy = options.triggerStrategy == null ? new NoTrigger() : options.triggerStrategy;
        ActivateTechStrategy activateStrategy = options.activateStrategy == null ? new NoActivate() : options.activateStrategy;
        ActionValidateTechStrategy actionValidateStrategy = options.actionValidateStrategy == null ? new NoValidate() : options.actionValidateStrategy;
        UpdateTechStrategy updateStrategy = options.updateStrategy == null ? new NoUpdate() : options.updateStrategy;
        ExitTechStrategy exitStrategy = options.exitStrategy == null ? new NoExit() : options.exitStrategy;

        Technique tech = new Technique(agent, options.name,  animController, techTrigger, 
                                triggerStrategy, activateStrategy, actionValidateStrategy, 
                                updateStrategy, exitStrategy);
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
            "Run Forward",
            "Base/BasicMove",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.MoveForward },
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveForward, false),
                                new EndTechValidate.ActionState(Agent.Action.MoveBack, true),
                                new EndTechValidate.ActionState(Agent.Action.Jump, true) ),
            new RunForwardUpdate(10f, 40f),
            null
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Run Back",
            "Base/BasicMove",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.MoveBack },
            null,
            new FlipDirectionActivate(),
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveForward, true),
                                new EndTechValidate.ActionState(Agent.Action.MoveBack, false),
                                new EndTechValidate.ActionState(Agent.Action.Jump, true) ),
            new RunForwardUpdate(10f, 40f),
            null
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Jump",
            "Base/BasicJump",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.Jump },
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.Jump, false) ),
            null,
            new JumpExit()
        );
        GenerateTechnique( agent, options );
    }
}
