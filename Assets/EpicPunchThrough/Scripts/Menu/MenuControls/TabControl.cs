using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabControl : MenuControl
{
    public TabControl nextTab;
    public TabControl previousTab;
    public bool isVertical = false;
    public bool autoSwitchOnInput = true;

    public override bool HandleConfirmInput( float value, Menu menu )
    {
        base.HandleConfirmInput(value, menu);

        if( Mathf.Abs(value) > 0 ) {
            Debug.Log("Switch to " + gameObject.name + " tab");

            return true;
        }

        return false;
    }

    public override bool HandleHorizontal( float value, Menu menu )
    {
        base.HandleHorizontal(value, menu);

        if( !isVertical && Mathf.Abs(value) > 0 ) {
            TabControl selectedTab = null;
            if( value > 0 ) {
                selectedTab = nextTab;
            } else {
                selectedTab = previousTab;
            }

            menu.selectedItemCord = selectedTab.menuItemCord;
            if( autoSwitchOnInput ) {  selectedTab.HandleConfirmInput(1, menu); }

            return true;
        }

        return false; 
    }
    public override bool HandleVertical( float value, Menu menu )
    {
        base.HandleVertical(value, menu);

        if( isVertical  && Mathf.Abs(value) > 0 ) {
            TabControl selectedTab = null;
            if( value > 0 ) {
                selectedTab = previousTab;
            } else {
                selectedTab = nextTab;
            }
            if( selectedTab == null ) { return false; }

            menu.selectedItemCord = selectedTab.menuItemCord;
            if( autoSwitchOnInput ) {  selectedTab.HandleConfirmInput(1, menu); }

            return true;
        }

        return false; 
    }
}
