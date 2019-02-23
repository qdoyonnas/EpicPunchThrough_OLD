using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider) )]
public class Agent : MonoBehaviour
{
    #region Animation Fields

    [SerializeField] protected RuntimeAnimatorController baseController;

    protected Animator animator;
    public RuntimeAnimatorController animatorController {
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
        WallSliding,
        OnCeiling,
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
                    xFriction = AgentManager.Instance.settings.groundFriction;
                    yFriction = 0;
                    rigidbody.useGravity = false;
                    rigidbody.velocity = new Vector3( rigidbody.velocity.x, 0, 0 );
                    break;
                case State.InAir:
                    xFriction = AgentManager.Instance.settings.airFriction;
                    yFriction = 0;
                    rigidbody.useGravity = true;
                    break;
                case State.WallSliding:
                    yFriction = AgentManager.Instance.settings.wallFriction;
                    rigidbody.velocity = new Vector3( 0, rigidbody.velocity.y, 0 );
                    break;
                case State.OnCeiling:
                    xFriction = AgentManager.Instance.settings.airFriction;
                    yFriction = 0;
                    break;
                case State.Flinched:

                    break;
                case State.Stunned:

                    break;
                case State.Launched:
                    xFriction = AgentManager.Instance.settings.airFriction;
                    yFriction = 0;
                    rigidbody.useGravity = true;
                    break;
            }
        }
    }

    bool groundFound = false;
    bool wallFound = false;
    bool ceilingFound = false;

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

    #endregion

    #region Physics Fields

    protected float xFriction = 0f;
    protected float yFriction = 0f;

    protected Rigidbody _rigidbody;
    new public Rigidbody rigidbody {
        get {
            return _rigidbody;
        }
    }
    protected CapsuleCollider _collider;
    new public CapsuleCollider collider {
        get {
            return _collider;
        }
    }

    protected TriggerCheck _groundCheck;
    protected TriggerCheck _rightWallCheck;
    protected TriggerCheck _leftWallCheck;
    protected TriggerCheck _ceilingCheck;

    BoxCollider[] boundaryColliders;

    #endregion

    #region Initialization

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
        animatorController = baseController;

        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        _groundCheck = FindTriggerCheck( "GroundCheck", SetGroundFound );
        _leftWallCheck = FindTriggerCheck( "LeftWallCheck", SetWallFound );
        _rightWallCheck = FindTriggerCheck( "RightWallCheck", SetWallFound );
        _ceilingCheck = FindTriggerCheck( "CeilingCheck", SetCeilingFound );

        state = AgentManager.Instance.settings.initialAgentState;

        OnEnvironmentChange( null, EnvironmentManager.Instance.GetEnvironment() );
        EnvironmentManager.Instance.environmentChanged += OnEnvironmentChange;

        AgentManager.Instance.RegisterAgent(this);

        didInit = true;
    }
    protected virtual TriggerCheck FindTriggerCheck( string name, UnityAction<bool> action )
    {
        Transform colliderTransform = transform.Find(name);
        TriggerCheck check = null;
        if( colliderTransform != null ) {
            check = colliderTransform.GetComponent<TriggerCheck>();
        }
        if( check == null ) {
            Debug.LogError("Agent does not have TriggerCheck named " + name);
        } else {
            check.onTrigger.AddListener(action);
        }

        return check;
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
        boundaryColliders = new BoxCollider[colliderCount];
        colliderCount = 0;
        if( nextEnvironment.agentBounds.topBound ) {
            float yPosition = nextEnvironment.agentBounds.maxY + (AgentManager.Instance.settings.verticalBoundarySize.y / 2);
            Vector3 position = new Vector3( transform.position.x, yPosition, transform.position.z);

            CreateBoundary(colliderCount, "_TopBoundary", true, position);

            colliderCount++;
        }

        if( nextEnvironment.agentBounds.rightBound ) {
            float xPosition = nextEnvironment.agentBounds.maxX + (AgentManager.Instance.settings.horizontalBoundarySize.x / 2);
            Vector3 position = new Vector3( xPosition, transform.position.y, transform.position.z);

            CreateBoundary(colliderCount, "_RightBoundary", true, position);

            colliderCount++;
        }

        if( nextEnvironment.agentBounds.bottombound ) {
            float yPosition = nextEnvironment.agentBounds.minY - (AgentManager.Instance.settings.verticalBoundarySize.y / 2);
            Vector3 position = new Vector3( transform.position.x, yPosition, transform.position.z);

            CreateBoundary(colliderCount, "_BottomBoundary", true, position);

            colliderCount++;
        }

        if( nextEnvironment.agentBounds.leftBound ) {
            float xPosition = nextEnvironment.agentBounds.minX - (AgentManager.Instance.settings.horizontalBoundarySize.x / 2);
            Vector3 position = new Vector3( xPosition, transform.position.y, transform.position.z);

            CreateBoundary(colliderCount, "_LeftBoundary", false, position);
        }
    }
    void CreateBoundary(int index, string name, bool isVertical, Vector3 position)
    {
        if( index < 0 || index >= boundaryColliders.Length ) { return; }

        boundaryColliders[index] = new GameObject(gameObject.name + name).AddComponent<BoxCollider>();
        boundaryColliders[index].gameObject.layer = LayerMask.NameToLayer("Boundary");
        boundaryColliders[index].transform.parent = transform;
        boundaryColliders[index].transform.parent = null;

        boundaryColliders[index].size = ( isVertical ? AgentManager.Instance.settings.verticalBoundarySize : AgentManager.Instance.settings.horizontalBoundarySize );

        Follow followScript = boundaryColliders[index].gameObject.AddComponent<Follow>();
        followScript.target = transform;
        followScript.followX = isVertical;
        followScript.followY = !isVertical;
        followScript.onFixedUpdate = true;

        boundaryColliders[index].transform.position = position;
    }

    #endregion

    #region Update

    public virtual void DoUpdate(GameManager.UpdateData data)
    {
        CheckState();

        if( animator.GetBool(transitionBool) ) {
            TransitionTechnique(activeTechnique, false);
        }

        if( activeTechnique != null ) {
            activeTechnique.Update(data);
        } else {
            HandlePhysics();
        }
    }

    public virtual void CheckState()
    {
        switch( state ) {
            case State.InAir:
                if( groundFound ) {
                    state = State.Grounded;
                } else if( wallFound ) {
                    state = State.WallSliding;
                } else if( ceilingFound ) {
                    state = State.OnCeiling;
                }
                break;
            case State.Grounded:
                if( !groundFound ) {
                    state = State.InAir;
                }
                break;
            case State.WallSliding:
                if( groundFound ) {
                    state = State.Grounded;
                } else if( !wallFound ) {
                    state = State.InAir;
                }
                break;
            case State.OnCeiling:
                if( groundFound ) {
                    state = State.Grounded;
                } else if( wallFound ) {
                    state = State.WallSliding;
                } else if( !ceilingFound ) {
                    state = State.InAir;
                }
                break;
        }
    }

    public virtual void HandlePhysics()
    {
        HandleFriction();
    }
    public virtual void HandleFriction()
    {
        if( xFriction > 0 ) {
            rigidbody.velocity = new Vector3( rigidbody.velocity.x * (1 - xFriction), rigidbody.velocity.y, 0 );
            if( rigidbody.velocity.x < AgentManager.Instance.settings.autoStopSpeed ) {
                rigidbody.velocity = new Vector3( 0, rigidbody.velocity.y, 0);
            }
        }

        if( yFriction > 0 ) {
            rigidbody.velocity = new Vector3( rigidbody.velocity.y , rigidbody.velocity.y * (1 - yFriction), 0 );
            if( rigidbody.velocity.y < AgentManager.Instance.settings.autoStopSpeed ) {
                rigidbody.velocity = new Vector3( rigidbody.velocity.x, 0, 0);
            }
        }
    }
    
    #endregion

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
            animatorController = baseController;
            animator.SetBool(transitionBool, false);
        } else {
            animatorController = technique.animatorController;
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

    #region Utility

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

    void SetGroundFound( bool state )
    {
        groundFound = state;
    }
    void SetWallFound( bool state )
    {
        wallFound = state;
    }
    void SetCeilingFound( bool state )
    {
        ceilingFound = state;
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        if( !GameManager.Instance.DebugOn ) { return; }

        DebugExtension.DrawCapsule( new Vector3(transform.position.x, transform.position.y + collider.height / 2, transform.position.z), 
                                    new Vector3(transform.position.x, transform.position.y - collider.height / 2, transform.position.z),
                                    Color.green,
                                    collider.radius );

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere( _groundCheck.transform.position, ((SphereCollider)_groundCheck.collider).radius );
        Gizmos.DrawWireCube( _rightWallCheck.transform.position, ((BoxCollider)_rightWallCheck.collider).size );
        Gizmos.DrawWireCube( _leftWallCheck.transform.position, ((BoxCollider)_leftWallCheck.collider).size );
        Gizmos.DrawWireSphere( _ceilingCheck.transform.position, ((SphereCollider)_ceilingCheck.collider).radius );

        if( boundaryColliders != null ) {
            for( int i = 0; i < boundaryColliders.Length; i++ ) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(boundaryColliders[i].transform.position, boundaryColliders[i].size);
            }
        }
    }

    #endregion
}
