using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Actor : MonoBehaviour
{
    [SerializeField] protected RuntimeAnimatorController idleController;
    [SerializeField] protected string controllerTransistionStateName = "BattleIdle1";

    protected Animator animator;
    public RuntimeAnimatorController AnimatorController {
        get {
            return animator.runtimeAnimatorController;
        }
        set {
            animator.runtimeAnimatorController = value;
        }
    }
    protected bool stopControllerTransition = false;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        animator = GetComponent<Animator>();

        ActorManager.Instance.RegisterActor(this);
    }

    public virtual void DoUpdate(GameManager.UpdateData data)
    {
    }
}
