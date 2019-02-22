using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Animator), typeof(Rigidbody) )]
public class Agent : MonoBehaviour
{
    #region Animation Fields

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
    protected bool stopAnimatorControllerTransition = false;

    protected string transitionStateName = "ControllerTransition";
    protected string transitionBool = "transitionController";

    #endregion

    #region Technique Fields

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

    #endregion

    #region Agent State Fields

    protected int _team = 0;
    public int Team {
        get {
            return _team;
        }
        set {
            _team = value;
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
        Any, // For technique triggers only - Agents should never be in this state
        Grounded,
        InAir,
        Flinched,
        Stunned,
        Launched
    }
    protected State _state;
    public State state {
        get {
            return _state;
        }
        set {
            _state = value;
            switch( value ) {
                case State.Grounded:
                    friction = AgentManager.Instance.settings.groundFriction;
                    break;
                case State.InAir:
                    friction = AgentManager.Instance.settings.airFriction;
                    break;
            }
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
    [SerializeField] protected List<Action> actions = new List<Action>();

    #endregion

    #region Physics Fields

    public float friction = 0f;

    protected Rigidbody _rigidbody;
    new public Rigidbody rigidbody {
        get {
            return _rigidbody;
        }
    }

    GameObject[] boundaryColliders;

    #endregion

    protected bool didInit = false;
    private void Start()
    {
        if( !didInit ) {
            Init();
        }
    }

    private void OnDestroy()
    {
        if( boundaryColliders != null ) {
            for( int i = boundaryColliders.Length - 1; i >= 0; i-- ) {
                Destroy(boundaryColliders[i]);
            }
            boundaryColliders = null;
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
        state = State.InAir;

        OnEnvironmentChange( null, EnvironmentManager.Instance.GetEnvironment() );
        EnvironmentManager.Instance.environmentChanged += OnEnvironmentChange;

        AgentManager.Instance.RegisterAgent(this);

        didInit = true;
    }

    public virtual void DoUpdate(GameManager.UpdateData data)
    {
        if( animator.GetBool(transitionBool) ) {
            TransitionTechnique(activeTechnique, false);
        }

        if( activeTechnique != null ) {
            activeTechnique.Update(data);
        } else {
            HandlePhysics();
        }
    }

    public virtual void OnEnvironmentChange( Environment previousEnvironment, Environment nextEnvironment )
    {
        if( boundaryColliders != null ) {
            for( int i = boundaryColliders.Length - 1; i >= 0; i-- ) {
                Destroy(boundaryColliders[i]);
            }
        }

        int colliderCount = ( nextEnvironment.agentBounds.topBound ? 1 : 0 )
                            + ( nextEnvironment.agentBounds.rightBound ? 1 : 0 )
                            + ( nextEnvironment.agentBounds.bottombound ? 1 : 0 )
                            + ( nextEnvironment.agentBounds.leftBound ? 1 : 0 );
        boundaryColliders = new GameObject[colliderCount];
        if( nextEnvironment.agentBounds.topBound ) {
            boundaryColliders[0] = new GameObject(gameObject.name + "_TopBoundary");
            boundaryColliders[0].transform.parent = transform;
            boundaryColliders[0].transform.parent = null;

            BoxCollider collider = boundaryColliders[0].AddComponent<BoxCollider>();
            collider.size = new Vector3(1, 5, 1);

            Follow followScript = boundaryColliders[0].AddComponent<Follow>();
            followScript.followX = true;
            followScript.onFixedUpdate = true;

            float yPosition = nextEnvironment.agentBounds.maxY + (collider.size.y / 2);
            boundaryColliders[0].transform.position = new Vector3( transform.position.x, yPosition, transform.position.z);
        }

        if( nextEnvironment.agentBounds.rightBound ) {
            boundaryColliders[0] = new GameObject(gameObject.name + "_RightBoundary");
            boundaryColliders[0].transform.parent = transform;
            boundaryColliders[0].transform.parent = null;

            BoxCollider collider = boundaryColliders[0].AddComponent<BoxCollider>();
            collider.size = new Vector3(5, 1, 1);

            Follow followScript = boundaryColliders[0].AddComponent<Follow>();
            followScript.followY = true;
            followScript.onFixedUpdate = true;

            float xPosition = nextEnvironment.agentBounds.maxX + (collider.size.x / 2);
            boundaryColliders[0].transform.position = new Vector3( xPosition, transform.position.y, transform.position.z);
        }

        if( nextEnvironment.agentBounds.bottombound ) {
            boundaryColliders[0] = new GameObject(gameObject.name + "_BottomBound");
            boundaryColliders[0].transform.parent = transform;
            boundaryColliders[0].transform.parent = null;

            BoxCollider collider = boundaryColliders[0].AddComponent<BoxCollider>();
            collider.size = new Vector3(1, 5, 1);

            Follow followScript = boundaryColliders[0].AddComponent<Follow>();
            followScript.followX = true;
            followScript.onFixedUpdate = true;

            float yPosition = nextEnvironment.agentBounds.minY - (collider.size.y / 2);
            boundaryColliders[0].transform.position = new Vector3( transform.position.x, yPosition, transform.position.z);
        }

        if( nextEnvironment.agentBounds.leftBound ) {
            boundaryColliders[0] = new GameObject(gameObject.name + "_LeftBoundary");
            boundaryColliders[0].transform.parent = transform;
            boundaryColliders[0].transform.parent = null;

            BoxCollider collider = boundaryColliders[0].AddComponent<BoxCollider>();
            collider.size = new Vector3(5, 1, 1);

            Follow followScript = boundaryColliders[0].AddComponent<Follow>();
            followScript.followY = true;
            followScript.onFixedUpdate = true;

            float xPosition = nextEnvironment.agentBounds.minX - (collider.size.x / 2);
            boundaryColliders[0].transform.position = new Vector3( xPosition, transform.position.y, transform.position.z);
        }
    }

    public virtual void HandlePhysics()
    {
        if( friction > 0 ) {
            rigidbody.velocity = rigidbody.velocity * (1 - friction);
            if( rigidbody.velocity.magnitude < AgentManager.Instance.settings.autoStopSpeed ) {
                rigidbody.velocity = Vector3.zero;
            }
        }
    }

    #region Technique Methods

    public virtual void AddTechnique( Technique technique )
    {
        techniques.Add(technique);
    }
    public virtual void RemoveTechnique( Technique technique )
    {
        techniques.Remove(technique);
    }

    public virtual void  TransitionTechnique( Technique technique = null, bool blend = true )
    {
        activeTechnique = technique;
        if( blend && !animator.GetCurrentAnimatorStateInfo(0).IsName(transitionStateName) ) {
            animator.SetBool(transitionBool, true);
            return;
        }

        if( technique == null ) {
            AnimatorController = baseController;
            animator.SetBool(transitionBool, false);
        } else {
            AnimatorController = technique.animatorController;
            animator.SetBool(transitionBool, false);
        }
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
            if( actions.Count > AgentManager.Instance.settings.actionSequenceLength ) {
                actions.RemoveAt(0);
            }
        }
    }

    #endregion

    public virtual void SetName( string name )
    {
        gameObject.name = name;
        if( boundaryColliders != null ) {
            for( int i = 0; i < boundaryColliders.Length; i++ ) {
                string boundaryName = boundaryColliders[i].name;
                int underScoreIndex = boundaryName.LastIndexOf("_");
                boundaryColliders[i].name = gameObject.name + boundaryName.Substring( underScoreIndex + 1, boundaryName.Length - underScoreIndex - 1 );
            }
        }
    }
}
