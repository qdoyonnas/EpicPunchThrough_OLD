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

    public struct TechniqueOptions {
        public string animatorControllerPath;

        public Agent.State state;
        public List<Agent.Action> actions;

        public TriggerStrategy triggerStrategy;
        public ValidateStrategy validateStrategy;
        public UpdateStrategy updateStrategy;
    }
    public void GenerateTechnique( Agent agent, TechniqueOptions options )
    {
        RuntimeAnimatorController animController = RetrieveAnimatorController(options.animatorControllerPath);
        if( animController == null ) {
            return;
        }
        Technique.TechTrigger techTrigger = GenerateTechTrigger(options);

        TriggerStrategy triggerStrategy = options.triggerStrategy == null ? new NoTrigger() : options.triggerStrategy;
        ValidateStrategy validateStrategy = options.validateStrategy == null ? new NoValidate() : options.validateStrategy;
        UpdateStrategy updateStrategy = options.updateStrategy == null ? new NoUpdate() : options.updateStrategy;

        Technique tech = new Technique(agent, animController, techTrigger, 
                                triggerStrategy, validateStrategy, updateStrategy);
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
        Technique.TechTrigger techTrigger = new Technique.TechTrigger(options.state);
        if( options.actions == null || options.actions.Count == 0 ) {
            Debug.LogError("Technique generated with no trigger actions. Probably undersired behaviour.");
            techTrigger.sequence.Add(Agent.Action.AttackForward);
        } else {
            techTrigger.sequence = new List<Agent.Action>(options.actions);
        }


        return techTrigger;
    }

    #endregion

    public void AddBaseMovementTechniques(Agent agent)
    {
        TechniqueOptions options = new TechniqueOptions();
        options.animatorControllerPath = "Base/BasicMove";
        options.state = Agent.State.Grounded;
        options.actions = new List<Agent.Action>();
        options.actions.Add(Agent.Action.MoveForward);
        options.updateStrategy = new RunForwardUpdate(10f, 6f);
        options.validateStrategy = new EndTechValidate( new EndTechValidate.ActionState(Agent.Action.MoveForward, false),
                                                        new EndTechValidate.ActionState(Agent.Action.MoveBack, true) );

        GenerateTechnique( agent, options );

        options.actions.Clear();
        options.actions.Add(Agent.Action.MoveBack);
        options.triggerStrategy = new FlipDirectionTrigger();

        GenerateTechnique( agent, options );
    }
}
