using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : CameraBase
{
    // External
    [Header("Camera Settings")]
	public float speed = 1;
    public float zoomSpeed = 2;
	public float minimumSize = 4;
	public float maximumSize = 8;

	void FixedUpdate ()
	{
        HandleZoom();
        HandleMovement();
	}

    void HandleZoom()
    {
        float zoomInput = Input.GetAxis( "Mouse ScrollWheel" ) * zoomSpeed;
        if( Mathf.Abs( zoomInput ) > 0 ) {
            Camera.orthographicSize += zoomInput;
            ConfineZoom();

            OnZoom( zoomInput );
        }
    }
    void ConfineZoom()
    {
        if( Camera.orthographicSize < minimumSize ) {
            Camera.orthographicSize = minimumSize;
        } else if( Camera.orthographicSize > maximumSize ) {
            Camera.orthographicSize = maximumSize;
        }
    }

    void HandleMovement()
    {
        Vector3 oldPos = transform.position;

        transform.Translate( new Vector3( Input.GetAxis( "Horizontal" ),
                                        Input.GetAxis( "Vertical" ), 0 ).normalized * speed * Time.fixedDeltaTime );

        CheckWorldBounds();

        if( transform.position != oldPos ) {
            Vector3 translation = oldPos - transform.position;
            OnMove( translation );
        }
    }
}