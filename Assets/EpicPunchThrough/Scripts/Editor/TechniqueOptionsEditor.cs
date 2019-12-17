using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TechniqueOptions))]
public class TechniqueOptionsEditor : Editor
{
    TechniqueOptions options;
    SerializedObject serializedOptions;

    #region StrategyData
    class StrategyData {
        public Type baseType;
        public string label;
        public bool show = true;

        Dictionary<string, Type> types;
        
        public StrategyEntry[] strategies;

        public StrategyData(TechniqueOptionsEditor window, Type baseType, string label, TechStrategyOptions[] strategies)
        {
            this.baseType = baseType;
            this.label = label;
            GetAllDerivedTypes();

            this.strategies = new StrategyEntry[strategies.Length];
            for( int i = 0; i < strategies.Length; i++ ) {
                if( strategies[i] != null ) {
                    int choice = Array.IndexOf(types.Keys.ToArray(), strategies[i].GetType().Name);
                    this.strategies[i] = new StrategyEntry(choice, strategies[i]);
                }
            }
        }

        void GetAllDerivedTypes()
        {
            Type[] derivedTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                        from assemblyType in domainAssembly.GetExportedTypes()
                        where baseType.IsAssignableFrom(assemblyType)
                        select assemblyType).ToArray();

            types = new Dictionary<string, Type>();
            foreach( Type type in derivedTypes ) {
                if( type == baseType ) { continue; }
                types.Add(type.Name, type);
            }
        }

