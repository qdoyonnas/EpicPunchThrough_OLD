using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAgent : Agent
{
    public float mouseAimSmoothing = 15f;
  
    public enum Control {
        Horizontal,
        Vertical,
        Jump
    }
    protected Dictionary<Control, float> controlState = new Dictionary<Control, float>();
    protected List<Control> controlQueue = new List<Control>();

    public override void Init()
    {
        base.Init();

        controlState.Add(Control.Horizontal, 0);
        controlState.Add(Control.Vertical, 0);
        controlState.Add(Control.Jump, 0);

        directionIndicator.gameObject.SetActive(true);

        AgentManager.Instance.playerAgent = this;

        HandleSubscriptions(true);
    }

    protected void HandleSubscriptions(bool state)
    {
        if( state ) {
            InputManager.Instance.HorizontalInput += OnHorizontal;
            InputManager.Instance.VerticalInput += OnVertical;
            InputManager.Instance.JumpInput += OnJump;

            InputManager.Instance.AimHorizontal += OnAimHorizontal;
            InputManager.Instance.AimVertical += OnAimVertical;
        } else {
            InputManager.Instance.HorizontalInput -= OnHorizontal;
            InputManager.Instance.VerticalInput -= OnVertical;
            InputManager.Instance.JumpInput -= OnJump;

            InputManager.Instance.AimHorizontal -= OnAimHorizontal;
            InputManager.Instance.AimVertical -= OnAimVertical;
        }
    }

    #region Controls Methods

    protected bool OnAimHorizontal( float value )
    {
        aimDirection = new Vector2(aimDirection.x + (value / mouseAimSmoothing), aimDirection.y);

        return true;
    }
    protected bool OnAimVertical( float value )
    {
        aimDirection = new Vector2(aimDirection.x, aimDirection.y + (value / mouseAimSmoothing));

        return true;
    }

    protected bool OnHorizontal( float value )
    {
        UpdateControl(Control.Horizontal, value);

        return true;
    }
    protected bool OnVertical( float value )
    {
        UpdateControl(Control.Vertical, value);

        return true;
    }

    protected bool OnJump( float value )
    {
        UpdateControl(Control.Jump, value);

        return true;
    }

    protected void UpdateControl( Control control, float value )
    {
        controlState[control] = value;

        if( controlQueue.Count > 0 && (value == 0 || controlQueue[0] != control) ) {
            for( int i = 0; i < controlQueue.Count; i++ ) {
                if( controlQueue[i] == control ) {
                    controlQueue.RemoveAt(i);
                }
            }
            if( value == 0 ) { return; }
        }

        controlQueue.Insert(0, control);
    }

    #endregion

    public override void DoUpdate( GameManager.UpdateData data )
    {
        if( controlQueue.Count > 0 ) {
            SubmitAction();
        }

        if( activeActionValue != 0 ) {
            Control control = Control.Horizontal;
            switch( lastAction ) {
                case Action.MoveForward:
                    control = Control.Horizontal;
                    break;
                case Action.MoveBack:
                    control = Control.Horizontal;
                    break;
                case Action.MoveUp:
                    control = Control.Vertical;
                    break;
                case Action.MoveDown:
                    control = Control.Vertical;
                    break;
                case Action.Jump:
                    control = Control.Jump;
                    break;
            }
            if( controlState[control] == 0 ) {
                PerformAction(lastAction, 0);
            }
        }

        base.DoUpdate(data);
    }

    protected void SubmitAction()
    {
        switch( controlQueue[0] ) {
            case Control.Horizontal:
                SubmitHorizontalAction();
                break;
            case Control.Vertical:
                SubmitVerticalAction();
                break;
            case Control.Jump:
                SubmitAction(Action.Jump, controlState[Control.Jump]);
                break;
        }
    }
    protected void SubmitAction( Action action, float value )
    {
        if( lastAction != action || activeActionValue != value ) {
            PerformAction(action, value);
        }
    }
    protected void SubmitHorizontalAction()
    {
        if( controlState[Control.Horizontal] > 0 ) {
            if( isFacingRight ) {
                SubmitAction(Action.MoveForward, controlState[Control.Horizontal]);
            } else {
                SubmitAction(Action.MoveBack, controlState[Control.Horizontal]);
            }
        } else {
            if( isFacingRight ) {
                SubmitAction(Action.MoveBack, -controlState[Control.Horizontal]);
            } else {
                SubmitAction(Action.MoveForward, -controlState[Control.Horizontal]);
            }
        }
    }
    protected void SubmitVerticalAction()
    {
        if( controlState[Control.Vertical] > 0 ) {
            SubmitAction(Action.MoveUp, controlState[Control.Vertical]);
        } else {
            SubmitAction(Action.MoveDown, -controlState[Control.Vertical]);
        }
    }
}
