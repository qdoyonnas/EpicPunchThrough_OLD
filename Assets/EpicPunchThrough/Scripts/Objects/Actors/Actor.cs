using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Animator), typeof(Rigidbody) )]
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

    protected Rigidbody _rigidbody;
    new public Rigidbody rigidbody {
        get {
            return _rigidbody;
        }
    }

    protected int _team = 0;
    public int Team {
        get {
            return _team;
        }
        set {
            _team = value;
        }
    }

    protected List<Technique> techniques = new List<Technique>();
    protected Technique _activeTechnique;
    public Technique activeTechnique {
        get {
            return _activeTechnique;
        }
        set {
            _activeTechnique = value;
        }
    }

    private bool _isFacingRight = false;
    public bool isFacingRight {
        get {
            return _isFacingRight;
        }
        set {
            _isFacingRight = value;
            transform.localScale = new Vector3( isFacingRight ? -1 : 1, 1, 1 );
        }
    }

    public enum State {
        Any, // For technique triggers only - Actors should never be in this state
        Grounded,
        InAir,
        Flinched,
        Stunned,
        Launched
    }
    protected State _state = State.Grounded;
    public State state {
        get {
            return _state;
        }
    }

    public enum Action {
        MoveForward,
        MoveBack,
        AttackForward,
        AttackDown,
        AttackBack,
        AttackUp,
        Block,
        Jump,
        Dash,
        Clash
    }
    protected List<Action> actions = new List<Action>();

    protected bool didInit = false;
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

        _rigidbody = GetComponent<Rigidbody>();

        ActorManager.Instance.RegisterActor(this);

        didInit = true;
    }

    public virtual void DoUpdate(GameManager.UpdateData data)
    {
        if( activeTechnique != null ) {
            activeTechnique.Update(data);
        }
    }

    #region Technique Methods

    public virtual void AddTechnique(Technique technique)
    {
        techniques.Add(technique);
    }
    public virtual void RemoveTechnique(Technique technique)
    {
        techniques.Remove(technique);
    }

    #endregion

    #region Action Methods

    public delegate void ActionEvent();
    public event ActionEvent MoveForward;
    public event ActionEvent MoveBack;
    public event ActionEvent Jump;

    public void SubscribeToActionEvent(Action action, ActionEvent callback, bool unSubscribe = false)
    {
        if( !unSubscribe ) {
            switch( action ) {
                case Action.MoveForward:
                    MoveForward += callback;
                    break;
                case Action.MoveBack:
                    MoveBack += callback;
                    break;
                case Action.Jump:
                    Jump += callback;
                    break;
                default:
                    Debug.LogError("Action not implemented");
                    break;
            }
        } else {
            switch( action ) {
                case Action.MoveForward:
                    MoveForward -= callback;
                    break;
                case Action.MoveBack:
                    MoveBack -= callback;
                    break;
                case Action.Jump:
                    Jump -= callback;
                    break;
                default:
                    Debug.LogError("Action not implemented");
                    break;
            }
        }
    }

    public void PerformAction( Action action, bool state )
    {
        if( activeTechnique != null
            && !activeTechnique.ValidateAction(action, state) )
        { return; }
        if( !state ) { return; }

        ActionEvent handler;
        switch( action ) {
            case Action.MoveForward:
                handler = MoveForward;
                break;
            case Action.MoveBack:
                handler = MoveBack;
                break;
            case Action.Jump:
                handler = Jump;
                break;
            default:
                Debug.LogError("Action not implemented: " + action);
                return;
        }
        if( handler != null ) {
            handler();
        }

        if( state ) {
            actions.Add(action);
            if( actions.Count > ActorManager.Instance.settings.actionSequenceLength ) {
                actions.RemoveAt(0);
            }
        }
    }

    #endregion
}
