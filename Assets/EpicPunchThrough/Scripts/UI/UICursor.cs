using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UICursor : MonoBehaviour
{
    public bool hideOnStart = false;
    public bool hideOnMouseControlNotActive = true;
    public bool onlyActiveOnState = false;
    public GameManager.GameState activeOnState;
    protected Image image;

    private bool stateEnabled {
        get {
            return ( !onlyActiveOnState || GameManager.Instance.State == activeOnState );
        }
    }
    private bool activeControlEnabled {
        get {
            return ( !hideOnMouseControlNotActive || InputManager.Instance.activeControlType == InputManager.ActiveControlType.Mouse );
        }
    }

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
            if( onlyActiveOnState ) {
                GameManager.Instance.stateChanged += OnStateChanged;
            }
        } else {
            InputManager.Instance.MouseMovement -= UpdatePointer;
            InputManager.Instance.ActiveControlChanged -= OnActiveControlChange;
            GameManager.Instance.stateChanged -= OnStateChanged;
        }
    }

    protected virtual void OnStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        UpdateVisibility();
    }

    protected virtual bool UpdatePointer(Vector2 pos, Vector2 delta)
    {
        UpdateVisibility();
        image.rectTransform.position = pos;

        return true;
    }
    protected virtual void OnActiveControlChange(InputManager.ActiveControlType previouseState, InputManager.ActiveControlType newState)
    {
        UpdateVisibility();
    }

    protected virtual void UpdateVisibility()
    {
        if( stateEnabled && activeControlEnabled ) {
            SetVisibility(1);
        } else {
            SetVisibility(0);
        }
    }
    protected virtual void SetVisibility(float value)
    {
        Color col = image.color;
        col.a = value;
        image.color = col;
    }
}
