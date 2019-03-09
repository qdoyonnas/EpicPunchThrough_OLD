using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Prop : MonoBehaviour
{
    public bool isPassable = true;

    protected Rigidbody rigidBody;
    protected Collider[] colliders;

    protected void Start()
    {
        colliders = GetComponentsInChildren<Collider>();

        rigidBody = GetComponent<Rigidbody>();

        PropsManager.Instance.RegisterProp(this);
    }

    public void DoUpdate( GameManager.UpdateData data )
    {

    }
}
