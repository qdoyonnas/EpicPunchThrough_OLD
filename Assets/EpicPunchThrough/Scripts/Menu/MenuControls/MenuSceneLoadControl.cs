using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneLoadControl : MenuControl
{
    public string displayMenuNameOnLoad = string.Empty;

    public override bool HandleConfirmInput( float value, Menu menu )
    {
        base.HandleConfirmInput(value, menu);

        if( Mathf.Abs(value) > 0 ) {
            MenuManager.Instance.displayMenuNameOnLoad = displayMenuNameOnLoad;
            GameManager.Instance.SetState(GameManager.GameState.menu);
            return true;
        }

        return false;
    }
}
