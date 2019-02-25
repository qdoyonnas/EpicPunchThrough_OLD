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
            InputManager.Instance.LeftInput += OnLeftInput;
            InputManager.Instance.RightInput += OnRightInput;
            InputManager.Instance.JumpInput += OnJump;
            InputManager.Instance.MouseMovement += OnDirectionInput;
        } else {
            InputManager.Instance.LeftInput -= OnLeftInput;
            InputManager.Instance.RightInput -= OnRightInput;
            InputManager.Instance.JumpInput -= OnJump;
            InputManager.Instance.MouseMovement -= OnDirectionInput;
        }
    }

    protected bool OnLeftInput(bool isDown)
    {
        if( !isFacingRight ) {
            PerformAction(Action.MoveForward, isDown);
        } else {
            PerformAction(Action.MoveBack, isDown);
        }
        return true;
    }
    protected bool OnRightInput(bool isDown)
    {
        if( isFacingRight ) {
            PerformAction(Action.MoveForward, isDown);
        } else {
            PerformAction(Action.MoveBack, isDown);
        }
        return true;
    }
    protected bool OnJump(bool isDown)
    {
        PerformAction(Action.Jump, isDown);
        return true;
    }
    protected bool OnDirectionInput( Vector2 pos, Vector2 delta )
    {
        if( delta == Vector2.zero ) { return false; }

        Vector2 normalizedDelta = delta.normalized;

        aimDirection = -normalizedDelta;

        return true;
    }
}
