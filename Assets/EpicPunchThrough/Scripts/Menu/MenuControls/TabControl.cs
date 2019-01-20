using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabControl : MenuControl
{
    public TabControl nextTab;
    public TabControl previousTab;
    public bool isVertical = false;
    public bool autoSwitchOnInput = true;

    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleConfirmInput(isDown, menu);

        if( isDown ) {
            Debug.Log("Switch to " + gameObject.name + " tab");

            return true;
        }

        return false;
    }

    public override bool HandleRightInput( bool isDown, Menu menu )
    {
        base.HandleRightInput(isDown, menu);

        if( !isVertical && nextTab != null && isDown ) {
            menu.selectedItemCord = nextTab.menuItemCord;
            if( autoSwitchOnInput ) {  nextTab.HandleConfirmInput(true, menu); }

            return true;
        }

        return false; 
    }
    public override bool HandleLeftInput( bool isDown, Menu menu )
    {
        base.HandleLeftInput(isDown, menu);

        if( !isVertical && previousTab != null && isDown ) {
            menu.selectedItemCord = previousTab.menuItemCord;
            if( autoSwitchOnInput ) { previousTab.HandleConfirmInput(true, menu); }

            return true;
        }

        return false;
    }

    public override bool HandleUpInput( bool isDown, Menu menu )
    {
        base.HandleUpInput(isDown, menu);

        if( isVertical && previousTab != null && isDown ) {
            menu.selectedItemCord = previousTab.menuItemCord;
            if( autoSwitchOnInput ) {  previousTab.HandleConfirmInput(true, menu); }

            return true;
        }

        return false; 
    }
    public override bool HandleDownInput( bool isDown, Menu menu )
    {
        base.HandleDownInput(isDown, menu);

        if( isVertical && nextTab != null && isDown ) {
            menu.selectedItemCord = nextTab.menuItemCord;
            if( autoSwitchOnInput ) { nextTab.HandleConfirmInput(true, menu); }

            return true;
        }

        return false;
    }
}
