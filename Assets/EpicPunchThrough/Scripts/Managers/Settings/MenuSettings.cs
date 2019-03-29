using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Project/Settings/MenuSettings")]
public class MenuSettings : ScriptableObject
{
    public bool useController;
    public string pauseMenuName;
}
