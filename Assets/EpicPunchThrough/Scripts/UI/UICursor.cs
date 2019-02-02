using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UICursor : MonoBehaviour
{
    public bool hideOnStart = false;
    public bool hideOnMouseControlNotActive = true;
    protected Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        HandleSubscriptions(true);

        if( hideOnStart ) {
            SetVisibility(0);
        }
    }
    private void OnDestroy()
    {
        HandleSubscriptions(false);
    }

    void HandleSubscriptions(bool state)
    {
        if( state ) {
            InputManager.Instance.MouseMovement += UpdatePointer;
            InputManager.Instance.ActiveControlChanged += OnActiveControlChange;
        } else {
            InputManager.Instance.MouseMovement -= UpdatePointer;
            InputManager.Instance.ActiveControlChanged -= OnActiveControlChange;
        }
    }

    protected virtual bool UpdatePointer(Vector2 pos, Vector2 delta)
    {
        image.rectTransform.position = pos;

        return true;
    }
    protected virtual void OnActiveControlChange(InputManager.ActiveControlType previouseState, InputManager.ActiveControlType newState)
    {
        if( hideOnMouseControlNotActive ) {
            if( newState != InputManager.ActiveControlType.Mouse ) {
                SetVisibility(0);
            } else {
                SetVisibility(1);
            }
        }


    }

    protected virtual void SetVisibility(float value)
    {
        Color col = image.color;
        col.a = value;
        image.color = col;
    }
}
