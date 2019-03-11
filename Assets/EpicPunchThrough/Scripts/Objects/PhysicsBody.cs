using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsBody : MonoBehaviour
{
    protected Vector3 _velocity;
    public Vector3 velocity {
        get {
            return _velocity;
        }
        set {
            SetVelocity( value );
        }
    }
    public Vector3 frictionCoefficients;
    public bool useGravity {
        get {
            return rigidbody.useGravity;
        }
        set {
            rigidbody.useGravity = value;
        }
    }
    public LayerMask layerMaskOverride;

    new protected Rigidbody rigidbody;
    protected Vector3 lastPosition;

    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    public void SetVelocity( Vector3 inVelocity )
    {
        _velocity = inVelocity;

        if( _velocity.magnitude < AgentManager.Instance.settings.autoStopSpeed ) {
            _velocity = Vector3.zero;
        }

        rigidbody.velocity = _velocity;
    }
    public void AddVelocity( Vector3 inVelocity )
    {
        velocity = _velocity + inVelocity;
    }

    public void DoUpdate( GameManager.UpdateData data )
    {
        HandleGravity( data );
        HandleFriction();

        Vector3 newPosition = transform.position + (_velocity * data.deltaTime);
        newPosition = HandleCollisions( newPosition );
        transform.position = newPosition;

        lastPosition = transform.position;
    }

    public void HandleGravity( GameManager.UpdateData data )
    {
        if( !useGravity ) { return; }
        AddVelocity(EnvironmentManager.Instance.GetEnvironment().gravity * data.deltaTime);
    }
    public void HandleFriction()
    {
        velocity = new Vector3(velocity.x * (1 - frictionCoefficients.x), velocity.y * (1 - frictionCoefficients.y), velocity.z * (1 - frictionCoefficients.z));
    }

    protected Vector3 HandleCollisions( Vector3 newPosition )
    {
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
