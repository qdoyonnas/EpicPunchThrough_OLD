using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorControlDecorator : MenuControlDecorator
{
    [Tooltip("Set size to that of Rect Transform - Overrides manual setting of size")]
    public bool autoSize;
    public Vector2 size;
    public float selectorTime = 0.2f;

    RectTransform rectTransform;

    public override void Init()
    {
        rectTransform = GetComponent<RectTransform>();

        if( autoSize ) {
            size = rectTransform.sizeDelta;
        }
    }

    public override void Focused( Menu menu )
    {
        base.Focused(menu);

        menu.DisplaySelector(true);
        if( size == null || size == Vector2.zero ) {
            menu.MoveSelector(transform.position, size, selectorTime);
        }  else {
            menu.MoveSelector(transform.position, selectorTime);
        }
    }
    public override void UnFocused( Menu menu )
    {
        base.UnFocused(menu);

        menu.DisplaySelector(false);
    }
}
