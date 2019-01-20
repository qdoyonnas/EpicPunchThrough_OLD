using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputNavigateMenuDecorator : MenuDecorator
{
    public override bool OnUp( bool isDown )
    {
        base.OnUp(isDown);

        if( isDown ) {
            menu.selectedItemCord = new Vector2Int(menu.selectedItemCord[0], menu.selectedItemCord[1]-1);

            return true;
        }

        return false;
    }
    public override bool OnRight( bool isDown )
    {
        base.OnRight(isDown);

        if( isDown ) {
            menu.selectedItemCord = new Vector2Int(menu.selectedItemCord[0]+1, menu.selectedItemCord[1]);

            return true;
        }

        return false;
    }
    public override bool OnDown( bool isDown )
    {
        base.OnDown(isDown);

        if( isDown ) {
            menu.selectedItemCord = new Vector2Int(menu.selectedItemCord[0], menu.selectedItemCord[1]+1);

            return true;
        }

        return false;
    }
    public override bool OnLeft( bool isDown )
    {
        base.OnLeft(isDown);

        if( isDown ) {
            menu.selectedItemCord = new Vector2Int(menu.selectedItemCord[0]-1, menu.selectedItemCord[1]);
        }

        return false;
    }
}
