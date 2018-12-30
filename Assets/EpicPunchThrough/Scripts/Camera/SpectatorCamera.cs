using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : CameraControl
{
    // External
	public float speed = 1;
    public float zoomSpeed = 2;

    protected override void DoFixedUpdate( GameManager.UpdateData data )
	{
        HandleZoom();
        HandleMovement();
	}
    void HandleZoom()
    {
        float zoomInput = Input.GetAxis( "Mouse ScrollWheel" ) * zoomSpeed;
        cameraBase.Zoom(zoomInput, true);
    }
    void HandleMovement()
    {
        cameraBase.Move( new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * speed * Time.fixedDeltaTime, true );
    }
}