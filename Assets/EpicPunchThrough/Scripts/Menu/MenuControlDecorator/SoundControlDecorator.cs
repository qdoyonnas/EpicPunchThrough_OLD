using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundControlDecorator : MenuControlDecorator
{
    public AudioClip soundOnFocus;
    public AudioClip soundOnUnfocus;
    public AudioClip soundOnAny;
    public AudioClip soundOnUp;
    public AudioClip soundOnRight;
    public AudioClip soundOnDown;
    public AudioClip soundOnLeft;
    public AudioClip soundOnConfirm;
    public AudioClip soundOnCancel;

    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public override void Focused( Menu menu )
    {
        base.Focused(menu);

        if( soundOnFocus != null ) {
            source.clip = soundOnFocus;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }
    }
    public override void UnFocused( Menu menu )
    {
        base.UnFocused(menu);

        if( soundOnUnfocus != null ) {
            source.clip = soundOnUnfocus;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }
    }

    public override bool HandleAnyInput( Menu menu )
    {
        base.HandleAnyInput(menu);

        if( soundOnAny != null ) {
            source.clip = soundOnAny;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    public override bool HandleUpInput( bool isDown, Menu menu )
    {
        base.HandleAnyInput(menu);

        if( isDown && soundOnUp != null ) {
            source.clip = soundOnUp;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    public override bool HandleRightInput( bool isDown, Menu menu )
    {
        base.HandleAnyInput(menu);

        if( isDown && soundOnRight != null ) {
            source.clip = soundOnRight;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    public override bool HandleDownInput( bool isDown, Menu menu )
    {
        base.HandleAnyInput(menu);

        if( isDown && soundOnDown != null ) {
            source.clip = soundOnDown;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    public override bool HandleLeftInput( bool isDown, Menu menu )
    {
        base.HandleAnyInput(menu);

        if( isDown && soundOnLeft != null ) {
            source.clip = soundOnLeft;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    public override bool HandleConfirmInput( bool isDown, Menu menu )
    {
        base.HandleAnyInput(menu);

        if( isDown && soundOnConfirm != null ) {
            source.clip = soundOnConfirm;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    public override bool HandleCancelInput( bool isDown, Menu menu )
    {
        base.HandleAnyInput(menu);

        if( isDown && soundOnCancel != null ) {
            source.clip = soundOnCancel;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
}
