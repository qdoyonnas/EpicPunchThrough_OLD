using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsBody : MonoBehaviour
{
    public Vector3 velocity {
        get {
            return rigidbody.velocity;
        }
        set {
            SetVelocity(value);
        }
    }
    public Vector3 frictionCoefficients;
    public bool useGravity = true;

    public int layer = 0;

    new protected Rigidbody rigidbody;

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
    }

    public void SetVelocity( Vector3 inVelocity )
    {
        rigidbody.velocity = inVelocity;

        if( velocity.magnitude < AgentManager.Instance.settings.autoStopSpeed ) {
            rigidbody.velocity = Vector3.zero;
        }
    }
    public void AddVelocity( Vector3 inVelocity )
    {
        velocity = velocity + inVelocity;
    }

    public void HandleGravity( GameManager.UpdateData data, Vector3? gravityOverride )
    {
        if( !useGravity ) { return; }

        Vector3 gravity = gravityOverride ?? EnvironmentManager.Instance.GetEnvironment().gravity;
        AddVelocity(gravity * data.deltaTime);
    }
    public void HandleFriction( Vector3? frictionOverride )
    {
        Vector3 friction = frictionOverride ?? frictionCoefficients;

        velocity = new Vector3(velocity.x * (1 - friction.x), velocity.y * (1 - friction.y), velocity.z * (1 - friction.z));
    }

    public bool DetectCollisions( float delaTime, out RaycastHit[] hits )
    {
        RaycastHit[] allHits = rigidbody.SweepTestAll( velocity, velocity.magnitude * delaTime );

        List<RaycastHit> hitList = new List<RaycastHit>();
        for( int i = 0; i < allHits.Length; i++ ) {
            bool newHit = true;
            foreach( RaycastHit b in hitList ) {
                if( allHits[i].collider == b.collider ) {
                    newHit = false;
                    break;
                }
            }
            if( newHit ) {
                hitList.Add(allHits[i]);
            }
        }

        hits = hitList.ToArray();
        return hits.Length > 0;
    }
}

// Old code attempting to overwrite Unity Physics
/*
[RequireComponent(typeof(Rigidbody))]
public class PhysicsBody : MonoBehaviour
{
    public Vector3 velocity {
        get {
            return rigidbody.velocity;
        }
        set {
            SetVelocity(value);
        }
    }
    public Vector3 frictionCoefficients;
    public bool useGravity = true;
    public LayerMask layerMaskOverride;

    public int layer = 0;

    new protected Rigidbody rigidbody;
    protected Vector3 lastPosition;

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        lastPosition = transform.position;
    }

    public void SetVelocity( Vector3 inVelocity )
    {
        rigidbody.velocity = inVelocity;

        if( velocity.magnitude < AgentManager.Instance.settings.autoStopSpeed ) {
            rigidbody.velocity = Vector3.zero;
        }
    }
    public void AddVelocity( Vector3 inVelocity )
    {
        velocity = velocity + inVelocity;
    }

    public void DoUpdate( GameManager.UpdateData data )
    {
        if( !rigidbody.isKinematic ) { return; }

        HandleGravity( data, null );
        HandleFriction( null );

        Vector3 newPosition = transform.position + (velocity * data.deltaTime);
        newPosition = HandleCollisions( newPosition );
        transform.position = newPosition;

        lastPosition = transform.position;
    }

    public void HandleGravity( GameManager.UpdateData data, Vector3? gravityOverride )
    {
        if( !useGravity ) { return; }

        Vector3 gravity = gravityOverride ?? EnvironmentManager.Instance.GetEnvironment().gravity;
        AddVelocity(gravity * data.deltaTime);
    }
    public void HandleFriction( Vector3? frictionOverride )
    {
        Vector3 friction = frictionOverride ?? frictionCoefficients;

        velocity = new Vector3(velocity.x * (1 - friction.x), velocity.y * (1 - friction.y), velocity.z * (1 - friction.z));
    }

    public bool DetectCollisions( float delaTime, out RaycastHit[] hits )
    {
        RaycastHit[] allHits = rigidbody.SweepTestAll( velocity, velocity.magnitude * delaTime );

        List<RaycastHit> hitList = new List<RaycastHit>();
        for( int i = 0; i < allHits.Length; i++ ) {
            bool newHit = true;
            foreach( RaycastHit b in hitList ) {
                if( allHits[i].collider == b.collider ) {
                    newHit = false;
                    break;
                }
            }
            if( newHit ) {
                hitList.Add(allHits[i]);
            }
        }

        hits = hitList.ToArray();
        return hits.Length > 0;
    }

    protected Vector3 HandleCollisions( Vector3 newPosition )
    {
        if( !rigidbody.isKinematic ) { return transform.position; }

        //int mask = ( (int)layerMaskOverride == 0 ? PhysicsCollisionMatrix.MaskForLayer(gameObject.layer) : (int)layerMaskOverride );

        Vector3 direction = (newPosition - transform.position);
        RaycastHit hit;
        if( rigidbody.SweepTest(direction.normalized, out hit, direction.magnitude) ) {
            Vector3 closestPoint = rigidbody.ClosestPointOnBounds(hit.point);
            Vector3 difference = hit.point - closestPoint;

            velocity = velocity - ( hit.normal * Vector3.Dot(velocity, hit.normal) );

            return transform.position + difference;
        }

        return newPosition;
    }
}
*/