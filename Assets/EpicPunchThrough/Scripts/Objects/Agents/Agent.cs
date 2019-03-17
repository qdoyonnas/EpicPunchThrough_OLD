using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(Animator), typeof(PhysicsBody), typeof(CapsuleCollider) )]
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

    protected Transform graphicsChild;
    protected DirectionIndicator directionIndicator;

    #endregion

    #region Technique Fields

    protected List<Technique> techniques = new List<Technique>();
    protected List<Technique> activatingTechniques = new List<Technique>();

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
            graphicsChild.localScale = new Vector3( isFacingRight ? -1 : 1, 1, 1 );
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
            switch( value ) {
                case State.Grounded:
                    physicsBody.useGravity = false;
                    physicsBody.velocity = new Vector3(physicsBody.velocity.x, 0, 0);
                    physicsBody.frictionCoefficients.x = EnvironmentManager.Instance.GetEnvironment().groundFriction;
                    PerformAction(Action.Land, 1);
                    PerformAction(Action.Land, 0);
                    break;
                case State.InAir:
                    physicsBody.useGravity = true;
                    physicsBody.frictionCoefficients.x = EnvironmentManager.Instance.GetEnvironment().airFriction;
                    break;
                case State.WallSliding:
                    physicsBody.useGravity = true;
                    physicsBody.velocity = new Vector3(0, physicsBody.velocity.y, 0);
                    PerformAction(Action.Land, 1);
                    PerformAction(Action.Land, 0);
                    break;
                case State.OnCeiling:
                    physicsBody.useGravity = true;
                    physicsBody.velocity = new Vector3(physicsBody.velocity.x, 0, 0);
                    PerformAction(Action.Land, 1);
                    PerformAction(Action.Land, 0);
                    break;
                case State.Flinched:
                    break;
                case State.Stunned:
                    break;
                case State.Launched:
                    physicsBody.useGravity = true;
                    physicsBody.frictionCoefficients.x = EnvironmentManager.Instance.GetEnvironment().airFriction;
                    break;
            }

            _state = value;
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
        Clash,
        Land
    }
    protected List<Action> actions = new List<Action>();
    public Action[] ActionSequence {
         get {
            return actions.ToArray();
        }
    }

    private float _activeActionValue = 0;
    public float activeActionValue {
        get {
            return _activeActionValue;
        }
    }

    protected Vector2 _aimDirection = Vector2.up;
    public Vector2 aimDirection {
        get {
            return _aimDirection;
        }
        set {
            if( value.magnitude <= 0 ) {
                _aimDirection = Vector2.up;
            }
            _aimDirection = value.normalized;

            directionIndicator.transform.eulerAngles = new Vector3( 0, 0, Vector2.SignedAngle(Vector2.up, _aimDirection) );
        }
    }

    #endregion

    #region Physics Fields

    protected PhysicsBody _physicsBody;
    public PhysicsBody physicsBody {
        get {
            return _physicsBody;
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

    List<Collider> ignoredColliders = new List<Collider>();

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

        _physicsBody = GetComponent<PhysicsBody>();
        _collider = GetComponent<CapsuleCollider>();

        _groundCheck = FindTriggerCheck( "GroundCheck", SetGroundFound );
        _leftWallCheck = FindTriggerCheck( "LeftWallCheck", SetWallFound );
        _rightWallCheck = FindTriggerCheck( "RightWallCheck", SetWallFound );
        _ceilingCheck = FindTriggerCheck( "CeilingCheck", SetCeilingFound );

        graphicsChild = transform.Find("Graphics");
        directionIndicator = GetComponentInChildren<DirectionIndicator>();
        directionIndicator.gameObject.SetActive(false);

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
        EnableTriggerChecks();
        CheckState();

        HandleTechniques();
        if( ValidActiveTechnique() ) {
            activeTechnique.Update(data);
        }
         
        physicsBody.HandleFriction();

        HandlePropCollisions( data.deltaTime );
    }

    protected virtual void EnableTriggerChecks()
    {
        if( state == State.WallSliding ) {
            _leftWallCheck.doDetect= true;
            _rightWallCheck.doDetect = true;
        } else if( physicsBody.velocity.x > 0 ) {
            _rightWallCheck.doDetect = true;
            _leftWallCheck.doDetect= false;
        } else if( physicsBody.velocity.x < 0 ) {
            _leftWallCheck.doDetect = true;
            _rightWallCheck.doDetect = false;
        } else {
            _leftWallCheck.doDetect= false;
            _rightWallCheck.doDetect = false;
            wallFound = false;
        }
        if( state == State.OnCeiling || physicsBody.velocity.y > 0 ) {
            _ceilingCheck.doDetect = true;
        } else {
            _ceilingCheck.doDetect = false;
            ceilingFound = false;
        }
        if ( state == State.Grounded || physicsBody.velocity.y < 0 ) {
            _groundCheck.doDetect = true;
        } else {
            _groundCheck.doDetect = false;
            groundFound = false;
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

    protected virtual void HandleTechniques()
    {
        if( !ValidActiveTechnique() && _activeActionValue != 0 ) {
            PerformAction(actions[actions.Count-1], _activeActionValue);
        }

        if( animator.GetBool(transitionBool) ) {
            TransitionTechnique(activeTechnique, false);
        }

        ActivateTechniques();
    }
    protected virtual void ActivateTechniques()
    {
        if( activatingTechniques.Count == 1 ) {
            TransitionTechnique(activatingTechniques[0], false);
            activatingTechniques.Clear();
        } else if( activatingTechniques.Count > 1 ) {
            Technique chosenTech = null;
            int longestSequence = 0;
            foreach( Technique tech in activatingTechniques ) {
                if( tech.techTrigger.sequence.Length > longestSequence ) {
                    chosenTech = tech;
                    longestSequence = tech.techTrigger.sequence.Length;
                } else if( tech.techTrigger.sequence.Length == longestSequence ) {
                    if( chosenTech.techTrigger.state == State.Any && tech.techTrigger.state != State.Any ) {
                        chosenTech = tech;
                        longestSequence = tech.techTrigger.sequence.Length;
                    }
                }
            }

            TransitionTechnique( chosenTech, false );
            activatingTechniques.Clear();
        }
    }

    protected virtual void HandlePropCollisions( float deltaTime )
    {
        for( int i = ignoredColliders.Count - 1; i >= 0; i-- ) {
            if( !ignoredColliders[i].bounds.Intersects(collider.bounds) ) {
                IgnoreCollider(ignoredColliders[i], false);
            }
        }

        if( state == State.Grounded ) {
            RaycastHit[] hits;
            if( physicsBody.DetectCollisions( deltaTime, out hits ) ) {
                foreach( RaycastHit hit in hits ) {
                    Prop prop = hit.collider.GetComponent<Prop>();
                    if( prop != null && prop.isPassable ) {
                        IgnoreCollider(hit.collider);
                    }
                }
            }
        }
    }

    public virtual void IgnoreCollider(Collider other, bool doIgnore = true)
    {
        if( doIgnore ) {
            if( ignoredColliders.Contains(other) ) { return; }

            Physics.IgnoreCollision(collider, other);
            Physics.IgnoreCollision(_groundCheck.collider, other);
            Physics.IgnoreCollision(_ceilingCheck.collider, other);
            Physics.IgnoreCollision(_leftWallCheck.collider, other);
            Physics.IgnoreCollision(_rightWallCheck.collider, other);

            ignoredColliders.Add(other);
        } else {
            if( !ignoredColliders.Contains(other) ) { return; }

            Physics.IgnoreCollision(collider, other, false);
            Physics.IgnoreCollision(_groundCheck.collider, other, false);
            Physics.IgnoreCollision(_ceilingCheck.collider, other, false);
            Physics.IgnoreCollision(_leftWallCheck.collider, other, false);
            Physics.IgnoreCollision(_rightWallCheck.collider, other, false);

            ignoredColliders.Remove(other);
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

    public virtual void AddActivatingTechnique( Technique technique )
    {
        activatingTechniques.Add( technique );
    }

    public virtual void TransitionTechnique( Technique technique = null, bool blend = true )
    {
        if( activeTechnique != technique ) {
            if( ValidActiveTechnique() ) {
                activeTechnique.Exit();
            }
            activeTechnique = technique;
            if( technique != null && technique.IsValid() ) {
                technique.Activate();
            }
        }

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

    public virtual bool ValidActiveTechnique()
    {
        return ( activeTechnique != null && activeTechnique.IsValid() );
    }

    #endregion

    #region Action Methods

    public delegate void ActionEvent();
    public event ActionEvent MoveForward;
    public event ActionEvent MoveBack;
    public event ActionEvent AttackForward;
    public event ActionEvent AttackDown;
    public event ActionEvent AttackBack;
    public event ActionEvent AttackUp;
    public event ActionEvent Block;
    public event ActionEvent Jump;
    public event ActionEvent Dash;
    public event ActionEvent Clash;
    public event ActionEvent Land;

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
                case Action.AttackForward:
                    AttackForward += callback;
                    break;
                case Action.AttackDown:
                    AttackDown += callback;
                    break;
                case Action.AttackBack:
                    AttackBack += callback;
                    break;
                case Action.AttackUp:
                    AttackUp += callback;
                    break;
                case Action.Block:
                    Block += callback;
                    break;
                case Action.Jump:
                    Jump += callback;
                    break;
                case Action.Dash:
                    Dash += callback;
                    break;
                case Action.Clash:
                    Clash += callback;
                    break;
                case Action.Land:
                    Land += callback;
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
                case Action.AttackForward:
                    AttackForward -= callback;
                    break;
                case Action.AttackDown:
                    AttackDown -= callback;
                    break;
                case Action.AttackBack:
                    AttackBack -= callback;
                    break;
                case Action.AttackUp:
                    AttackUp -= callback;
                    break;
                case Action.Block:
                    Block -= callback;
                    break;
                case Action.Jump:
                    Jump -= callback;
                    break;
                case Action.Dash:
                    Dash -= callback;
                    break;
                case Action.Clash:
                    Clash -= callback;
                    break;
                case Action.Land:
                    Land -= callback;
                    break;
                default:
                    Debug.LogError("Action not implemented");
                    break;
            }
        }
    }

    public void PerformAction( Action action, float value )
    {
        if( ValidActiveTechnique()
            && !activeTechnique.ValidateAction(action, value) )
        { return; }

        if( value == 0 ) { _activeActionValue = 0; return; }

        ActionEvent handler;
        switch( action ) {
            case Action.MoveForward:
                handler = MoveForward;
                break;
            case Action.MoveBack:
                handler = MoveBack;
                break;
            case Action.AttackForward:
                handler = AttackForward;
                break;
            case Action.AttackDown:
                handler = AttackDown;
                break;
            case Action.AttackBack:
                handler = AttackBack;
                break;
            case Action.AttackUp:
                handler = AttackUp;
                break;
            case Action.Block:
                handler = Block;
                break;
            case Action.Jump:
                handler = Jump;
                break;
            case Action.Dash:
                handler = Dash;
                break;
            case Action.Clash:
                handler = Clash;
                break;
            case Action.Land:
                handler = Land;
                break;
            default:
                Debug.LogError("Action not implemented: " + action);
                return;
        }

        if( actions.Count == 0 || actions[actions.Count - 1] != action ) {
            actions.Add(action);

            if( actions.Count > AgentManager.Instance.settings.actionSequenceLength ) {
                actions.RemoveAt(0);
            }
        }
        _activeActionValue = value;

        if( handler != null ) {
            handler();
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
        if( _groundCheck.enabled ) {  Gizmos.DrawWireSphere( _groundCheck.transform.position, ((SphereCollider)_groundCheck.collider).radius ); }
        if( _rightWallCheck.enabled ) {  Gizmos.DrawWireCube( _rightWallCheck.transform.position, ((BoxCollider)_rightWallCheck.collider).size ); }
        if( _leftWallCheck.enabled ) {  Gizmos.DrawWireCube( _leftWallCheck.transform.position, ((BoxCollider)_leftWallCheck.collider).size ); }
        if( _ceilingCheck.enabled ) {  Gizmos.DrawWireSphere( _ceilingCheck.transform.position, ((SphereCollider)_ceilingCheck.collider).radius ); }

        if( boundaryColliders != null ) {
            for( int i = 0; i < boundaryColliders.Length; i++ ) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(boundaryColliders[i].transform.position, boundaryColliders[i].size);
            }
        }
    }

    #endregion
}