        public bool DrawStratData()
        {
            bool update = false;

            EditorGUILayout.BeginHorizontal();
            show = EditorGUILayout.Foldout( show, label );
            int length = EditorGUILayout.IntField(strategies.Length, GUILayout.MaxWidth(50));
            EditorGUILayout.EndHorizontal();
            if( show ) {
                EditorGUILayout.BeginVertical();
                if( strategies.Length > 0 ) {
                    foreach( StrategyEntry entry in strategies ) {
                        if( entry != null ) {
                            if( entry.DrawStrategy(types) ) {
                                update = true;
                            }
                        }
                    }
                } else {
                    EditorGUILayout.LabelField("Default Strategy");
                }
                EditorGUILayout.EndVertical();
            }

            length = length == 0 ? 1 : length;
            if( length != strategies.Length ) {
                update = true;
                StrategyEntry[] oldData = strategies;
                strategies = new StrategyEntry[length];
                for( int i = 0; i < length; i++ ) {
                    if( i < oldData.Length ) {
                        strategies[i] = oldData[i];
                    } else {
                        strategies[i] = new StrategyEntry(0, (TechStrategyOptions)ScriptableObject.CreateInstance(types[types.Keys.First()]) );
                    }
                }
            }

            return update;
        }
    }
    #endregion

    #region StrategyEntry
    public class StrategyEntry {
        public int choice;
        public TechStrategyOptions strategy;
        public StrategyEntry( int c, TechStrategyOptions s )
        {
            choice = c;
            strategy = s;
        }

        public bool DrawStrategy(Dictionary<string, Type> types)
        {
            bool update = false;

            EditorGUILayout.BeginVertical("Box");
            int choice = EditorGUILayout.Popup(this.choice, types.Keys.ToArray());
            EditorGUI.indentLevel++;
            strategy.InspectorDraw();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            if( choice != this.choice ) {
                string name = types.Keys.ToArray()[choice];
                strategy = (TechStrategyOptions)ScriptableObject.CreateInstance( types[name] );
                this.choice = choice;
                update = true;
            }

            return update;
        }
    }
    #endregion

    StrategyData[] strategyDatas;

    private void ValidateOptions()
    {
        if( target == null ) { return; }
        options = (TechniqueOptions)target;
        serializedOptions = new SerializedObject(target);

        strategyDatas = new StrategyData[] {
            GenerateStrategyData<TriggerTechStrategyOptions>( "Trigger Strategies", ref options.triggerStrategies, typeof(NoTriggerOptions) ),
            GenerateStrategyData<ActivateTechStrategyOptions>( "Activate Strategies", ref options.activateStrategies, typeof(NoActivateOptions) ),
            GenerateStrategyData<StateChangeStrategyOptions>( "State Strategies", ref options.stateStrategies, typeof(EndTechStateChangeOptions) ),
            GenerateStrategyData<ActionValidateTechStrategyOptions>( "Action Strategies", ref options.actionValidateStrategies, typeof(NoValidateOptions) ),
            GenerateStrategyData<UpdateTechStrategyOptions>( "Update Strategies", ref options.updateStrategies, typeof(NoUpdateOptions) ),
            GenerateStrategyData<ExitTechStrategyOptions>( "Exit Strategies", ref options.exitStrategies, typeof(NoExitOptions) )
        };
    }
    private StrategyData GenerateStrategyData<T>( string label, ref T[] strategyOptions, Type defaultStrat )
        where T : TechStrategyOptions
    {
        if( strategyOptions == null ) {
            strategyOptions = new T[1];
        }

        for( int i = 0; i < strategyOptions.Length; i++ ) {
            if( strategyOptions[i] == null ) {
                strategyOptions[i] = (T)ScriptableObject.CreateInstance(defaultStrat);
            }
        }

        return new StrategyData( this, typeof(T), label, strategyOptions );
    }
    
    public override void OnInspectorGUI()
    {
        if( strategyDatas == null ) {
            ValidateOptions();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField( serializedOptions.FindProperty("techniqueName") );
        EditorGUILayout.PropertyField( serializedOptions.FindProperty("animatorController") );
        EditorGUILayout.PropertyField( serializedOptions.FindProperty("particleController") );

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Trigger", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField( serializedOptions.FindProperty("states"), true );
        EditorGUILayout.PropertyField( serializedOptions.FindProperty("actionSequence"), true );

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Strategies", EditorStyles.boldLabel);

        foreach( StrategyData data in strategyDatas ) {
            if( data.DrawStratData() ) {
                switch(data.baseType.Name) {
                    case "TriggerTechStrategyOptions":
                        options.triggerStrategies = new TriggerTechStrategyOptions[data.strategies.Length];
                        for( int i = 0; i < data.strategies.Length; i++ ) {
                            options.triggerStrategies[i] = (TriggerTechStrategyOptions)data.strategies[i].strategy;
                        }
                        break;
                    case "ActivateTechStrategyOptions":
                        options.activateStrategies = new ActivateTechStrategyOptions[data.strategies.Length];
                        for( int i = 0; i < data.strategies.Length; i++ ) {
                            options.activateStrategies[i] = (ActivateTechStrategyOptions)data.strategies[i].strategy;
                        }
                        break;
                    case "StateChangeStrategyOptions":
                        options.stateStrategies = new StateChangeStrategyOptions[data.strategies.Length];
                        for( int i = 0; i < data.strategies.Length; i++ ) {
                            options.stateStrategies[i] = (StateChangeStrategyOptions)data.strategies[i].strategy;
                        }
                        break;
                    case "ActionValidateTechStrategyOptions":
                        options.actionValidateStrategies = new ActionValidateTechStrategyOptions[data.strategies.Length];
                        for( int i = 0; i < data.strategies.Length; i++ ) {
                            options.actionValidateStrategies[i] = (ActionValidateTechStrategyOptions)data.strategies[i].strategy;
                        }
                        break;
                    case "UpdateTechStrategyOptions":
                        options.updateStrategies = new UpdateTechStrategyOptions[data.strategies.Length];
                        for( int i = 0; i < data.strategies.Length; i++ ) {
                            options.updateStrategies[i] = (UpdateTechStrategyOptions)data.strategies[i].strategy;
                        }
                        break;
                    case "ExitTechStrategyOptions":
                        options.exitStrategies = new ExitTechStrategyOptions[data.strategies.Length];
                        for( int i = 0; i < data.strategies.Length; i++ ) {
                            options.exitStrategies[i] = (ExitTechStrategyOptions)data.strategies[i].strategy;
                        }
                        break;
                }

                options.SetDirtyRecursive();
            }
        }

        serializedOptions.ApplyModifiedProperties();
    }
}
