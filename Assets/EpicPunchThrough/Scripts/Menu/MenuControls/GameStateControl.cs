using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateControl : MenuControl
{
    public GameManager.GameState state;

    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleConfirmInput(isDown, menu);

        if( isDown ) {
            GameManager.Instance.SetState(state);

            return true;
        }

        return false;
    }
}
