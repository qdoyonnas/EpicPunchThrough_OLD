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
        public StateChangeStrategy stateStrategy;
        public ActionValidateTechStrategy actionValidateStrategy;
        public UpdateTechStrategy updateStrategy;
        public ExitTechStrategy exitStrategy;

        public TechniqueOptions( string name, string animatorControllerPath, Agent.State state, Agent.Action[] actions, TriggerTechStrategy triggerStrategy, ActivateTechStrategy activateStrategy,
                    StateChangeStrategy stateStrategy, ActionValidateTechStrategy actionValidateStrategy, UpdateTechStrategy updateStrategy, ExitTechStrategy exitStrategy )
        {
            this.name = name;
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

    public void GenerateTechnique( Agent agent, TechniqueOptions options )
    {
        RuntimeAnimatorController animController = RetrieveAnimatorController(options.animatorControllerPath);
        if( animController == null ) {
            return;
        }
        Technique.TechTrigger techTrigger = GenerateTechTrigger(options);

        TriggerTechStrategy triggerStrategy = options.triggerStrategy == null ? new NoTrigger() : options.triggerStrategy;
        ActivateTechStrategy activateStrategy = options.activateStrategy == null ? new NoActivate() : options.activateStrategy;
        StateChangeStrategy stateStrategy = options.stateStrategy == null ? new EndTechStateChange() : options.stateStrategy;
        ActionValidateTechStrategy actionValidateStrategy = options.actionValidateStrategy == null ? new NoValidate() : options.actionValidateStrategy;
        UpdateTechStrategy updateStrategy = options.updateStrategy == null ? new NoUpdate() : options.updateStrategy;
        ExitTechStrategy exitStrategy = options.exitStrategy == null ? new NoExit() : options.exitStrategy;

        Technique tech = new Technique(agent, options.name,  animController, techTrigger, 
                                triggerStrategy, activateStrategy, stateStrategy, actionValidateStrategy, 
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
            "Base/Move",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.MoveRight },
            null,
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveRight, false),
                                new EndTechValidate.ActionState(Agent.Action.MoveLeft, true),
                                new EndTechValidate.ActionState(Agent.Action.Jump, true) ),
            new MoveForwardUpdate(10f, 40f),
            null
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Run Back",
            "Base/Move",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.MoveLeft },
            null,
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveRight, true),
                                new EndTechValidate.ActionState(Agent.Action.MoveLeft, false),
                                new EndTechValidate.ActionState(Agent.Action.Jump, true) ),
            new MoveForwardUpdate(10f, 40f),
            null
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Jump",
            "Base/Jump",
            Agent.State.Grounded,
            new Agent.Action[] { Agent.Action.Jump },
            null,
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.Jump, false) ),
            new ChargeUpdate(0.2f, 10f, 16f, 200f),
            new JumpExit(0.5f)
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Air Move Forward",
            "Base/BaseCharacter",
            Agent.State.InAir,
            new Agent.Action[] { Agent.Action.MoveRight },
            null,
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveRight, false),
                                new EndTechValidate.ActionState(Agent.Action.MoveLeft, true) ),
            new MoveForwardUpdate(3f, 10f),
            null
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Air Move Back",
            "Base/BaseCharacter",
            Agent.State.InAir,
            new Agent.Action[] { Agent.Action.MoveLeft },
            null,
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveRight, true),
                                new EndTechValidate.ActionState(Agent.Action.MoveLeft, false) ),
            new MoveForwardUpdate(2f, 6f),
            null
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Wall Slide",
            "Base/WallSlide",
            Agent.State.WallSliding,
            new Agent.Action[] { Agent.Action.MoveUp },
            null,
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveUp, false),
                                new EndTechValidate.ActionState(Agent.Action.Jump, true) ),
            new WallSlideUpdate(10f, 0.25f),
            null
        );
        GenerateTechnique( agent, options );

        options = new TechniqueOptions(
            "Wall Jump",
            "Base/WallSlide",
            Agent.State.WallSliding,
            new Agent.Action[] { Agent.Action.Jump },
            null,
            null,
            null,
            new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.Jump, false) ),
            new ChargeUpdate(10f, 10f, 16f, 200f),
            new JumpExit(0.5f)
        );
        GenerateTechnique( agent, options );
    }
}