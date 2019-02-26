using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputNavigateMenuDecorator : MenuDecorator
{
    public override bool OnVertical( float value )
    {
        base.OnVertical(value);

        if( Mathf.Abs(value) > 0 ) {
            int direction = 0;
            if( value > 0 ) {
                direction = -1;
            } else {
                direction = 1;
            }
            menu.selectedItemCord = new Vector2Int(menu.selectedItemCord[0], menu.selectedItemCord[1] + direction);

            return true;
        }

        return false;
    }
    public override bool OnHorizontal( float value )
    {
        base.OnHorizontal(value);

        if( Mathf.Abs(value) > 0 ) {
            int direction = 0;
            if( value > 0 ) {
                direction = 1;
            } else {
                direction = -1;
            }
            menu.selectedItemCord = new Vector2Int(menu.selectedItemCord[0] + direction, menu.selectedItemCord[1]);

            return true;
        }

        return false;
    }
}
