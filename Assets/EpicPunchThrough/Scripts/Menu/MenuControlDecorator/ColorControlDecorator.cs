using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class ColorControlDecorator : MenuControlDecorator
{
    public Color unfocusedColor;
    public Color focusedColor;
    public Color selectedColor;
    public float transitionTime = 0.2f;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public override void Focused( Menu menu )
    {
        base.Focused(menu);

        image.DOColor(focusedColor, transitionTime);
    }
    public override void UnFocused( Menu menu )
    {
        base.UnFocused(menu);

        image.DOColor(unfocusedColor, transitionTime);
    }

    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleConfirmInput(isDown, menu);

        image.DOColor(selectedColor, transitionTime);

        return false;
    }
    public override bool HandleCancelInput( bool isDown, Menu menu )
    {
        base.HandleCancelInput(isDown, menu);

        image.DOColor(selectedColor, transitionTime);

        return false;
    }
}
