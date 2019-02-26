using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartControl : MenuControl
{
    public override bool HandleConfirmInput( float value, Menu menu )
    {
        if( Mathf.Abs(value) > 0 ) {
            GameManager.Instance.SetState(GameManager.GameState.play);

            return true;
        }

        return false;
    }
}
