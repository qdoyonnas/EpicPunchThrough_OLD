using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBar : Menu
{
    protected override bool OnAnyKey( bool isDown )
    {
        if( base.OnAnyKey(isDown) ) { return true; }
        if( !isDown || InputManager.Instance.GetInput("cancel") ) { return false; }

        TransitionOut( () => {
            Menu mainMenu = MenuManager.Instance.GetMenu("MainMenu");
            if( mainMenu != null ) { mainMenu.TransitionIn(); }
            else { Debug.LogError("IntroBar did not find MainMenu"); }
        });

        return true;
    }
    protected override bool OnCancel( bool isDown )
    {
        if( base.OnCancel(isDown) ) { return true; }
        if( !isDown ) { return false; }

        Debug.Log("QUIT");
        Application.Quit();

        return true;
    }
}
