using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Transitional : MonoBehaviour
{
    [Tooltip("Position when visible and active")]
    public Vector3 inPosition;
    [Tooltip("Position of entering from")]
    public Vector3 outPosition;
    [Tooltip("Position to exit to")]
    public Vector3 exitPosition;
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

        rectTransform.anchoredPosition = outPosition;
        activeTween = rectTransform.DOAnchorPos(inPosition, duration).SetEase(ease);

        return activeTween;
    }
    public virtual Tweener TransitionOut(float duration)
    {
        if( activeTween != null ) { activeTween.Kill(); }

        rectTransform.anchoredPosition = inPosition;
        activeTween = rectTransform.DOAnchorPos(exitPosition, duration).SetEase(ease);

        return activeTween;
    }
}
