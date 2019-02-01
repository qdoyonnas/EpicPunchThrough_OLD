using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartControl : MenuControl
{
    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        if( isDown ) {
            GameManager.Instance.SetState(GameManager.GameState.play);

            return true;
        }

        return false;
    }
}
