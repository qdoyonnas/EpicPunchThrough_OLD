using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelMenuDecorator : MenuDecorator
{
    public string[] cancelMenuNames; // XXX: Consider making non-array

    public override bool OnCancel( float value )
    {
        base.OnCancel(value);

        if( cancelMenuNames.Length > 0 && Mathf.Abs(value) > 0 ) {
            bool didFail = false;
            Menu[] menus = new Menu[cancelMenuNames.Length];

            for( int i = 0; i < cancelMenuNames.Length; i++ ) {
                Menu loadMenu = MenuManager.Instance.GetMenu(cancelMenuNames[i]);
                if( loadMenu != null ) {
                    menus[i] = loadMenu;
                } else {
                    Debug.LogError("Menu: " + gameObject.name + "could not find menu by name of: " + cancelMenuNames[i]);
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
