using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    protected override bool OnCancel( bool isDown )
    {
        if( base.OnCancel(isDown) ) { return true; }

        TransitionOut(() => {
            Menu introBar = MenuManager.Instance.GetMenu("IntroBar");
            if( introBar != null ) { introBar.TransitionIn(); }
            else { Debug.LogError("MainMenu did not find IntroBar"); }
        });
        return true;
    }
}
