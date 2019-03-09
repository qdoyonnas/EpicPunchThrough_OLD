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
    public bool usesGravity = true;

    new protected Rigidbody rigidbody;

    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void SetVelocity( Vector3 inVelocity )
    {
        _velocity = inVelocity;

        if( _velocity.magnitude < AgentManager.Instance.settings.autoStopSpeed ) {
            _velocity = Vector3.zero;
        }
    }
    public void AddVelocity( Vector3 inVelocity )
    {
        velocity = _velocity + inVelocity;
    }

    public void DoUpdate( GameManager.UpdateData data )
    {
        HandleGravity( data );
        HandleFriction();

        transform.position += _velocity * data.deltaTime;
    }

    protected void HandleGravity( GameManager.UpdateData data )
    {
        if( !usesGravity ) { return; }
        AddVelocity(EnvironmentManager.Instance.GetEnvironment().gravity * data.deltaTime);
    }
    protected void HandleFriction()
    {
        velocity = new Vector3(velocity.x * (1 - frictionCoefficients.x), velocity.y * (1 - frictionCoefficients.y), velocity.z * (1 - frictionCoefficients.z));
    }
}
