using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : CameraControl
{
	public Transform target;
    public bool offsetByTargetVelocity;

    public float acceleration;
    public float dampening;
    public float friction;
    public float stopSpeed;
    public Vector3 deadZone;

    protected Vector3 velocity = new Vector3();

    protected override void DoFixedUpdate( GameManager.UpdateData data )
    {
        if( target == null ) { return; }

        Vector3 targetPosition = target.position;
        if( offsetByTargetVelocity ) {
            PhysicsBody targetBody = target.GetComponent<PhysicsBody>();
            if( targetBody != null ) {
                targetPosition += targetBody.velocity;
            }
        }

        Vector3 offset = targetPosition - cameraBase.transform.position;

        if( Mathf.Sign(velocity.x) != Mathf.Sign(offset.x) ) {
            velocity.x = velocity.x * (1 - dampening);
        }
        velocity.x += (offset.x / deadZone.x) * acceleration * data.deltaTime;

        if( Mathf.Sign(velocity.y) != Mathf.Sign(offset.y) ) {
            velocity.y = velocity.y * (1 - dampening);
        }
        velocity.y += (offset.y / deadZone.y) * acceleration * data.deltaTime;

        if( velocity.magnitude > 0 ) {
            velocity = velocity * (1 - friction);
            if( velocity.magnitude < stopSpeed ) {
                velocity = new Vector3();
            }
        }

        Vector3 oldPosition = cameraBase.transform.position;
        cameraBase.Move(velocity, true);
        velocity = cameraBase.transform.position - oldPosition;
    }
}
