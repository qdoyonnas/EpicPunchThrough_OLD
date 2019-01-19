using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControlDecorator : MenuControl
{
    public override void Init(int[] cord)
    {
        // Do not init decorators (MenuControl handles chaining)
    }
}
