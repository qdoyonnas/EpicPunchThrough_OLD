using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : Agent
{
    public override void Init()
    {
        base.Init();

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

    protected bool OnHorizontal( float value )
    {
        if( value == 0 ) {
            if( actions[actions.Count - 1] == Action.MoveForward ) {
                PerformAction(Action.MoveForward, 0);
            } else if( actions[actions.Count - 1] == Action.MoveBack ) {
                PerformAction(Action.MoveBack, 0);
            }
        } else {
            if( value > 0 ) {
                if( isFacingRight ) {
                    PerformAction(Action.MoveForward, value);
                } else {
                    PerformAction(Action.MoveBack, value);
                }
            } else {
                if( isFacingRight ) {
                    PerformAction(Action.MoveBack, value);
                } else {
                    PerformAction(Action.MoveForward, value);
                }
            }
        }

        return true;
    }
    protected bool OnVertical( float value )
    {

        return true;
    }

    protected bool OnAimHorizontal( float value )
    {
        if( Mathf.Abs(value) <= 0.1f ) { return false; }
        value = (value < 0 ? -1: 1);

        aimDirection = new Vector2( value, InputManager.Instance.GetInput(InputManager.Instance.settings.aimVertical) );

        return true;
    }
    protected bool OnAimVertical( float value )
    {
        if( Mathf.Abs(value) <= 0.1f ) { return false; }
        value = (value < 0 ? -1: 1);

        aimDirection = new Vector2( InputManager.Instance.GetInput(InputManager.Instance.settings.aimHorizontal), value );

        return true;
    }

    protected bool OnJump( float value )
    {
        PerformAction( Action.Jump, value );
        return true;
    }

}
