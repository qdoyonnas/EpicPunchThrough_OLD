using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UICursor : MonoBehaviour
{
    #region Singleton

    private static UICursor _instance;
    public static UICursor instance {
        get {
            if( _instance == null ) {
                Debug.LogError("Call to UICursor. No instance available.");
            }

            return _instance;
        }
    }

    #endregion

    #region Fields
    
    public bool hideOnStart = false;
    public bool hideOnMouseControlNotActive = true;
    public bool onlyActiveOnState = false;
    public GameManager.GameState activeOnState;

    bool isVisible = true;
    float motionMultiplier = 1f;
    protected Image image;

    #endregion

    #region Properties


    protected bool stateEnabled {
        get {
            return ( !onlyActiveOnState || GameManager.Instance.State == activeOnState );
        }
    }
    protected bool activeControlEnabled {
        get {
            return ( !hideOnMouseControlNotActive || InputManager.Instance.activeControlType == InputManager.ActiveControlType.Mouse );
        }
    }

    protected float imageTop {
         get {
            return image.rectTransform.localPosition.y + ( image.rectTransform.rect.height * (1 - image.rectTransform.pivot.y) );
        }
    }
    protected float imageBottom {
        get {
            return image.rectTransform.localPosition.y - ( image.rectTransform.rect.height * image.rectTransform.pivot.y );
        }
    }
    protected float imageLeft {
        get {
            return image.rectTransform.localPosition.x + ( image.rectTransform.rect.width * image.rectTransform.pivot.x );
        }
    }
    protected float imageRight {
         get {
            return image.rectTransform.localPosition.x + ( image.rectTransform.rect.width * (1 - image.rectTransform.pivot.x) );
        }
    }

    #endregion

    #region Events

    public delegate bool PositionUpdateDelegate( Vector2 pos, Vector2 delta );
    public static event PositionUpdateDelegate PositionChanged;

    #endregion

    private void Awake()
    {
        if( _instance != null ) { Destroy(this); return; }

        _instance = this;
        motionMultiplier = InputManager.Instance.settings.pointerSensitivity;

        image = GetComponent<Image>();
        HandleSubscriptions(true);

        if( hideOnStart ) {
            SetVisibility(0);
        }
    }
    private void OnDestroy()
    {
        _instance = null;
        HandleSubscriptions(false);
    }

    void HandleSubscriptions(bool state)
    {
        if( InputManager.Instance == null || GameManager.Instance == null ) { return; }

        if( state ) {
            InputManager.Instance.PointerHorizontal += OnHorizontal;
            InputManager.Instance.PointerVertical += OnVertical;
            InputManager.Instance.ActiveControlChanged += OnActiveControlChange;
            if( onlyActiveOnState ) {
                GameManager.Instance.stateChanged += OnStateChanged;
            }
        } else {
            InputManager.Instance.PointerHorizontal -= OnHorizontal;
            InputManager.Instance.PointerVertical -= OnVertical;
            InputManager.Instance.ActiveControlChanged -= OnActiveControlChange;
            GameManager.Instance.stateChanged -= OnStateChanged;
        }
    }

    #region Movement

    protected virtual bool OnHorizontal(float value)
    {
        Vector3 oldPos = image.rectTransform.position;

        UpdateVisibility();
        image.rectTransform.position = new Vector3(image.rectTransform.position.x + (value * motionMultiplier), image.rectTransform.position.y, 0);

        CheckBounds();

        if( isVisible && image.rectTransform.position != oldPos && PositionChanged != null ) {
            PositionChanged( image.rectTransform.position, image.rectTransform.position - oldPos );
        }

        return true;
    }
    protected virtual bool OnVertical(float value)
    {
        Vector3 oldPos = image.rectTransform.position;

        UpdateVisibility();
        image.rectTransform.position = new Vector3(image.rectTransform.position.x, image.rectTransform.position.y + (value * motionMultiplier), 0);

        CheckBounds();

        if( isVisible && image.rectTransform.position != oldPos && PositionChanged != null ) {
            PositionChanged( image.rectTransform.position, image.rectTransform.position - oldPos );
        }

        return true;
    }
    protected virtual void CheckBounds()
    {
        if( imageTop > image.canvas.pixelRect.height/2 ) {
            image.rectTransform.localPosition = new Vector3(image.rectTransform.localPosition.x, image.canvas.pixelRect.height/2, 0);
        } else if( imageBottom < -image.canvas.pixelRect.height/2 ) {
            image.rectTransform.localPosition = new Vector3(image.rectTransform.localPosition.x, image.rectTransform.rect.height + (-image.canvas.pixelRect.height/2), 0);
        }
        
        if( imageLeft < -image.canvas.pixelRect.width/2 ) {
            image.rectTransform.localPosition = new Vector3(-image.canvas.pixelRect.width/2, image.rectTransform.localPosition.y, 0);
        } else if( imageRight > image.canvas.pixelRect.width/2 ) {
            image.rectTransform.localPosition = new Vector3(image.canvas.pixelRect.width/2 - (image.rectTransform.rect.width/2), image.rectTransform.localPosition.y, 0);
        }
    }

    #endregion

    #region Visibility

    protected virtual void OnStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        UpdateVisibility();
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

        isVisible = ( value > 0 );
    }

    #endregion
}
