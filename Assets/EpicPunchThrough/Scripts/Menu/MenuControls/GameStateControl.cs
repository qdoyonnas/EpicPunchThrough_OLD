using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateControl : MenuControl
{
    public GameManager.GameState state;

    public override bool HandleConfirmInput( float value, Menu menu )
    {
        base.HandleConfirmInput(value, menu);

        if( Mathf.Abs(value) > 0 ) {
            GameManager.Instance.SetState(state);

            return true;
        }

        return false;
    }
}
