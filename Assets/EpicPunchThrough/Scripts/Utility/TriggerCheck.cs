using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerCheck : MonoBehaviour
{
    [Serializable]
    public class TriggerAction : UnityEvent<bool, Collider> {}
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
            if( !value ) {
                onTrigger.Invoke( false, null );
            }
        }
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter( Collider other )
    {
        onTrigger.Invoke( true, other );
    }
    private void OnTriggerExit( Collider other )
    {
        onTrigger.Invoke( false, other );
    }
}
