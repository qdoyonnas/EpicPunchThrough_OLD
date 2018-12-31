﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : Menu
{
    public float selectorTweenDuration = 0.2f;

    Vector3 startPosition;
    Vector3 outPosition {
        get {
            Vector3 pos = transform.position;
            pos.x = GetComponentInParent<Canvas>().pixelRect.width;

            return pos;
        }
    }

    Image selector;
    int selectorLocation = 0;
    [SerializeField] Button[] buttons;

    Tweener activeTween;

    void Awake()
    {
        MenuManager.Instance.RegisterMenu(this);
        
        selector = transform.Find("Selector").GetComponent<Image>();
        if( buttons.Length > 0 ) {
            selector.transform.position = buttons[0].transform.position;
        } else {
            Debug.LogError("MainMenu found no buttons");
        }

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

            DisplaySelector(true);

            completeAction();
        } ).SetEase(Ease.OutCirc);
    }
    public override void TransitionOut(TweenCallback completeAction = null)
    {
        HandleSubscriptions(false);
        DisplaySelector(false);

        if( activeTween != null ) { activeTween.Kill(); }
        activeTween = transform.DOMove(outPosition, transitionTime).OnComplete( () => {
            this.gameObject.SetActive(false);

            completeAction();
        } );

        inFocus = false;
    }

    void DisplaySelector(bool state)
    {
        selector.gameObject.SetActive(state);
    }

    void HandleSubscriptions(bool state)
    {
        if( state ) {
            InputManager.Instance.CancelInput += Back;
            InputManager.Instance.UpInput += SelectorUp;
            InputManager.Instance.DownInput += SelectorDown;
            InputManager.Instance.ConfirmInput += Select;
        } else {
            InputManager.Instance.CancelInput -= Back;
            InputManager.Instance.UpInput -= SelectorUp;
            InputManager.Instance.DownInput -= SelectorDown;
            InputManager.Instance.ConfirmInput -= Select;
        }
    }

    public override void DoUpdate( GameManager.UpdateData data )
    {
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
        if( raycaster == null ) { return; }

        PointerEventData

        for( int i = 0; i < buttons.Length; i++ ) {
            raycaster.Raycast(
        }
    }

    #region Input Actions

    void Back(bool isDown)
    {
        if( isDown ) {
            TransitionOut( () => {
                Menu introBar = MenuManager.Instance.GetMenu("IntroBar");
                if( introBar != null ) {
                    introBar.TransitionIn();
                } else {
                    Debug.LogError("MainMenu could not find IntroBar");
                }
            } );
        }
    }

    void SelectorUp(bool isDown)
    {
        if( !inFocus ) { return; }

        if( isDown ) {
            selectorLocation -= 1;
            if( selectorLocation < 0 ) {
                selectorLocation = buttons.Length - 1;
            }
            selector.transform.DOMove(buttons[selectorLocation].transform.position, selectorTweenDuration).SetEase(Ease.InOutCirc);
        }
    }
    void SelectorDown(bool isDown)
    {
        if( !inFocus ) { return; }
        
        if( isDown ) {
            selectorLocation += 1;
            if( selectorLocation >= buttons.Length ) {
                selectorLocation = 0;
            }
            selector.transform.DOMove(buttons[selectorLocation].transform.position, selectorTweenDuration).SetEase(Ease.InOutCirc);
        }
    }
    void Select(bool isDown)
    {
        if( isDown ) { return; }

        Debug.Log("Selected: " + buttons[selectorLocation].gameObject.name);
        buttons[selectorLocation].onClick.Invoke();
    }


    #endregion
}
