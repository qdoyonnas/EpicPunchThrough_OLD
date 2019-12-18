using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Project/Settings/TechniqueSettings", order = 1)]
public class TechniqueSettings : ScriptableObject
{
    [Header("General")]
    public TechniqueSet[] defaultSets;

    [Header("Sets")]
    public TechniqueSet baseMovementSet;
    public TechniqueSet baseAttackSet;
}
