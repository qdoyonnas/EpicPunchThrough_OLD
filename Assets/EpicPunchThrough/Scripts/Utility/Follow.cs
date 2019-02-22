using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Follow : MonoBehaviour
{
    #region Fields

    public Transform target;

    public bool followX;
    public bool followY;
    public bool followZ;
    public bool followAngle;
    
    public enum TrackType {
        Instant,
        Tween,
        Dynamic
    }

    private TrackType _positionTrack;
    public TrackType positionTrack {
        get {
            return _positionTrack;
        }
        set {
            _positionTrack = value;

            savedPosition = new Vector3();
            velocity = new Vector3();

            if( moveTween != null ) {
                moveTween.Kill();
            }
        }
    }

    private TrackType _rotationTrack;
    public TrackType rotationTrack {
        get {
            return _rotationTrack;
        }
        set {
            _rotationTrack = value;

            savedRotation = new Quaternion();
            angularVelocity = new Vector3();

            if( rotateTween != null ) {
                rotateTween.Kill();
            }
        }
    }
    
    #region Tween

    public Ease moveEase;
    public float moveTime;

    public Ease rotateEase;
    public float rotateTime;

    #endregion

    #region Dynamic

    public float acceleration = 0f;
    public float rotationAcceleration = 0f;
    public bool onFixedUpdate;

    #endregion

    private Vector3 savedPosition = new Vector3();
    private Quaternion savedRotation = new Quaternion();

    private Vector3 velocity = new Vector3();
    private Vector3 angularVelocity = new Vector3();

    private Tween moveTween;
    private Tween rotateTween;

    #endregion

    private void Update()
    {
        if( !onFixedUpdate ) {
            FollowTarget();
        }
    }
    private void FixedUpdate()
    {
        if( onFixedUpdate ) {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        if( target == null ) { return; }

        FollowPosition();
        FollowRotation();
    }
    private void FollowPosition()
    {
        Vector3 targetPosition = new Vector3( followX ? target.position.x : transform.position.x,
                                        followY ? target.position.y : transform.position.y,
                                        followZ ? target.position.z : transform.position.z );

        switch( positionTrack ) {
            case TrackType.Instant:
                transform.position = targetPosition;
                return;

            case TrackType.Tween:
                if( moveTween != null ) {
                    if( targetPosition == savedPosition ) {
                        return;
                    }
                    moveTween.Kill();
                }
                transform.DOMove(targetPosition, moveTime).SetEase(moveEase);
                savedPosition = targetPosition;
                return;

            case TrackType.Dynamic:
                Vector3 direction = (targetPosition - transform.position).normalized;
                velocity += direction * acceleration * ( onFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime );
                transform.position += velocity;
                return;
        }

        

    }
    private void FollowRotation()
    {
        if( !followAngle ) { return; }

        switch( rotationTrack ) {
            case TrackType.Instant:
                transform.rotation = target.rotation;
                return;

            case TrackType.Tween:
                if( rotateTween != null ) {
                    if( target.rotation == savedRotation ) {
                        return;
                    }
                    rotateTween.Kill();
                }
                transform.DORotate(target.eulerAngles, rotateTime).SetEase(rotateEase);
                savedRotation = target.rotation;
                return;

            case TrackType.Dynamic:
                Vector3 direction = (target.eulerAngles - transform.eulerAngles).normalized;
                angularVelocity += direction * acceleration * ( onFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime );
                transform.eulerAngles = angularVelocity;
                return;
        }
    }
}
