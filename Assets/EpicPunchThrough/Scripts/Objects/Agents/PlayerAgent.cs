using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : Agent
{
    public float mouseAimSmoothing = 15f;

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
        if( value == 0 ) {
            if( actions[actions.Count - 1] == Action.MoveUp ) {
                PerformAction(Action.MoveUp, 0);
            } else if( actions[actions.Count - 1] == Action.MoveDown ) {
                PerformAction(Action.MoveDown, 0);
            }
        }else {
            if( value > 0 ) {
                PerformAction( Action.MoveUp, value );
            } else {
                PerformAction( Action.MoveDown, value );
            }
        }

        return true;
    }

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

    protected bool OnJump( float value )
    {
        PerformAction( Action.Jump, value );
        return true;
    }

}
