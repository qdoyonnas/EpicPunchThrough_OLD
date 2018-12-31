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
        public KeyCode upKey;
        public KeyCode rightKey;
        public KeyCode downKey;
        public KeyCode leftKey;
        public KeyCode confirmKey;
        public KeyCode cancelKey;
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

        if( Input.GetKeyDown(settings.upKey) && UpInput != null ) { UpInput(true); }
        if( Input.GetKeyUp(settings.upKey) && UpInput != null ) { UpInput(false); }

        if( Input.GetKeyDown(settings.rightKey) && RightInput != null ) { RightInput(true); }
        if( Input.GetKeyUp(settings.rightKey) && RightInput != null ) { RightInput(false); }

        if( Input.GetKeyDown(settings.downKey) && DownInput != null ) { DownInput(true); }
        if( Input.GetKeyUp(settings.downKey) && DownInput != null ) { DownInput(false); }

        if( Input.GetKeyDown(settings.leftKey) && LeftInput != null ) { LeftInput(true); }
        if( Input.GetKeyUp(settings.leftKey) && LeftInput != null ) { LeftInput(false); }

        if( Input.GetKeyDown(settings.confirmKey) && ConfirmInput != null ) { ConfirmInput(true); }
        if( Input.GetKeyUp(settings.confirmKey) && ConfirmInput != null ) { ConfirmInput(false); }

        if( Input.GetKeyDown(settings.cancelKey) && CancelInput != null ) { CancelInput(true); }
        if( Input.GetKeyUp(settings.cancelKey) && CancelInput != null ) { CancelInput(false); }
    }
}
