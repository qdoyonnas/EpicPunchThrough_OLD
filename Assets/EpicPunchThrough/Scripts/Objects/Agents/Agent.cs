using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(Animator), typeof(PhysicsBody), typeof(CapsuleCollider) )]
public class Agent : MonoBehaviour
{
    #region Animation Fields

    [SerializeField] protected RuntimeAnimatorController baseController;

    protected Animator _animator;
    public Animator animator {
         get {
            return _animator;
        }
    }
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

    protected ParticleController particleController;

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
            if( ValidActiveTechnique() ) {
                _activeTechnique.OnStateChange( _state, value );
            }

            switch( value ) {
                case State.Grounded:
                    physicsBody.useGravity = false;
                    physicsBody.velocity = new Vector3(physicsBody.velocity.x, 0, 0);
                    physicsBody.frictionCoefficients = EnvironmentManager.Instance.GetEnvironment().groundFriction;
                    break;
                case State.InAir:
                    physicsBody.useGravity = true;
                    physicsBody.frictionCoefficients = EnvironmentManager.Instance.GetEnvironment().airFriction;
                    break;
                case State.WallSliding:
                    physicsBody.useGravity = true;
                    physicsBody.velocity = new Vector3(0, physicsBody.velocity.y, 0);
                    physicsBody.frictionCoefficients = EnvironmentManager.Instance.GetEnvironment().wallFriction;
                    break;
                case State.OnCeiling:
                    physicsBody.useGravity = true;
                    physicsBody.velocity = new Vector3(physicsBody.velocity.x, 0, 0);
                    break;
                case State.Flinched:
                    break;
                case State.Stunned:
                    break;
                case State.Launched:
                    physicsBody.useGravity = true;
                    physicsBody.frictionCoefficients = EnvironmentManager.Instance.GetEnvironment().airFriction;
                    break;
            }

