﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitControl : MenuControl
{
    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleConfirmInput(isDown, menu);

        if( isDown ) {
            Debug.Log("QUIT");
            Application.Quit();

            return true;
        }

        return false;
    }
}
