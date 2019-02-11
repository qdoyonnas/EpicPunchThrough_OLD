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
        [Header("Menu Inputs")]
        public KeyCode[] upKey;
        public KeyCode[] rightKey;
        public KeyCode[] downKey;
        public KeyCode[] leftKey;
        public KeyCode[] confirmKey;
        public KeyCode[] cancelKey;
        [Header("Player Inputs")]
        public KeyCode[] attackKey;
        public KeyCode[] blockKey;
        public KeyCode[] jumpKey;
        public bool mouseEnabled;
        public float activateMouseThreshold;
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

    public delegate bool InputAction(bool isDown);
    public delegate bool MouseAction(Vector2 pos, Vector2 delta);

    /// <summary>
    /// Triggers on the frame that Unity's Input.anyKeyDown is true.
    /// WARNING: If registered to AnyInput event as well as other Input events, make sure to check the state the other keys inside the registered method."
    /// </summary>
    public event InputAction AnyInput;

    public event InputAction UpInput;
    public event InputAction RightInput;
    public event InputAction DownInput;
    public event InputAction LeftInput;

    // Menu Inputs
    public event InputAction ConfirmInput;
    public event InputAction CancelInput;

    // Player Inputs
    public event InputAction AttackInput;
    public event InputAction BlockInput;
    public event InputAction JumpInput;

    public event MouseAction MouseMovement;

    #endregion

    public InputSettings settings;

    Vector3 oldMousePosition = new Vector2();

    public delegate void ActiveControlAction(ActiveControlType previouseState, ActiveControlType newState);
    public event ActiveControlAction ActiveControlChanged;
    public enum ActiveControlType {
        Mouse,
        Keyboard,
        Controller
    }
    ActiveControlType _activeControlType = ActiveControlType.Keyboard;
    public ActiveControlType activeControlType {
        get {
            return _activeControlType;
        }
        set {
            if( ActiveControlChanged != null ) { ActiveControlChanged(_activeControlType, value); }
            _activeControlType = value;
        }
    }

    private bool isInitialized = false;
    public bool Initialize(InputSettings settings)
    {
        this.settings = settings;
        
        GameManager.Instance.updated -= DoUpdate;
        GameManager.Instance.updated += DoUpdate;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

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
    public bool IsNoInput()
    {
        return  !( GetInput(settings.upKey)
        || GetInput(settings.rightKey)
        || GetInput(settings.downKey)
        || GetInput(settings.leftKey)
        || GetInput(settings.confirmKey)
        || GetInput(settings.cancelKey)
        || GetInput(settings.attackKey)
        || GetInput(settings.blockKey)
        || GetInput(settings.jumpKey) );
    }

    public ActiveControlType GetInputType(KeyCode input)
    {
        // XXX: This might be really unreliable with any Unity updates.
        //      Consider either making settings to easily change values or
        //      finding better solution.
        if( (int)input >= 330 && (int)input <= 509 ) {
            return ActiveControlType.Controller;
        } else if( (int)input >= 323 && (int)input <= 329 ) {
            return ActiveControlType.Mouse;
        } else {
            return ActiveControlType.Keyboard;
        }
    }

    void CheckKey(KeyCode[] keys, InputAction action)
    {
        foreach( KeyCode key in keys ) {
            if( Input.GetKeyDown(key) && action != null ) {
                activeControlType = GetInputType(key); // XXX: Need way to distinguish between keyboard and controller keys
                action(true);
                break;
            }
            if( Input.GetKeyUp(key) && action != null ) {
                activeControlType = GetInputType(key);
                action(false);
                break;
            }
        }
    }
    public void DoUpdate(GameManager.UpdateData data)
    {
        ManageKeys();
        ManageMouse();
    }
    void ManageKeys()
    {
        if( Input.anyKeyDown && AnyInput != null ) { AnyInput(true); }

        CheckKey(settings.upKey, UpInput);
        CheckKey(settings.rightKey, RightInput);
        CheckKey(settings.downKey, DownInput);
        CheckKey(settings.leftKey, LeftInput);
        CheckKey(settings.confirmKey, ConfirmInput);
        CheckKey(settings.cancelKey, CancelInput);
        CheckKey(settings.attackKey, AttackInput);
        CheckKey(settings.blockKey, BlockInput);
        CheckKey(settings.jumpKey, JumpInput);
    }
    void ManageMouse()
    {
        if( activeControlType != ActiveControlType.Mouse 
            && Vector2.Distance(Input.mousePosition, oldMousePosition) >= settings.activateMouseThreshold )
        {
            activeControlType = ActiveControlType.Mouse;
        }
        if( activeControlType == ActiveControlType.Mouse
            && Input.mousePosition != oldMousePosition )
        {
            Vector2 delta = oldMousePosition - Input.mousePosition;
            if( MouseMovement != null ) {  MouseMovement(Input.mousePosition, delta); }
        }

        oldMousePosition = Input.mousePosition;
    }
}
