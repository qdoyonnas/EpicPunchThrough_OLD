using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControlDecorator : MenuControl
{
    public override void Init()
    {
        // Do not init decorators (MenuControl handles chaining)
    }
}
