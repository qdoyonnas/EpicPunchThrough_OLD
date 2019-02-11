using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : Actor
{
    public override void Init()
    {
        base.Init();

        ActorManager.Instance.playerActor = this;

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

    // AnimatorController Switching Test
    /* 
    public float chargeRate = 1f;
    public float maxCharge = 2f;
    public float minAttackSpeed = 5f;

    [SerializeField] protected RuntimeAnimatorController attackController;

    protected bool alternateAttack = false;
    protected bool isCharging = false;
    protected float chargeValue = 0.1f;

    protected override void Init()
    {
        base.Init();

        InputManager.Instance.BlockInput += BattleStance;
        InputManager.Instance.AttackInput += BasicAttack;

        ActorManager.Instance.playerActor = this;
    }

    public override void DoUpdate( GameManager.UpdateData data )
    {
        base.DoUpdate(data);

        if( isCharging && chargeValue < maxCharge) {
            chargeValue += chargeRate * data.deltaTime;
        }
    }

    bool BattleStance(bool isDown)
    {
        if( AnimatorController != idleController ) {
            if( animator.GetCurrentAnimatorStateInfo(0).IsName(controllerTransistionStateName) ) {
                AnimatorController = idleController;
                animator.SetBool("InFight", true);
            } else {
                return false;
            }
        }

        if( isDown ) {
            animator.SetBool("InFight", !animator.GetBool("InFight"));
        }

        return true;
    }
    bool BasicAttack(bool isDown)
    {
        if( AnimatorController == idleController && !animator.GetBool("InFight") ) { return false; }

        if( AnimatorController != attackController ) {
            AnimatorController = attackController;
        }


        if( isDown ) {
            if( !animator.GetCurrentAnimatorStateInfo(0).IsName(controllerTransistionStateName) ) { return false; }

            if( alternateAttack ) {
                animator.SetTrigger("Charge2");
            } else {
                animator.SetTrigger("Charge1");
            }

            isCharging = true;
        } else {
            if( !isCharging ) { return false; }

            if( alternateAttack ) {
                animator.SetTrigger("Attack2");
            } else {
                animator.SetTrigger("Attack1");
            }
            float attackSpeed = maxCharge / chargeValue;
            attackSpeed = attackSpeed > minAttackSpeed ? minAttackSpeed : attackSpeed;
            animator.SetFloat("AttackSpeed", attackSpeed);

            alternateAttack = !alternateAttack;
            isCharging = false;
            chargeValue = 0.1f;
        }

        return true;
    }
    */
}
