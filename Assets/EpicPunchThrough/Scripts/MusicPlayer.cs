using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public AudioClip song;
    public bool onStart = true;

    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();

        if( onStart ) {
            PlaySong();
        }
    }

    public void PlaySong()
    {
        if( source == null ) { return; }

        source.clip = song;
        SoundManager.Instance.RegisterSound(SoundManager.SoundLayer.backgroundMusic, 0, source);
    }
}
