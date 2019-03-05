using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    #region Settings

    [Serializable]
    public class InputSettings
    {
        [Header("Input Settings")]
        public bool mouseEnabled;
        public float activateMouseThreshold;
        public float pointerSensitivity;

        [Header("General Inputs")]
        public Axis[] horizontal;
        public Axis[] vertical;

        [Header("Menu Inputs")]
        public KeyCode[] confirmKey;
        public KeyCode[] cancelKey;
        public Axis[] pointerHorizontal;
        public Axis[] pointerVertical;

        [Header("Player Inputs")]
        public KeyCode[] attackKey;
        public KeyCode[] blockKey;
        public KeyCode[] jumpKey;
        public Axis[] aimHorizontal;
        public Axis[] aimVertical;
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

    #region Input Enums

    public enum ActiveControlType {
        Mouse,
        Keyboard,
        Controller
    }

    public enum Axis {
        ArrowKeysHorizontal, ArrowKeysVertical, WASDHorizontal, WASDVertical, IJKLHorizontal, IJKLVertical, NumpadHorizontal, NumpadVertical,
        MouseInputs, // DO NOT USE
        MouseX, MouseY, ScrollWheel,
        JoystickInputs, // DO NOT USE
        Joystick0Axis0, Joystick0Axis1, Joystick0Axis2, Joystick0Axis3, Joystick0Axis4, Joystick0Axis5, Joystick0Axis6,
        Joystick1Axis0, Joystick1Axis1, Joystick1Axis2, Joystick1Axis3, Joystick1Axis4, Joystick1Axis5, Joystick1Axis6,
        Joystick2Axis0, Joystick2Axis1, Joystick2Axis2, Joystick2Axis3, Joystick2Axis4, Joystick2Axis5, Joystick2Axis6,
        Joystick3Axis0, Joystick3Axis1, Joystick3Axis2, Joystick3Axis3, Joystick3Axis4, Joystick3Axis5, Joystick3Axis6
    }

    #endregion

    #region Input Events

    public delegate bool InputAction(float value);

    /// <summary>
    /// Triggers on the frame that Unity's Input.anyKeyDown is true.
    /// WARNING: If registered to AnyInput event as well as other Input events, make sure to check the state the other keys inside the registered method."
    /// </summary>
    public event InputAction AnyInput;

    public event InputAction HorizontalInput;
    public event InputAction VerticalInput;

    // Menu Inputs
    public event InputAction ConfirmInput;
    public event InputAction CancelInput;
    public event InputAction PointerHorizontal;
    public event InputAction PointerVertical;

    // Player Inputs
    public event InputAction AttackInput;
    public event InputAction BlockInput;
    public event InputAction JumpInput;
    public event InputAction AimHorizontal;
    public event InputAction AimVertical;

    #endregion

    #region Axes Saved Values

    float storedHorizontalValue = 0;
    float storedVerticalValue = 0;
    float storedPointerHorizontalValue = 0;
    float storedPointerVerticalValue = 0;
    float storedAimHorizontalValue = 0;
    float storedAimVerticalValue = 0;

    #endregion

    public InputSettings settings;

    public delegate void ActiveControlAction(ActiveControlType previouseState, ActiveControlType newState);
    public event ActiveControlAction ActiveControlChanged;
    
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

    #region Get Methods

    public float GetInput(string input)
    {
        input = input.ToLower();

        switch( input ) {
            case "horizontal":
                return GetInput(settings.horizontal);
            case "vertical":
                return GetInput(settings.vertical);
            case "confirm":
                return GetInput(settings.confirmKey);
            case "cancel":
                return GetInput(settings.cancelKey);
            default:
                Debug.LogError("InputManager was requested unknown input: " + input);
                return 0;
        }
    }
    public float GetInput(KeyCode[] input)
    {
        foreach( KeyCode key in input ) {
            if( Input.GetKey(key) ) { return 1; }
        }

        return 0;
    }
    public float GetInput(Axis[] axes)
    {
        float value = 0;
        foreach( Axis axis in axes ) {
            float axisValue = Input.GetAxisRaw(axis.ToString());
            if( Mathf.Abs(axisValue) >= 1 ) { return axisValue; }
            if( Mathf.Abs(axisValue) > Mathf.Abs(value) ) {
                value = axisValue;
                activeControlType = GetInputType(axis);
            }
        }

        return value;
    }

    public bool IsNoInput()
    {
        return  ( GetInput(settings.horizontal) == 0
        && GetInput(settings.vertical) == 0
        && GetInput(settings.confirmKey) == 0
        && GetInput(settings.cancelKey) == 0
        && GetInput(settings.attackKey) == 0
        && GetInput(settings.blockKey) == 0
        && GetInput(settings.jumpKey) == 0 );
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
    public ActiveControlType GetInputType(Axis axis)
    {
        if( axis < Axis.MouseInputs ) {
            return ActiveControlType.Keyboard;
        } else if( axis < Axis.JoystickInputs ) {
            return ActiveControlType.Mouse;
        } else {
            return ActiveControlType.Controller;
        }
    }

    #endregion

    #region Update Methods

    bool CheckInput( KeyCode[] keys, InputAction action )
    {
        foreach( KeyCode key in keys ) {
            if( Input.GetKeyDown(key) && action != null ) {
                activeControlType = GetInputType(key);
                action(1);
                return true;
            }
            if( Input.GetKeyUp(key) && action != null ) {
                activeControlType = GetInputType(key);
                action(0);
                return true;
            }
        }

        return false;
    }
    bool CheckInput( Axis[] axes, InputAction action, ref float storedValue )
    {
        float value = GetInput(axes);
        if( value != storedValue ) {
            if( action != null ) { action(value); }
            storedValue = value;
            return true;
        }

        return false;
    }

    public void DoUpdate(GameManager.UpdateData data)
    {
        bool anyKeyThisFrame = HandleKeys();
        HandleAxes();

        if( anyKeyThisFrame ) {
            if( AnyInput != null ) { AnyInput(1); }
        }
    }
    bool HandleKeys()
    {
        return 
            CheckInput( settings.confirmKey, ConfirmInput )
            | CheckInput( settings.cancelKey, CancelInput )
            | CheckInput( settings.attackKey, AttackInput )
            | CheckInput( settings.blockKey, BlockInput )
            | CheckInput( settings.jumpKey, JumpInput );
    }
    void HandleAxes()
    {
        CheckInput( settings.horizontal, HorizontalInput, ref storedHorizontalValue );
        CheckInput( settings.vertical, VerticalInput, ref storedVerticalValue );

        CheckInput( settings.pointerHorizontal, PointerHorizontal, ref storedPointerHorizontalValue );
        CheckInput( settings.pointerVertical, PointerVertical, ref storedPointerVerticalValue );

        CheckInput( settings.aimHorizontal, AimHorizontal, ref storedAimHorizontalValue );
        CheckInput( settings.aimVertical, AimVertical, ref storedAimVerticalValue );
    }

    #endregion
}
