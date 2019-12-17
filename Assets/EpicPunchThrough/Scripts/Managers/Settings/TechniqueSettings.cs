using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Project/Settings/TechniqueSettings", order = 1)]
public class TechniqueSettings : ScriptableObject
{
    public TechniqueSet baseMovementSet;
    public TechniqueSet baseAttackSet;
}
