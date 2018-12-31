using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    #region Settings

    [Serializable]
    public struct InputSettings
    {
        public KeyCode[] upKey;
        public KeyCode[] rightKey;
        public KeyCode[] downKey;
        public KeyCode[] leftKey;
        public KeyCode[] confirmKey;
        public KeyCode[] cancelKey;
    }

    #endregion

    #region Static

    private static InputManager instance;
    public static InputManager Instance
    {
        get {
            if( instance == null ) {
                instance = new InputManager();
                instance.Initialize(new InputSettings());
            }
            return instance;
        }
    }

    #endregion

    #region Input Events

    public delegate void InputAction(bool isDown);

    public event InputAction AnyInput;

    public event InputAction UpInput;
    public event InputAction RightInput;
    public event InputAction DownInput;
    public event InputAction LeftInput;
    public event InputAction ConfirmInput;
    public event InputAction CancelInput;

    #endregion

    public InputSettings settings;

    private bool isInitialized = false;
    public bool Initialize(InputSettings settings)
    {
        this.settings = settings;
        
        GameManager.Instance.updated -= DoUpdate;
        GameManager.Instance.updated += DoUpdate;

        isInitialized = true;
        return isInitialized;
    }

    public bool GetInput(string input)
    {
        input = input.ToLower();

        switch( input ) {
            case "up":
                return GetInput(settings.upKey);
            case "right":
                return GetInput(settings.rightKey);
            case "down":
                return GetInput(settings.downKey);
            case "left":
                return GetInput(settings.leftKey);
            case "confirm":
                return GetInput(settings.confirmKey);
            case "cancel":
                return GetInput(settings.cancelKey);
            default:
                Debug.LogError("InputManager was requested unknown input: " + input);
                return false;
        }
    }
    public bool GetInput(KeyCode[] input)
    {
        foreach( KeyCode key in input ) {
            if( Input.GetKey(key) ) { return true; }
        }

        return false;
    }

    void CheckKey(KeyCode[] keys, InputAction action)
    {
        foreach( KeyCode key in keys ) {
            if( Input.GetKeyDown(key) && action != null ) { action(true); break; }
            if( Input.GetKeyUp(key) && action != null ) { action(false); break; }
        }
    }
    public void DoUpdate(GameManager.UpdateData data)
    {
        if( Input.anyKeyDown && AnyInput != null ) { AnyInput(true); }

        CheckKey(settings.upKey, UpInput);
        CheckKey(settings.rightKey, RightInput);
        CheckKey(settings.downKey, DownInput);
        CheckKey(settings.leftKey, LeftInput);
        CheckKey(settings.confirmKey, ConfirmInput);
        CheckKey(settings.cancelKey, CancelInput);
    }
}
