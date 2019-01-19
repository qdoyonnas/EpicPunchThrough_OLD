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
    public bool inFocus = false;

    [Serializable]
    protected struct MenuItemsRow {
        public MenuControl[] row;
    }

    [Header("Menu Items")]
    [SerializeField] protected MenuItemsRow[] menuItems;
    protected int[] _selectedItemCord = { 0, 0 };
    public int[] selectedItemCord { // XXX: I feel array range checks should happen here. However, this must not lose input direction functionality as found in the Input Methods below
        get {
            return _selectedItemCord;
        }
        set {
            if( selectedItem != null ) { selectedItem.UnFocused(this); }

            _selectedItemCord = value;

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

    [SerializeField] protected Selector selector;

    [Header("Navigation Options")]
    public float transitionTime = 0.4f;
    public string[] cancelMenuNames;
    Transitional[] transitionals;

    bool didInit = false;
    GraphicRaycaster raycaster;

    private void Awake()
    {
        Init();
    }
    public virtual void Init()
    {
        if( didInit ) { return; }

        MenuManager.Instance.RegisterMenu(this);

        raycaster = GetComponentInParent<GraphicRaycaster>();

        transitionals = GetComponentsInChildren<Transitional>();
        foreach( Transitional transional in transitionals ) {
            transional.Init();
            transional.TransitionOut(0);
        }
        for( int y = 0; y < menuItems.Length; y++ ) {
            for( int x = 0; x < menuItems[y].row.Length; x++ ) {
                menuItems[y].row[x].Init(new int[]{x, y});
            }
        }
        DisplaySelector(false);

        gameObject.SetActive(false);

        didInit = true;
    }

    public virtual void DoUpdate(GameManager.UpdateData data) {}

    public virtual void TransitionIn(TweenCallback completeAction = null)
    {
        this.gameObject.SetActive(true);

        Tweener tween = null;
        foreach( Transitional transional in transitionals ) {
            tween = transional.TransitionIn(transitionTime);
        }

        if( tween != null ) { tween.OnComplete(() => {
                inFocus = true;
                HandleSubscriptions(true);

                _selectedItemCord[0] = 0; _selectedItemCord[1] = 0;
                if( selectedItem != null ) {  selectedItem.Focused(this); }

                completeAction();
            });
        }
    }
    public virtual void TransitionOut(TweenCallback completeAction = null)
    {
        HandleSubscriptions(false);
        if( selectedItem != null ) {  selectedItem.UnFocused(this); }
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
        if( state ) {
            InputManager.Instance.AnyInput += OnAnyKey;

            InputManager.Instance.UpInput += OnUp;
            InputManager.Instance.RightInput += OnRight;
            InputManager.Instance.DownInput += OnDown;
            InputManager.Instance.LeftInput += OnLeft;
            InputManager.Instance.ConfirmInput += OnConfirm;
            InputManager.Instance.CancelInput += OnCancel;

            InputManager.Instance.MouseMovement += HandlePointerInteraction;
        } else {
            InputManager.Instance.AnyInput -= OnAnyKey;

            InputManager.Instance.UpInput -= OnUp;
            InputManager.Instance.RightInput -= OnRight;
            InputManager.Instance.DownInput -= OnDown;
            InputManager.Instance.LeftInput -= OnLeft;
            InputManager.Instance.ConfirmInput -= OnConfirm;
            InputManager.Instance.CancelInput -= OnCancel;

            InputManager.Instance.MouseMovement -= HandlePointerInteraction;
        }
    }

    #region Selector Methods

    public virtual void DisplaySelector(bool state, bool resetPos = false)
    {
        if( selector == null ) { return; }

        if( resetPos ) { selector.ResetPos(); }
        selector.gameObject.SetActive(state);
    }
    public virtual void MoveSelector(Vector2 pos, float duration)
    {
        MoveSelector(pos, Vector2.zero, duration);
    }
    public virtual void MoveSelector(Vector2 pos, Vector2 size, float duration)
    {
        if( selector == null ) {
            Debug.LogError("Menu " + gameObject.name + " does not have a Selector assgined and a call to move a selector was made to it.");
            return;
        }

        selector.MoveTo(pos, duration);
        if( size != Vector2.zero ) {  selector.SizeTo(size, duration); }
    }
    public virtual void SelectorColor(Color color, float duration)
    {
        if( selector == null ) { return; }

        selector.ColorTo(color, duration);
    }

    #endregion

    #region Input Methods

    protected virtual bool OnAnyKey(bool isDown)
    {
        if( menuItems.Length == 0 || selectedItem == null ) { return false; }

        return selectedItem.HandleAnyInput(this);
    }

    protected virtual bool OnUp(bool isDown)
    {
        if( menuItems.Length == 0 ) { return false; }

        if( selectedItem != null && selectedItem.HandleUpInput(isDown, this) ) { return true; }

        if( isDown ) {
            if( selectedItem != null ) { selectedItem.UnFocused(this); }

            selectedItemCord[0] = 2;

            _selectedItemCord[1]--;
            if( _selectedItemCord[1] < 0 ) {
                _selectedItemCord[1] = menuItems.Length - 1;
            }
            if( _selectedItemCord[0] >= menuItems[_selectedItemCord[1]].row.Length ) {
                _selectedItemCord[0] = menuItems[_selectedItemCord[1]].row.Length - 1;
            }

            if( selectedItem != null ) { selectedItem.Focused(this); }
            return true;
        }

        return false;
    }
    protected virtual bool OnRight(bool isDown)
    {
        if( menuItems.Length == 0 ) { return false; }

        if( selectedItem != null && selectedItem.HandleRightInput(isDown, this) ) { return true; }

        if( isDown ) {
            if( selectedItem != null ) { selectedItem.UnFocused(this); }

            _selectedItemCord[0]++;
            if( _selectedItemCord[0] >= menuItems[_selectedItemCord[1]].row.Length ) {
                _selectedItemCord[0] = 0;
            }
            if( _selectedItemCord[1] >= menuItems.Length ) {
                _selectedItemCord[1] = menuItems.Length - 1;
            }

            if( selectedItem != null ) { selectedItem.Focused(this); }
            return true;
        }
        return false;
    }
    protected virtual bool OnDown(bool isDown)
    {
        if( menuItems.Length == 0 ) { return false; }

        if( selectedItem != null && selectedItem.HandleDownInput(isDown, this) ) { return true; }

        if( isDown ) {
            if( selectedItem != null ) { selectedItem.UnFocused(this); }

            _selectedItemCord[1]++;
            if( _selectedItemCord[1] >= menuItems.Length ) {
                _selectedItemCord[1] = 0;
            }
            if( _selectedItemCord[0] >= menuItems[_selectedItemCord[1]].row.Length ) {
                _selectedItemCord[0] = menuItems[_selectedItemCord[1]].row.Length - 1;
            }

            if( selectedItem != null ) { selectedItem.Focused(this); }
            return true;
        }
        return false;
    }
    protected virtual bool OnLeft(bool isDown)
    {
        if( menuItems.Length == 0 ) { return false; }

        if( selectedItem != null && selectedItem.HandleLeftInput(isDown, this) ) { return true; }

        if( isDown ) {
            if( selectedItem != null ) { selectedItem.UnFocused(this); }

            _selectedItemCord[0]--;
            if( _selectedItemCord[0] < 0 ) {
                _selectedItemCord[0] = menuItems[_selectedItemCord[1]].row.Length - 1;
            }
            if( _selectedItemCord[1] >= menuItems.Length ) {
                _selectedItemCord[1] = menuItems.Length - 1;
            }

            if( selectedItem != null ) { selectedItem.Focused(this); }
            return true;
        }
        return false;
    }
    protected virtual bool OnConfirm(bool isDown)
    {
        if( menuItems.Length == 0 || selectedItem == null ) { return false; }

        return selectedItem.HandleConfirmInput(isDown, this);
    }
    protected virtual bool OnCancel(bool isDown)
    {
        if( selectedItem != null && selectedItem.HandleCancelInput(isDown, this) ) { return true; }

        if( cancelMenuNames.Length > 0 && isDown ) {
            bool didFail = false;
            Menu[] menus = new Menu[cancelMenuNames.Length];

            for( int i = 0; i < cancelMenuNames.Length; i++ ) {
                Menu loadMenu = MenuManager.Instance.GetMenu(cancelMenuNames[i]);
                if( loadMenu != null ) {
                    menus[i] = loadMenu;
                } else {
                    Debug.LogError("Menu: " + gameObject.name + "could not find menu by name of: " + cancelMenuNames[i]);
                    didFail = true;
                }
            }

            if( !didFail ) {
                TransitionOut( () => {
                    foreach( Menu loadMenu in menus ) {
                        loadMenu.TransitionIn();
                    }
                });
            }

            return true;
        }

        return false;
    }

    protected virtual bool HandlePointerInteraction(Vector2 pos, Vector2 delta)
    {
        // Setup Raycast
        if( raycaster == null ) { return false; }

        PointerEventData pointerData = new PointerEventData(MenuManager.Instance.eventSystem);
        pointerData.position = pos;
        List<RaycastResult> resultList = new List<RaycastResult>();
        raycaster.Raycast(pointerData, resultList);
        
        foreach( RaycastResult result in resultList ) {
            // Find all MenuControl components (catching Decorators)
            MenuControl[] items = result.gameObject.GetComponents<MenuControl>();
            foreach( MenuControl item in items ) {
                if( item != null ) {
                    // Skip decorators as they are not recorded by the menu
                    if( item.GetType().IsSubclassOf(typeof(MenuControlDecorator)) ) { continue; }

                    // Check if focus needs to be updated
                    if( item != selectedItem ) {
                        selectedItemCord = item.menuItemCord;
                    }
                    return true;
                }
            }
        }

        return false;
    }

    #endregion
}
