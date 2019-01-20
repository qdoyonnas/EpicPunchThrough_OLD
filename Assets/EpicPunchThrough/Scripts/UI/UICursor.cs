using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UICursor : MonoBehaviour
{
    public bool hideOnMouseControlNotActive = true;
    protected Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        InputManager.Instance.MouseMovement += UpdatePointer;
        InputManager.Instance.ActiveControlChanged += OnActiveControlChange;
    }

    protected virtual bool UpdatePointer(Vector2 pos, Vector2 delta)
    {
        image.rectTransform.position = pos;

        return true;
    }
    protected virtual void OnActiveControlChange(InputManager.ActiveControlType previouseState, InputManager.ActiveControlType newState)
    {
        if( hideOnMouseControlNotActive ) {
            Color col = image.color;

            if( newState != InputManager.ActiveControlType.Mouse ) {
                col.a = 0;
            } else {
                col.a = 1;
            }

            image.color = col;
        }


    }
}
