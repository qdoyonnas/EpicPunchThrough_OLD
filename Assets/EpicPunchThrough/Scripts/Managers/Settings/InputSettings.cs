using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Project/Settings/InputSettings")]
public class InputSettings : ScriptableObject
{
    [Header("Input Settings")]
    public bool mouseEnabled;
    public float activateMouseThreshold;
    public float pointerSensitivity;

    [Header("General Inputs")]
    public InputManager.Axis[] horizontal;
    public InputManager.Axis[] vertical;

    [Header("Menu Inputs")]
    public KeyCode[] confirmKey;
    public KeyCode[] cancelKey;
    public InputManager.Axis[] pointerHorizontal;
    public InputManager.Axis[] pointerVertical;

    [Header("Player Inputs")]
    public KeyCode[] attackKey;
    public KeyCode[] blockKey;
    public KeyCode[] jumpKey;
    public InputManager.Axis[] aimHorizontal;
    public InputManager.Axis[] aimVertical;
}
