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

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
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

        HandleGravity( data );
        HandleFriction();

        Vector3 newPosition = transform.position + (velocity * data.deltaTime);
        newPosition = HandleCollisions( newPosition );
        transform.position = newPosition;

        lastPosition = transform.position;
    }

    public void HandleGravity( GameManager.UpdateData data )
    {
        if( !rigidbody.isKinematic || !useGravity ) { return; }
        AddVelocity(EnvironmentManager.Instance.GetEnvironment().gravity * data.deltaTime);
    }
    public void HandleFriction()
    {
        velocity = new Vector3(velocity.x * (1 - frictionCoefficients.x), velocity.y * (1 - frictionCoefficients.y), velocity.z * (1 - frictionCoefficients.z));
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
