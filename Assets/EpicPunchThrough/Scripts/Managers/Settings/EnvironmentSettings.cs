using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(menuName = "Project/Settings/EnvironmentSettings")]
public class EnvironmentSettings : ScriptableObject
{
    [Serializable] public class EnvironmentDictionary : SerializableDictionaryBase<string, Environment> { }

    public EnvironmentDictionary environments;
}
