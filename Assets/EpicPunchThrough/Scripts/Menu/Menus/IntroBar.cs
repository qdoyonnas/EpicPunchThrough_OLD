using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBar : Menu
{
    protected override bool OnAnyKey( float value )
    {
        if( base.OnAnyKey(value) ) { return true; }
        if( Mathf.Abs(value) <= 0 || Mathf.Abs(InputManager.Instance.GetInput("cancel")) > 0 ) { return false; }

        TransitionOut( () => {
            Menu mainMenu = MenuManager.Instance.GetMenu("MainMenu");
            if( mainMenu != null ) { mainMenu.TransitionIn(); }
            else { Debug.LogError("IntroBar did not find MainMenu"); }
        });

        return true;
    }
    protected override bool OnCancel( float value )
    {
        if( base.OnCancel(value) ) { return true; }
        if( Mathf.Abs(value) <= 0 ) { return false; }

        Debug.Log("QUIT");
        Application.Quit();

        return true;
    }
}
