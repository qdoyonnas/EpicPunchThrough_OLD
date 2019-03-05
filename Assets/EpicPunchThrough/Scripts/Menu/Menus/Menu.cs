using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// Interface for Menu objects
/// </summary>
public class Menu: MonoBehaviour
{
    bool _inFocus = false;
    public bool inFocus {
        get {
            return _inFocus;
        }
        set {
            if( value ) {
                if( selectedItem != null ) {  selectedItem.Focused(this); }
            } else {
                if( selectedItem != null ) {  selectedItem.UnFocused(this); }
            }
            HandleSubscriptions(value);
            _inFocus = value;
        }
    }

    [Serializable]
    protected struct MenuItemsRow {
        public MenuControl[] row;
    }

    [Header("Menu Items")]
    [SerializeField] protected MenuItemsRow[] menuItems;
    protected Vector2Int _selectedItemCord = new Vector2Int();
    public Vector2Int selectedItemCord { // XXX: I feel array range checks should happen here. However, this must not lose input direction functionality as found in the Input Methods below
        get {
            return _selectedItemCord;
        }
        set {
            if( selectedItem != null ) { selectedItem.UnFocused(this); }

            int xDirection = 0;
            int yDirection = 0;

            // Determine direction of change
            if( value[0] > _selectedItemCord[0] ) {
                xDirection = 1;
            } else if( value[0] < _selectedItemCord[1] ) {
                xDirection = -1;
            }
            if( value[1] > _selectedItemCord[1] ) {
                yDirection = 1;
            } else if( value[1] < _selectedItemCord[1] ) {
                yDirection = -1;
            }

            _selectedItemCord = value;

            // Determine if warp or cap behavior - only wrap if direction = difference
            if( _selectedItemCord[1] < 0 ) {
                if( yDirection == -1 ) {
                    _selectedItemCord[1] = menuItems.Length-1; // wrap
                } else {
                    _selectedItemCord[1] = 0;
                }
            } else if( _selectedItemCord[1] >= menuItems.Length ) {
                if( yDirection == 1 ) {
                    _selectedItemCord[1] = 0; // wrap
                } else {
                    _selectedItemCord[1] = menuItems.Length-1;
                }
            }

            if( _selectedItemCord[0] < 0 ) {
                if( xDirection == -1 ) {
                    _selectedItemCord[0] = menuItems[_selectedItemCord[1]].row.Length-1; // wrap
                } else {
                    _selectedItemCord[0] = 0;
                }
            } else if( _selectedItemCord[0] >= menuItems[_selectedItemCord[1]].row.Length ) {
                if( xDirection == 1 ) {
                    _selectedItemCord[0] = 0; // wrap
                } else {
                    _selectedItemCord[0] = menuItems[_selectedItemCord[1]].row.Length-1;
                }
            }

            if( selectedItem != null ) { selectedItem.Focused(this); }
        }
    }
    public MenuControl selectedItem {
        get {
            if( _selectedItemCord[1] >= menuItems.Length
                || _selectedItemCord[0] >= menuItems[_selectedItemCord[1]].row.Length )
            {
                return null;
            }
            
            return menuItems[_selectedItemCord[1]].row[_selectedItemCord[0]];
        }
    }

    protected Selector selector;

    [Header("Navigation Options")]
    public float transitionTime = 0.4f;
    Transitional[] transitionals;
    bool inFrame = false;

    protected MenuDecorator decorator;
    GraphicRaycaster raycaster;

    #region Initialization

    bool didInit = false;
    private void Awake()
    {
        Init();
    }
    public virtual void Init()
    {
        if( didInit ) { return; }

        MenuManager.Instance.RegisterMenu(this);

        raycaster = GetComponentInParent<GraphicRaycaster>();

        InitDecorators();
        InitTransitionals();
        InitMenuItems();
        InitSelector();

        gameObject.SetActive(false);

        didInit = true;
    }
    void InitDecorators()
    {
        MenuDecorator[] decorators = GetComponents<MenuDecorator>();
        MenuDecorator last = null;
        foreach( MenuDecorator decor in decorators ) {
            if( last == null ) {
                this.decorator = decor;
            } else {
                last.Init(this, decor);
            }
            last = decor;
        }
        if( last != null ) { last.Init(this, null); }
    }
    void InitTransitionals()
    {
        transitionals = GetComponentsInChildren<Transitional>();
        foreach( Transitional transional in transitionals ) {
            transional.Init();
            transional.TransitionOut(0);
        }
    }
    void InitMenuItems()
    {
        for( int y = 0; y < menuItems.Length; y++ ) {
            for( int x = 0; x < menuItems[y].row.Length; x++ ) {
                menuItems[y].row[x].Init(new Vector2Int(x, y));
            }
        }
    }
    void InitSelector()
    {
        selector = GetComponentInChildren<Selector>();
        DisplaySelector(false);
    }

    #endregion

    private void OnDestroy()
    {
        MenuManager.Instance.UnregisterMenu(this);
        HandleSubscriptions(false);
    }

    public virtual void DoUpdate(GameManager.UpdateData data)
    {
        if( decorator != null ) { decorator.DoUpdate(data); }
    }

