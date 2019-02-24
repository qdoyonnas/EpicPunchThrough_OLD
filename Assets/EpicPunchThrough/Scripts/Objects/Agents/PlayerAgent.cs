using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : Agent
{
    public override void Init()
    {
        base.Init();

        AgentManager.Instance.playerAgent = this;

        HandleSubscriptions(true);
    }

    protected void HandleSubscriptions(bool state)
    {
        if( state ) {
            InputManager.Instance.LeftInput += OnLeftInput;
            InputManager.Instance.RightInput += OnRightInput;
            InputManager.Instance.JumpInput += OnJump;
        } else {
            InputManager.Instance.LeftInput -= OnLeftInput;
            InputManager.Instance.RightInput -= OnRightInput;
            InputManager.Instance.JumpInput -= OnJump;
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
}
