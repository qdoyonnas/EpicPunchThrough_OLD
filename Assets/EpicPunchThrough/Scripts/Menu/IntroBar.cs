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

    void Awake()
    {
        MenuManager.Instance.RegisterMenu(this);
        this.gameObject.SetActive(false);

        startPosition = transform.position;
    }
    
    public override void TransitionIn()
    {
        this.gameObject.SetActive(true);

        transform.position = outPosition;
        transform.DOMove(startPosition, transitionTime).OnComplete( () => {
            inFocus = true;
            InputManager.Instance.AnyInput += OnAnyKey;
        } );
    }
    public override void TransitionOut()
    {
        InputManager.Instance.AnyInput -= OnAnyKey;
        
        transform.DOMove(outPosition, transitionTime).OnComplete( () => { this.gameObject.SetActive(false); } );

        inFocus = false;
    }

    void OnAnyKey( bool isDown )
    {
        if( !inFocus ) { return; }

        TransitionOut();
        Menu mainMenu = MenuManager.Instance.GetMenu("MainMenu");
        if( mainMenu != null ) {
            mainMenu.TransitionIn();
        } else {
            Debug.LogError("IntroBar could not find MainMenu");
        }
    }
}