    public virtual void TransitionIn(TweenCallback completeAction = null)
    {
        if( decorator != null ) { decorator.TransitionIn(completeAction); }

        this.gameObject.SetActive(true);

        if( inFrame ) {
            inFocus = true;
            completeAction();
        } else {
            Tweener tween = null;
            foreach( Transitional transional in transitionals ) {
                tween = transional.TransitionIn(transitionTime);
            }

            if( tween != null ) { tween.OnComplete(() => {
                    _selectedItemCord[0] = 0; _selectedItemCord[1] = 0;
                    inFocus = true;
                    inFrame = true;

                    completeAction();
                });
            }
        }
    }
    public virtual void TransitionOut(TweenCallback completeAction = null)
    {
        if( decorator != null ) { decorator.TransitionIn(completeAction); }

        inFocus = false;
        inFrame = false;
        DisplaySelector(false, true);

        Tweener tween = null;
        foreach( Transitional transional in transitionals ) {
            tween = transional.TransitionOut(transitionTime);
        }

        if( tween != null ) { tween.OnComplete(() => {
                this.gameObject.SetActive(false);

                completeAction();
            });
        }
    }

    protected virtual void HandleSubscriptions(bool state)
    {
        if( decorator != null ) { decorator.HandleSubscriptions(state); }

        if( state ) {
            InputManager.Instance.AnyInput += OnAnyKey;

            InputManager.Instance.HorizontalInput += OnHorizontal;
            InputManager.Instance.VerticalInput += OnVertical;
            InputManager.Instance.ConfirmInput += OnConfirm;
            InputManager.Instance.CancelInput += OnCancel;

            UICursor.PositionChanged += OnPointerPositionChange;
        } else {
            InputManager.Instance.AnyInput -= OnAnyKey;

            InputManager.Instance.HorizontalInput -= OnHorizontal;
            InputManager.Instance.VerticalInput -= OnVertical;
            InputManager.Instance.ConfirmInput -= OnConfirm;
            InputManager.Instance.CancelInput -= OnCancel;

            UICursor.PositionChanged -= OnPointerPositionChange;
        }
    }

    #region Selector Methods

    public virtual void DisplaySelector(bool state, bool resetPos = false)
    {
        if( decorator != null ) { decorator.DisplaySelector(state, resetPos); }

        if( selector == null ) { return; }

        if( resetPos ) { selector.ResetSettings(); }
        selector.gameObject.SetActive(state);
    }
    public virtual void MoveSelector(Vector2 pos, float duration)
    {
        MoveSelector(pos, Vector2.zero, duration);
    }
    public virtual void MoveSelector(Vector2 pos, Vector2 size, float duration)
    {
        if( decorator != null ) { decorator.MoveSelector(pos, size, duration); }

        if( selector == null ) {
            Debug.LogError("Menu " + gameObject.name + " does not have a Selector assigned and a call to move a selector was made to it.");
            return;
        }

        selector.MoveTo(pos, duration);
        if( size != Vector2.zero ) {  selector.SizeTo(size, duration); }
    }
    public virtual void SelectorColor(Color color, float duration)
    {
        if( decorator != null ) { decorator.SelectorColor(color, duration); }

        if( selector == null ) {
            Debug.LogError("Menu " + gameObject.name + " does not have a Selector assigned and a call to change color of a selector was made to it.");
            return;
        }

        selector.ColorTo(color, duration);
    }

    #endregion

    #region Input Methods

    protected virtual bool OnAnyKey(float value)
    {
        if( menuItems.Length > 0 && selectedItem != null && selectedItem.HandleAnyInput(this) ) { return true; }

        if( decorator != null ) { decorator.OnAnyKey(value); }
        return false;
    }

    protected virtual bool OnHorizontal(float value)
    {
        if( menuItems.Length > 0 && selectedItem != null && selectedItem.HandleHorizontal(value, this) ) { return true; }

        if( decorator != null ) { decorator.OnHorizontal(value); }
        return false;
    }
    protected virtual bool OnVertical(float value)
    {
        if( menuItems.Length > 0 && selectedItem != null && selectedItem.HandleVertical(value, this) ) { return true; }

        if( decorator != null ) { decorator.OnVertical(value); }
        return false;
    }
    protected virtual bool OnConfirm(float value)
    {
        if( menuItems.Length > 0 && selectedItem != null && selectedItem.HandleConfirmInput(value, this) ) { return true; }

        if( decorator != null ) { decorator.OnConfirm(value); }
        return false;
    }
    protected virtual bool OnCancel(float value)
    {
        if( menuItems.Length > 0 && selectedItem != null && selectedItem.HandleCancelInput(value, this) ) { return true; }

        if( decorator != null ) { decorator.OnCancel(value); }
        return false;
    }

    protected virtual bool OnPointerPositionChange( Vector2 pos, Vector2 delta )
    {
        if( !inFocus ) { return false; }

        PointerEventData pointerData = new PointerEventData(MenuManager.Instance.eventSystem);
        pointerData.position = pos;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach( RaycastResult result in results ) {
            MenuControl[] controls = result.gameObject.GetComponents<MenuControl>();
            foreach( MenuControl control in controls ) {
                if( !control.GetType().IsSubclassOf(typeof(MenuControlDecorator)) ) {
                    if( control.OnPointerPositionChange( pos, delta, this ) ) {
                        return true;
                    } else if( selectedItem != control ) {
                        selectedItemCord = control.menuItemCord;
                        return true;
                    }
                }
            }
        }

        if( decorator != null ) { decorator.OnPointerPositionChange( pos, delta ); }

        return false;
    }

    #endregion
}
