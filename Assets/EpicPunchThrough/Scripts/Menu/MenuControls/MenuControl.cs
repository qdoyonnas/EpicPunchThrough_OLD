using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    protected Vector2Int _menuItemCord = new Vector2Int();
    public Vector2Int menuItemCord {
        get {
            return _menuItemCord;
        }
    }

    protected bool didInit = false;
    protected MenuControlDecorator decorator;

    private void Awake()
    {
        Init(new Vector2Int(-1, -1));
    }
    public virtual void Init(Vector2Int cord)
    {
        if( cord.x != -1 ) { _menuItemCord = cord; }
        if( didInit ) { return; }

        MenuControlDecorator[] decorators = GetComponents<MenuControlDecorator>();
        MenuControl next = this;
        foreach( MenuControlDecorator decor in decorators ) {
            next.decorator = decor;
            next = decor;
        }

        didInit = true;
    }

    public virtual void Focused(Menu menu)
    {
        if( decorator != null ) { decorator.Focused(menu); }
    }
    public virtual void UnFocused(Menu menu)
    {
        if( decorator != null ) { decorator.UnFocused(menu); }
    }

    public virtual bool HandleAnyInput(Menu menu)
    {
        if( decorator != null ) { decorator.HandleAnyInput(menu); }

        return false;
    }
    public virtual bool HandleUpInput(bool isDown, Menu menu)
    {
        if( decorator != null ) { decorator.HandleUpInput(isDown, menu); }

        return false;
    }
    public virtual bool HandleRightInput(bool isDown, Menu menu)
    {
        if( decorator != null ) { decorator.HandleRightInput(isDown, menu); }

        return false;
    }
    public virtual bool HandleDownInput(bool isDown, Menu menu)
    {
        if( decorator != null ) { decorator.HandleDownInput(isDown, menu); }

        return false;
    }
    public virtual bool HandleLeftInput(bool isDown, Menu menu)
    {
        if( decorator != null ) { decorator.HandleLeftInput(isDown, menu); }

        return false;
    }
    public virtual bool HandleConfirmInput(bool isDown, Menu menu)
    {
        if( decorator != null ) { decorator.HandleConfirmInput(isDown, menu); }

        return false;
    }
    public virtual bool HandleCancelInput(bool isDown, Menu menu)
    {
        if( decorator != null ) { decorator.HandleCancelInput(isDown, menu); }

        return false;
    }
}