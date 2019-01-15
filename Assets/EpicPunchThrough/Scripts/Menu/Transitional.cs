using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Transitional : MonoBehaviour
{
    public Vector3 inPosition;
    public Vector3 outPosition;
    public Ease ease = Ease.OutCirc;

    protected RectTransform rectTransform;
    protected Tweener activeTween;

    bool didInit = false;

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        if( didInit ) { return; }

        rectTransform = GetComponent<RectTransform>();

        didInit = true;
    }

    public virtual Tweener TransitionIn(float duration)
    {
        if( activeTween != null ) { activeTween.Kill(); }

        activeTween = rectTransform.DOAnchorPos(inPosition, duration).SetEase(ease);

        return activeTween;
    }
    public virtual Tweener TransitionOut(float duration)
    {
        if( activeTween != null ) { activeTween.Kill(); }

        activeTween = rectTransform.DOAnchorPos(outPosition, duration).SetEase(ease);

        return activeTween;
    }
}
