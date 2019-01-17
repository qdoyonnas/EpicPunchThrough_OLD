using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChangeControl: MenuControl
{
    public string[] menuNames;

    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleConfirmInput(isDown, menu);

        if( isDown ) {

            bool didFail = false;
            Menu[] menus = new Menu[menuNames.Length];

            for( int i = 0; i < menuNames.Length; i++ ) {
                Menu loadMenu = MenuManager.Instance.GetMenu(menuNames[i]);
                if( loadMenu != null ) {
                    menus[i] = loadMenu;
                } else {
                    Debug.LogError("MenuChangeControl: " + gameObject.name + "could not find menu by name of: " + menuNames[i]);
                    didFail = true;
                }
            }

            if( !didFail ) {
                menu.TransitionOut( () => {
                    foreach( Menu loadMenu in menus ) {
                        loadMenu.TransitionIn();
                    }
                });
            }

            return true;
        }

        return false;
    }
}
