using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntrance : MonoBehaviour
{
    public float cameraSize = 5;

    private void Start()
    {
        if( GameManager.Instance.activeCamera != null ) {
            GameManager.Instance.activeCamera.Move(transform.position);
            GameManager.Instance.activeCamera.Zoom(cameraSize);
        }
    }
}