            _state = value;
        }
    }

    bool groundFound = false;
    bool wallFound = false;
    bool ceilingFound = false;

    public enum Action {
        MoveRight,
        MoveLeft,
        MoveUp,
        MoveDown,
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

    public Vector3[] boundingPoints {
        get {
            return new Vector3[8] {
                new Vector3( transform.position.x, _collider.bounds.center.y + (_collider.bounds.extents.y * 1.1f), transform.position.z ),
                new Vector3(  _collider.bounds.center.x + (_collider.bounds.extents.x * 0.9f),  _collider.bounds.center.y + (_collider.bounds.extents.y * 0.9f), transform.position.z ),
                new Vector3( _collider.bounds.center.x + (_collider.bounds.extents.x * 1.2f), transform.position.y, transform.position.z ),
                new Vector3( _collider.bounds.center.x + (_collider.bounds.extents.x * 0.9f), _collider.bounds.center.y - (_collider.bounds.extents.y * 0.9f), transform.position.z ),
                new Vector3( transform.position.x, _collider.bounds.center.y - (_collider.bounds.extents.y * 1.1f), transform.position.z ),
                new Vector3( _collider.bounds.center.x - (_collider.bounds.extents.x * 0.9f), _collider.bounds.center.y - (_collider.bounds.extents.y * 0.9f), transform.position.z ),
                new Vector3( _collider.bounds.center.x - (_collider.bounds.extents.x * 1.2f), transform.position.y, transform.position.z ),
                new Vector3( _collider.bounds.center.x - (_collider.bounds.extents.x * 0.9f), _collider.bounds.center.y + (_collider.bounds.extents.y * 0.9f), transform.position.z )
            };
        }
    }

    List<Collider> ignoredColliders = new List<Collider>();
    int frontCollisionLayer = int.MaxValue;

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

    public virtual void Init( RuntimeAnimatorController baseController, ParticleController particleController, int team )
    {
        this.baseController = baseController;
        this.particleController = particleController;
        this._team = team;

        Init();
    }
    public virtual void Init()
    {
        _animator = GetComponent<Animator>();
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
    protected virtual TriggerCheck FindTriggerCheck( string name, UnityAction<bool, Collider> action )
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

    public virtual void DoUpdate( GameManager.UpdateData data )
    {
        //EnableTriggerChecks();
        CheckState();

        HandleTechniques();
        if( ValidActiveTechnique() ) {
            activeTechnique.Update(data, _activeActionValue);
        } else {
            HandlePhysics(data);
            HandleAnimation();
        }
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

    public virtual void HandlePhysics( GameManager.UpdateData data )
    {
        HandlePhysics(data, null, null);
    }
    public virtual void HandlePhysics( GameManager.UpdateData data, Vector3? frictionOverride )
    {
        HandlePhysics(data, frictionOverride, null);
    }
    public virtual void HandlePhysics( GameManager.UpdateData data, Vector3? frictionOverride, Vector3? gravityOverride )
    {
        physicsBody.HandleGravity( data, gravityOverride );
        physicsBody.HandleFriction( frictionOverride );
        HandlePropCollisions( data.deltaTime );
    }

    protected virtual void HandlePropCollisions( float deltaTime )
    {
        CheckIgnoredCollisionBounds();

        RaycastHit[] hits;
        if( physicsBody.DetectCollisions(deltaTime, out hits) ) {

            foreach( RaycastHit hit in hits ) {
                bool doCollide = HitCollideLogic(hit);
                if( !doCollide ) {
                    IgnoreCollider(hit.collider, true);
                } else {
                    ParticleEmitter emitter = CreateParticle( "impact", hit.point, Vector3.SignedAngle(Vector3.up, hit.normal, Vector3.forward) );
                    if( emitter != null ) {
                        emitter.Expand(_physicsBody.velocity.magnitude / 12f);
                    }
                }
            }
        }
    }
    protected virtual void CheckIgnoredCollisionBounds()
    {
        for( int i = ignoredColliders.Count - 1; i >= 0; i-- ) {
            bool contained = false;
            foreach( Vector3 boundPoint in boundingPoints ) {
                if( ignoredColliders[i].bounds.Contains(boundPoint) ) {
                    contained = true;
                    break;
                }
            }
            if( !contained ) {
                IgnoreCollider(ignoredColliders[i], false);
            }
        }
    }
    protected virtual bool HitCollideLogic(RaycastHit hit)
    {
        Prop prop = hit.collider.GetComponent<Prop>();
        if( prop == null || !prop.isPassable ) { return true; }
        if( prop.physicsBody.layer > frontCollisionLayer ) { return false; }

        bool invertDown = false;
        foreach( Action action in AgentManager.Instance.settings.propInteractActions ) {
            if( actions[actions.Count - 1] == action && Mathf.Abs(_activeActionValue) > 0 ) {
                invertDown = true;
                break;
            } 
        }

        if( state == State.Grounded ) {
            if( prop.physicsBody.layer <= physicsBody.layer ) {
                return true;
            } else {
                return false;
            }
        } else {
            if( hit.point.y >= hit.collider.bounds.max.y ) {
                return invertDown ? false : true;
            }

            return invertDown ? AgentManager.Instance.settings.defaultIgnorePropCollisions
                : !AgentManager.Instance.settings.defaultIgnorePropCollisions;
        }

    }

    public virtual void HandleAnimation()
    {
        if( state == State.Grounded ) {
            animator.SetBool("Grounded", true );
        } else {
            animator.SetBool("Grounded", false );
            float yVelocity = (physicsBody.velocity.y / 10f);
            yVelocity = yVelocity > 0 ? Mathf.Clamp(yVelocity, 0.5f, 1f): Mathf.Clamp(yVelocity + 1, 0, 0.5f);
            animator.SetFloat("YDirection", yVelocity);
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

    public delegate void ActionEvent( float value );
    public event ActionEvent MoveRight;
    public event ActionEvent MoveLeft;
    public event ActionEvent MoveUp;
    public event ActionEvent MoveDown;
    public event ActionEvent AttackForward;
    public event ActionEvent AttackDown;
    public event ActionEvent AttackBack;
    public event ActionEvent AttackUp;
    public event ActionEvent Block;
    public event ActionEvent Jump;
    public event ActionEvent Dash;
    public event ActionEvent Clash;

    public void SubscribeToActionEvent(Action action, ActionEvent callback, bool unSubscribe = false)
    {
        if( !unSubscribe ) {
            switch( action ) {
                case Action.MoveRight:
                    MoveRight += callback;
                    break;
                case Action.MoveLeft:
                    MoveLeft += callback;
                    break;
                case Action.MoveUp:
                    MoveUp += callback;
                    break;
                case Action.MoveDown:
                    MoveDown += callback;
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
                default:
                    Debug.LogError("Action not implemented");
                    break;
            }
        } else {
            switch( action ) {
                case Action.MoveRight:
                    MoveRight -= callback;
                    break;
                case Action.MoveLeft:
                    MoveLeft -= callback;
                    break;
                case Action.MoveUp:
                    MoveUp -= callback;
                    break;
                case Action.MoveDown:
                    MoveDown -= callback;
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
            case Action.MoveRight:
                handler = MoveRight;
                break;
            case Action.MoveLeft:
                handler = MoveLeft;
                break;
            case Action.MoveUp:
                handler = MoveUp;
                break;
            case Action.MoveDown:
                handler = MoveDown;
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
            handler( value );
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

    protected virtual void SetGroundFound( bool state, Collider other )
    {
        groundFound = state;

        if( state ) {
            SetPhysicsLayer( other.GetComponent<PhysicsBody>() );
        }
    }
    protected virtual void SetWallFound( bool state, Collider other )
    {
        wallFound = state;

        if( other.transform.position.x < transform.position.x ) {
            if( isFacingRight != true ) { isFacingRight = true; }
        } else {
            if( isFacingRight == true ) { isFacingRight = false; }
        }

        if( state ) {
            SetPhysicsLayer( other.GetComponent<PhysicsBody>() );
        }
    }
    protected virtual void SetCeilingFound( bool state, Collider other )
    {
        ceilingFound = state;

        if( state ) {
            SetPhysicsLayer( other.GetComponent<PhysicsBody>() );
        }
    }
    protected virtual void SetPhysicsLayer( PhysicsBody body )
    {
        if( body != null ) {
            physicsBody.layer = body.layer;
        } else {
            physicsBody.layer = 0;
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

            PhysicsBody body = other.GetComponent<PhysicsBody>();
            if( body != null ) {
                frontCollisionLayer = body.layer < frontCollisionLayer ? body.layer : frontCollisionLayer;
            }

        } else {
            if( !ignoredColliders.Contains(other) ) { return; }

            Physics.IgnoreCollision(collider, other, false);
            Physics.IgnoreCollision(_groundCheck.collider, other, false);
            Physics.IgnoreCollision(_ceilingCheck.collider, other, false);
            Physics.IgnoreCollision(_leftWallCheck.collider, other, false);
            Physics.IgnoreCollision(_rightWallCheck.collider, other, false);

            ignoredColliders.Remove(other);

            frontCollisionLayer = int.MaxValue;
            foreach( Collider col in ignoredColliders ) {
                PhysicsBody body = col.GetComponent<PhysicsBody>();
                if( body != null ) {
                    frontCollisionLayer = body.layer < frontCollisionLayer ? body.layer : frontCollisionLayer;
                }
            }
        }
    }

    protected virtual ParticleEmitter CreateParticle( string particleName, Vector3 pos, float angle )
    {
        if( particleController == null ) { return null; }

        ParticleEmitter emitter = ParticleManager.Instance.CreateEmitter( pos, angle, null, particleController.GetParticles(particleName) );
        if( emitter == null ) { return null; }

        return emitter;
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
        if( _groundCheck.doDetect ) {  Gizmos.DrawWireSphere( _groundCheck.transform.position, ((SphereCollider)_groundCheck.collider).radius ); }
        if( _rightWallCheck.doDetect ) {  Gizmos.DrawWireCube( _rightWallCheck.transform.position, ((BoxCollider)_rightWallCheck.collider).size ); }
        if( _leftWallCheck.doDetect ) {  Gizmos.DrawWireCube( _leftWallCheck.transform.position, ((BoxCollider)_leftWallCheck.collider).size ); }
        if( _ceilingCheck.doDetect ) {  Gizmos.DrawWireSphere( _ceilingCheck.transform.position, ((SphereCollider)_ceilingCheck.collider).radius ); }

        Gizmos.color = Color.red;
        foreach( Vector3 boundPoint in boundingPoints ) {
            Gizmos.DrawSphere( boundPoint, 0.02f );
        }

        if( boundaryColliders != null ) {
            for( int i = 0; i < boundaryColliders.Length; i++ ) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(boundaryColliders[i].transform.position, boundaryColliders[i].size);
            }
        }
    }

    #endregion
}