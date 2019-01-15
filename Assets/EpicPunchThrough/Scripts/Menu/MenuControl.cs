using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    public virtual void Focused(Menu menu) {}

    public virtual bool HandleAnyInput() { return false; }
    public virtual bool HandleUpInput(bool isDown) { return false; }
    public virtual bool HandleRightInput(bool isDown) { return false; }
    public virtual bool HandleDownInput(bool isDown) { return false; }
    public virtual bool HandleLeftInput(bool isDown) { return false; }
    public virtual bool HandleConfirmInput(bool isDown) { return false; }
    public virtual bool HandleCancelInput(bool isDown) { return false; }
}