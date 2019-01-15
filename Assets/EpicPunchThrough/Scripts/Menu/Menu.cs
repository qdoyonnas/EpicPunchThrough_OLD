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
    protected int[] selectedItemCord = { 0, 0 };
    public MenuControl selectedItem {
        get {
            return menuItems[selectedItemCord[1]].row[selectedItemCord[0]];
        }
    }

    [Header("Selector")]
    [SerializeField] protected Selector selector;
    public Color defaultSelectorColor = Color.gray;
    public Color defaultSelectorSelectColor = Color.green;

    public float transitionTime = 0.4f;
    Transitional[] transitionals;

    bool didInit = false;

    private void Awake()
    {
        Init();
    }
    public virtual void Init()
    {
        if( didInit ) { return; }

        MenuManager.Instance.RegisterMenu(this);

        transitionals = GetComponentsInChildren<Transitional>();
        foreach( Transitional transional in transitionals ) {
            transional.Init();
            transional.TransitionOut(0);
        }

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

                selectedItemCord[0] = 0; selectedItemCord[1] = 1;
                selectedItem.Focused(this);

                completeAction();
            });
        }
    }
    public virtual void TransitionOut(TweenCallback completeAction = null)
    {
        HandleSubscriptions(false);
        DisplaySelector(false);

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

    public virtual void DisplaySelector(bool state)
    {
        if( selector == null ) { return; }

        selector.gameObject.SetActive(state);
    }
    public virtual void MoveSelector(Vector2 pos, float duration)
    {
        if( selector == null ) { return; }

        MoveSelector(pos, selector.size, duration);
    }
    public virtual void MoveSelector(Vector2 pos, Vector2 size, float duration)
    {
        if( selector == null ) { return; }

        selector.MoveTo(pos, duration);
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

        return selectedItem.HandleAnyInput();
    }

    protected virtual bool OnUp(bool isDown)
    {
        if( menuItems.Length == 0 ) { return false; }

        if( selectedItem != null && selectedItem.HandleUpInput(isDown) ) { return true; }

        if( isDown ) {
            selectedItemCord[0]--;
            if( selectedItemCord[0] < 0 ) {
                selectedItemCord[0] = menuItems.GetLength(0)-1;
            }
            return true;
        }

        return false;
    }
    protected virtual bool OnRight(bool isDown)
    {
        if( menuItems.Length == 0 ) { return false; }

        if( selectedItem != null && selectedItem.HandleRightInput(isDown) ) { return true; }

        if( isDown ) {
            selectedItemCord[1]++;
            if( selectedItemCord[1] >= menuItems.GetLength(1) ) {
                selectedItemCord[1] = 0;
            }
            return true;
        }
        return false;
    }
    protected virtual bool OnDown(bool isDown)
    {
        if( menuItems.Length == 0 ) { return false; }

        if( selectedItem != null && selectedItem.HandleDownInput(isDown) ) { return true; }

        if( isDown ) {
            selectedItemCord[0]++;
            if( selectedItemCord[0] >= menuItems.GetLength(0) ) {
                selectedItemCord[0] = 0;
            }
            return true;
        }
        return false;
    }
    protected virtual bool OnLeft(bool isDown)
    {
        if( menuItems.Length == 0 ) { return false; }

        if( selectedItem != null && selectedItem.HandleLeftInput(isDown) ) { return true; }

        if( isDown ) {
            selectedItemCord[1]--;
            if( selectedItemCord[1] < 0 ) {
                selectedItemCord[1] = menuItems.GetLength(1)-1;
            }
            return true;
        }
        return false;
    }
    protected virtual bool OnConfirm(bool isDown)
    {
        if( menuItems.Length == 0 || selectedItem == null ) { return false; }

        return selectedItem.HandleConfirmInput(isDown);
    }
    protected virtual bool OnCancel(bool isDown)
    {
        if( menuItems.Length == 0 || selectedItem == null ) { return false; }

        return selectedItem.HandleCancelInput(isDown);
    }

    protected virtual bool HandlePointerInteraction(Vector2 pos, Vector2 delta)
    {
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
        if( raycaster == null ) { return false; }

        PointerEventData pointerData = new PointerEventData(MenuManager.Instance.eventSystem);
        pointerData.position = pos;
        List<RaycastResult> resultList = new List<RaycastResult>();
        raycaster.Raycast(pointerData, resultList);
        
        foreach( RaycastResult result in resultList ) {
            MenuControl item = result.gameObject.GetComponent<MenuControl>();
            if( item != null ) {
                item.Focused(this);
                return true;
            }
        }

        return false;
    }

    #endregion
}
