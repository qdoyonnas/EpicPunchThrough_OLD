using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenu : Menu
{
    Vector3 startPosition;
    Vector3 outPosition {
        get {
            Vector3 pos = transform.position;
            pos.x = GetComponentInParent<Canvas>().pixelRect.width;

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
        } );
    }
    public override void TransitionOut()
    {
        transform.DOMove(outPosition, transitionTime).OnComplete( () => { this.gameObject.SetActive(false); } );

        inFocus = false;
    }
}
