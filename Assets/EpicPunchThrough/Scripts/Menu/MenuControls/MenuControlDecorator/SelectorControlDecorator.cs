using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorControlDecorator : MenuControlDecorator
{
    [Tooltip("Set size to that of Rect Transform - Overrides manual setting of size")]
    public bool autoSize;
    public Vector2 size;
    public Color focusedColor;
    public Color selectedColor;
    public float transitionTime = 0.2f;

    RectTransform rectTransform;

    public override void Init(Vector2Int cord)
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
        if( size != null && size != Vector2.zero ) {
            menu.MoveSelector(transform.position, size, transitionTime);
        }  else {
            menu.MoveSelector(transform.position, transitionTime);
        }
        menu.SelectorColor(focusedColor, transitionTime);
    }
    public override void UnFocused( Menu menu )
    {
        base.UnFocused(menu);

        menu.DisplaySelector(false);
    }

    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleConfirmInput(isDown, menu);

        menu.SelectorColor(selectedColor, transitionTime);

        return false;
    }
}
