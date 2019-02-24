using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionCamera : CameraControl
{
    public Vector2 motion;

    protected override void DoFixedUpdate( GameManager.UpdateData data )
    {
        Vector2 fixedMotion = motion * data.deltaTime;

        cameraBase.Move(fixedMotion, true);
    }
}
