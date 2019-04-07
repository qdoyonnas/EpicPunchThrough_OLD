using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TechniqueOptions))]
public class TechniqueOptionsEditor : Editor
{
    int triggerChoice = 0;
    int activateChoice = 0;
    int stateChoice = 0;
    int actionChoice = 0;
    int updateChoice = 0;
    int exitChoice = 0;

    Dictionary<string, Type> triggerStrats;
    Dictionary<string, Type> activateStrats;
    Dictionary<string, Type> stateChangeStrats;
    Dictionary<string, Type> actionValidateStrats;
    Dictionary<string, Type> updateStrats;
    Dictionary<string, Type> exitStrats;

    TechniqueOptions options;

    private void OnEnable()
    {
        //options = target as TechniqueOptions;

        GetAllDerivedTypes(typeof(TriggerTechStrategy), out triggerStrats);
        GetAllDerivedTypes(typeof(ActivateTechStrategy), out activateStrats);
        GetAllDerivedTypes(typeof(StateChangeStrategy), out stateChangeStrats);
        GetAllDerivedTypes(typeof(ActionValidateTechStrategy), out actionValidateStrats);
        GetAllDerivedTypes(typeof(UpdateTechStrategy), out updateStrats);
        GetAllDerivedTypes(typeof(ExitTechStrategy), out exitStrats);
    }
    void GetAllDerivedTypes(Type baseType, out Dictionary<string, Type> dictionary)
    {
        Type[] types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from assemblyType in domainAssembly.GetExportedTypes()
                    where baseType.IsAssignableFrom(assemblyType)
                    select assemblyType).ToArray();

        dictionary = new Dictionary<string, Type>();
        foreach( Type type in types ) {
            dictionary.Add(type.Name, type);
        }

        switch( baseType.Name ) {
            case "TriggerTechStrategy":
                if( options.triggerStrategy == null ) {
                    options.triggerStrategy = new NoTrigger();
                }
                triggerChoice = Array.IndexOf(triggerStrats.Keys.ToArray(), options.triggerStrategy.GetType().Name);
                break;
            case "ActivateTechStrategy":
                if( options.activateStrategy == null ) {
                    options.activateStrategy = new NoActivate();
                }
                activateChoice = Array.IndexOf(activateStrats.Keys.ToArray(), options.activateStrategy.GetType().Name);
                break;
            case "StateChangeStrategy":
                if( options.stateStrategy == null ) {
                    options.stateStrategy = new EndTechStateChange();
                }
                stateChoice = Array.IndexOf(stateChangeStrats.Keys.ToArray(), options.stateStrategy.GetType().Name);
                break;
            case "ActionValidateTechStrategy":
                if( options.actionValidateStrategy == null ) {
                    options.actionValidateStrategy = new NoValidate();
                }
                actionChoice = Array.IndexOf(actionValidateStrats.Keys.ToArray(), options.actionValidateStrategy.GetType().Name);
                break;
            case "UpdateTechStrategy":
                if( options.updateStrategy == null ) {
                    options.updateStrategy = new NoUpdate();
                }
                updateChoice = Array.IndexOf(updateStrats.Keys.ToArray(), options.updateStrategy.GetType().Name);
                break;
            case "ExitTechStrategy":
                if( options.exitStrategy == null ) {
                    options.exitStrategy = new NoExit();
                }
                exitChoice = Array.IndexOf(exitStrats.Keys.ToArray(), options.exitStrategy.GetType().Name);
                break;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Strategies", EditorStyles.boldLabel);

        DrawStratType("Trigger", triggerStrats, ref triggerChoice);
        DrawStratType("Activate", activateStrats, ref activateChoice);
        DrawStratType("StateChange", stateChangeStrats, ref stateChoice);
        DrawStratType("ActionValidate", actionValidateStrats, ref actionChoice);
        DrawStratType("Update", updateStrats, ref updateChoice);
        DrawStratType("Exit", exitStrats, ref exitChoice);
    }
    void DrawStratType(string label, Dictionary<string, Type> dictionary, ref int choice)
    {
        int oldChoice = choice;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField( label, GUILayout.MaxWidth(100f) );
        choice = EditorGUILayout.Popup(choice, dictionary.Keys.ToArray());
        EditorGUILayout.EndHorizontal();

        if( choice != oldChoice ) {
            switch( label ) {
                case "Trigger":

                    break;
                case "Activate":

                    break;
                case "StateChange":

                    break;
                case "ActionValidate":

                    break;
                case "Update":

                    break;
                case "Exit":

                    break;
            }
        }
    }
}
