using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IntroBar : Menu
{
    Vector3 startPosition;
    Vector3 outPosition {
        get {
            Vector3 pos = transform.position;
            pos.y = 0;

            return pos;
        }
    }

    Tweener activeTween;

    void Awake()
    {
        MenuManager.Instance.RegisterMenu(this);

        startPosition = transform.position;
        transform.position = outPosition;

        this.gameObject.SetActive(false);
    }
    
    public override void TransitionIn(TweenCallback completeAction = null)
    {
        this.gameObject.SetActive(true);
        
        if( activeTween != null ) { activeTween.Kill(); }
        activeTween = transform.DOMove(startPosition, transitionTime).OnComplete( () => {
            inFocus = true;
            HandleSubscriptions(true);

            completeAction();
        } );
    }
    public override void TransitionOut(TweenCallback completeAction = null)
    {
        HandleSubscriptions(false);
        
        if( activeTween != null ) { activeTween.Kill(); }
        activeTween = transform.DOMove(outPosition, transitionTime).OnComplete( () => {
            this.gameObject.SetActive(false);

            completeAction();
        } );

        inFocus = false;
    }

    void HandleSubscriptions(bool state)
    {
        if( state ) {
            InputManager.Instance.AnyInput += OnAnyKey;
            InputManager.Instance.CancelInput += Exit;
        } else {
            InputManager.Instance.AnyInput -= OnAnyKey;
            InputManager.Instance.CancelInput -= Exit;
        }
    }

    void Exit(bool isDown)
    {
        if( !isDown ) { return; }

        Application.Quit();
        Debug.Log("QUIT");
    }
    void OnAnyKey( bool isDown )
    {
        if( !isDown ) { return; }
        if( InputManager.Instance.GetInput("cancel") ) { return; }

        TransitionOut();

        Menu mainMenu = MenuManager.Instance.GetMenu("MainMenu");
        if( mainMenu != null ) {
            mainMenu.TransitionIn();
        } else {
            Debug.LogError("IntroBar could not find MainMenu");
        }
    }
}
