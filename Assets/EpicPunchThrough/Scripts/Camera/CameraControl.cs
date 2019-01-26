using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Tooltip("Leave blank for auto-find")]
    protected CameraBase _cameraBase;
    public CameraBase cameraBase {
        get {
            FindCamera();
            return _cameraBase;
        }
        set {
            _cameraBase = value;
        }
    }

    private void Start()
    {
        FindCamera();

        GameManager.Instance.fixedUpdated += DoFixedUpdate;
    }
    private void OnDisable()
    {
        GameManager.Instance.fixedUpdated -= DoFixedUpdate;
    }
    private void OnEnable()
    {
        FindCamera();

        GameManager.Instance.fixedUpdated += DoFixedUpdate;
    }

    void FindCamera()
    {
        if( cameraBase != null ) { return; }

        cameraBase = GetComponent<CameraBase>();
        if( cameraBase == null ) {
            cameraBase = GameManager.Instance.activeCamera;
            if( cameraBase == null ) {
                Debug.LogError("Spectator Camera could not find CameraBase");
                gameObject.SetActive(false);
            }
        }
    }

    protected virtual void DoFixedUpdate(GameManager.UpdateData data)
    {
        // No action
    }
}