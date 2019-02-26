using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundControlDecorator : MenuControlDecorator
{
    public AudioClip soundOnFocus;
    public AudioClip soundOnUnfocus;
    public AudioClip soundOnAny;
    public AudioClip soundOnHorizontal;
    public AudioClip soundOnVertical;
    public AudioClip soundOnConfirm;
    public AudioClip soundOnCancel;

    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    private void OnDestroy()
    {
        SoundManager.Instance.UnregisterSound(source);
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
    public override bool HandleHorizontal( float value, Menu menu )
    {
        base.HandleHorizontal(value, menu);

        if( Mathf.Abs(value) > 0 && soundOnHorizontal != null ) {
            source.clip = soundOnHorizontal;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    public override bool HandleVertical( float value, Menu menu )
    {
        base.HandleVertical(value, menu);

        if( Mathf.Abs(value) > 0 && soundOnVertical != null ) {
            source.clip = soundOnVertical;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    
    public override bool HandleConfirmInput( float value, Menu menu )
    {
        base.HandleConfirmInput(value, menu);

        if( Mathf.Abs(value) > 0 && soundOnConfirm != null ) {
            source.clip = soundOnConfirm;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
    public override bool HandleCancelInput( float value, Menu menu )
    {
        base.HandleCancelInput(value, menu);

        if( Mathf.Abs(value) > 0 && soundOnCancel != null ) {
            source.clip = soundOnCancel;
            SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.menu, 0, source);
        }

        return false;
    }
}
