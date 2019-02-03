using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Actor : MonoBehaviour
{
    [SerializeField] protected RuntimeAnimatorController baseController;

    protected Animator animator;
    public RuntimeAnimatorController AnimatorController {
        get {
            return animator.runtimeAnimatorController;
        }
        set {
            animator.runtimeAnimatorController = value;
        }
    }
    protected bool stopAnimationControllerTransition = false;

    private int _team = 0;
    public int Team {
        get {
            return _team;
        }
        set {
            _team = value;
        }
    }

    private bool didInit = false;
    private void Start()
    {
        if( !didInit ) {
            Init();
        }
    }

    public virtual void Init( RuntimeAnimatorController baseController, int team )
    {
        this.baseController = baseController;
        this._team = team;

        Init();
    }
    public virtual void Init()
    {
        animator = GetComponent<Animator>();
        AnimatorController = baseController;

        ActorManager.Instance.RegisterActor(this);

        didInit = true;
    }

    public virtual void DoUpdate(GameManager.UpdateData data)
    {
    }
}
