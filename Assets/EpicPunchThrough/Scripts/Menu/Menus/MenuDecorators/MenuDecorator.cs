using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MenuDecorator: MonoBehaviour
{
    protected Menu menu;
    protected MenuDecorator decorator;

    public virtual void Init(Menu in_menu, MenuDecorator in_decorator)
    {
        menu = in_menu;
        decorator = in_decorator;
    }

    public virtual void HandleSubscriptions( bool state )
    {
        if( decorator != null ) { decorator.HandleSubscriptions(state); }
    }

    public virtual void TransitionIn( TweenCallback completeAction = null )
    {
        if( decorator != null ) { decorator.TransitionIn(completeAction); }
    }
    public virtual void TransitionOut( TweenCallback completeAction = null )
    {
        if( decorator != null ) { decorator.TransitionOut(completeAction); }
    }

    public virtual void DoUpdate(GameManager.UpdateData data) {}

    public virtual void DisplaySelector( bool state, bool resetPos )
    {
        if( decorator != null ) { decorator.DisplaySelector(state, resetPos); }
    }
    public virtual void MoveSelector( Vector2 pos, Vector2 size, float duration )
    {
        if( decorator != null ) { decorator.MoveSelector(pos, size, duration); }
    }
    public virtual void SelectorColor( Color color, float duration )
    {
        if( decorator != null ) { decorator.SelectorColor(color, duration); }
    }
 
    public virtual bool OnAnyKey( bool isDown )
    {
        if( decorator != null ) { decorator.OnAnyKey(isDown); }

        return false;
    }
    public virtual bool OnUp( bool isDown )
    {
        if( decorator != null ) { decorator.OnUp(isDown); }

        return false;
    }
    public virtual bool OnRight( bool isDown )
    {
        if( decorator != null ) { decorator.OnRight(isDown); }

        return false;
    }
    public virtual bool OnDown( bool isDown )
    {
        if( decorator != null ) { decorator.OnDown(isDown); }

        return false;
    }
    public virtual bool OnLeft( bool isDown )
    {
        if( decorator != null ) { decorator.OnLeft(isDown); }

        return false;
    }
    public virtual bool OnConfirm( bool isDown )
    {
        if( decorator != null ) { decorator.OnConfirm(isDown); }

        return false;
    }
    public virtual bool OnCancel( bool isDown )
    {
        if( decorator != null ) { decorator.OnCancel(isDown); }

        return false;
    }

    public virtual bool HandlePointerInteraction( Vector2 pos, Vector2 delta )
    {
        if( decorator != null ) { decorator.HandlePointerInteraction(pos, delta); }

        return false;
    }
}