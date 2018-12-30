using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Tooltip("Leave blank for auto-find")]
    public CameraBase cameraBase;

    private void Start()
    {
        if( cameraBase == null ) {
            FindCamera();
        }

        GameManager.Instance.fixedUpdated += DoFixedUpdate;
    }
    void FindCamera()
    {
        cameraBase = GetComponent<CameraBase>();
        if( cameraBase == null ) {
            cameraBase = GameManager.Instance.activeCamera;
            if( cameraBase == null ) {
                Debug.LogError("Spectator Camera could not find CameraBase");
            }
        }
    }

    protected virtual void DoFixedUpdate(GameManager.UpdateData data)
    {
        // No action
    }
}
