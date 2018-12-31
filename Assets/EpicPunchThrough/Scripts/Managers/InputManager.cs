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
        
        GameManager.Instance.updated += DoUpdate;

        isInitialized = true;
        return isInitialized;
    }

    public void DoUpdate(GameManager.UpdateData data)
    {
        if( Input.anyKeyDown && AnyInput != null ) { AnyInput(true); }

        foreach( KeyCode key in settings.upKey ) {
            if( Input.GetKeyDown(key) && UpInput != null ) { UpInput(true); }
            if( Input.GetKeyUp(key) && UpInput != null ) { UpInput(false); }
        }

        foreach( KeyCode key in settings.rightKey ) {
            if( Input.GetKeyDown(key) && RightInput != null ) { RightInput(true); }
            if( Input.GetKeyUp(key) && RightInput != null ) { RightInput(false); }
        }

        foreach( KeyCode key in settings.downKey ) {
            if( Input.GetKeyDown(key) && DownInput != null ) { DownInput(true); }
            if( Input.GetKeyUp(key) && DownInput != null ) { DownInput(false); }
        }

        foreach( KeyCode key in settings.leftKey ) {
            if( Input.GetKeyDown(key) && LeftInput != null ) { LeftInput(true); }
            if( Input.GetKeyUp(key) && LeftInput != null ) { LeftInput(false); }
        }

        foreach( KeyCode key in settings.confirmKey ) {
            if( Input.GetKeyDown(key) && ConfirmInput != null ) { ConfirmInput(true); }
            if( Input.GetKeyUp(key) && ConfirmInput != null ) { ConfirmInput(false); }
        }

        foreach( KeyCode key in settings.cancelKey ) {
            if( Input.GetKeyDown(key) && CancelInput != null ) { CancelInput(true); }
            if( Input.GetKeyUp(key) && CancelInput != null ) { CancelInput(false); }
        }
    }
}
