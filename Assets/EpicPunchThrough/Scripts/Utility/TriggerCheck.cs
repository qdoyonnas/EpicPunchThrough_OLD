using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerCheck : MonoBehaviour
{
    [Serializable]
    public class TriggerAction : UnityEvent<bool> {}
    public TriggerAction onTrigger;

    Collider _collider;
    public new Collider collider {
        get {
            return _collider;
        }
    }

    public bool doDetect {
         get {
            return collider.enabled;
        }
        set {
            collider.enabled = value;
        }
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter( Collider other )
    {
        onTrigger.Invoke( true );
    }
    private void OnTriggerExit( Collider other )
    {
        onTrigger.Invoke( false );
    }
}
