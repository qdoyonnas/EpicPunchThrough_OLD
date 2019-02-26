using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitControl : MenuControl
{
    public override bool HandleConfirmInput( float value, Menu menu )
    {
        base.HandleConfirmInput(value, menu);

        if( Mathf.Abs(value) > 0 ) {
            Debug.Log("QUIT");
            GameManager.Instance.SetState(GameManager.GameState.exit);
            Application.Quit();

            return true;
        }

        return false;
    }
}
