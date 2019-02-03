using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneLoadControl : MenuControl
{
    public string displayMenuNameOnLoad = string.Empty;

    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleConfirmInput(isDown, menu);

        if( isDown ) {
            MenuManager.Instance.displayMenuNameOnLoad = displayMenuNameOnLoad;
            GameManager.Instance.SetState(GameManager.GameState.menu);
            return true;
        }

        return false;
    }
}
