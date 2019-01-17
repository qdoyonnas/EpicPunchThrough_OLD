using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipControlDecorator : MenuControlDecorator
{
    public GameObject tooltip;

    public override void Focused( Menu menu )
    {
        base.Focused(menu);

        tooltip.SetActive(true);
    }
    public override void UnFocused( Menu menu )
    {
        base.UnFocused(menu);

        tooltip.SetActive(false);
    }
}
