using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : CameraBase
{
	public float speed = 20;
	public float velLead = 1.5f;
	public float leadCutoffMargin = 0.8f;
	public float decelDistance = 15f;
	public float snapToDistance = 10f;
	public Transform target;
	public float zoomSpeed = 3;
	public float minimumSize = 4;
	public float maximumSize = 8;

	//temp
	bool isLocked = true;
	
	void Update ()
	{
		float zoomInput = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		if( Mathf.Abs(zoomInput) > 0
			&& Camera.orthographicSize + zoomInput >= minimumSize
			&& Camera.orthographicSize + zoomInput <= maximumSize )
		{
			Camera.orthographicSize += zoomInput;
			OnZoom(zoomInput);
		}
        
		if( Input.GetKeyDown(KeyCode.L) ) {
			isLocked = !isLocked;
		}

		if( isLocked && target != null ) {
			Vector3 focusTarget = target.position;
			Vector3 lockedFocus = new Vector3(focusTarget.x, focusTarget.y, transform.position.z);
			transform.position = lockedFocus;
		}
	}
}
