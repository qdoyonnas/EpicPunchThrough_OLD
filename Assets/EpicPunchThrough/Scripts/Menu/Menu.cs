using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for Menu objects
/// </summary>
public class Menu: MonoBehaviour
{
    public bool inFocus = false;
    protected GameObject[] menuAssets;

    public float transitionTime = 0.4f;

    public virtual void DoUpdate(GameManager.UpdateData data) {}
    public virtual void TransitionIn() {}
    public virtual void TransitionOut() {}
}
