using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChangeControl: MenuControl
{
    public string[] menuNames; // XXX: Should consider whether or not this *should* be an array or just a single value. Similar issue with initial menus and cancel-into menus.
    public bool additive = false;
    public bool setCancelBack = true;

    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleConfirmInput(isDown, menu);

        if( menuNames.Length > 0 && isDown ) {

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
                if( additive ) {
                    menu.inFocus = false;
                    foreach( Menu loadMenu in menus ) {
                        loadMenu.TransitionIn();

                        if( setCancelBack ) {
                            CancelMenuDecorator cancelDecor;
                            if( cancelDecor = loadMenu.GetComponent<CancelMenuDecorator>() ) {
                                cancelDecor.cancelMenuNames = new string[1] { menu.name };
                            }
                        }
                    }
                } else {
                    menu.TransitionOut( () => {
                        foreach( Menu loadMenu in menus ) {
                            loadMenu.TransitionIn();

                            if( setCancelBack ) {
                                CancelMenuDecorator cancelDecor;
                                if( cancelDecor = loadMenu.GetComponent<CancelMenuDecorator>() ) {
                                    cancelDecor.cancelMenuNames = new string[1] { menu.name };
                                }
                            }
                        }
                    });
                }
            }

            return true;
        }

        return false;
    }
}
