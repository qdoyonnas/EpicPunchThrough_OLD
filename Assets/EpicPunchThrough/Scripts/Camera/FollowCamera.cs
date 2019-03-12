using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : CameraControl
{
	public Transform target;

    protected override void DoFixedUpdate( GameManager.UpdateData data )
    {
        if( target == null ) { return; }

        cameraBase.Move(target.position);
    }
}
