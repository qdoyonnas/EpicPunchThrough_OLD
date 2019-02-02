using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    #region Settings

    [Serializable]
    public struct SoundManagerSettings
    {
        public bool musicOn;
        public bool soundOn;
    }

    #endregion

    #region Static

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get {
            if( instance == null ) {
                instance = new SoundManager();
                instance.Initialize(new SoundManagerSettings());
            }
            return instance;
        }
    }

    #endregion

    public enum SoundLayer {
        backgroundMusic,
        foregroundMusic,
        menu,
        game,
        layerCount
    }

    public SoundManagerSettings settings;

    List<AudioSource> sources = new List<AudioSource>();

    private bool isInitialized = false;
    public bool Initialize(SoundManagerSettings settings)
    {
        this.settings = settings;
        
        GameManager.Instance.updated += DoUpdate;

        isInitialized = true;
        return isInitialized;
    }

    public void RegisterSound(SoundLayer layer, int priority, AudioSource source)
    {
        // XXX: Layers and priorities not implemented

        switch( layer ) {
            case SoundLayer.backgroundMusic:
            case SoundLayer.foregroundMusic:
                if( !settings.musicOn ) { return; }
                break;
            default:
                if( !settings.soundOn ) { return; }
                break;
        }

        sources.Add(source);
        source.Play();
    }
    public void UnregisterSound(AudioSource source)
    {
        if( sources.Contains(source) ) {
            source.Stop();
            sources.Remove(source);
        }
    }

    void DoUpdate(GameManager.UpdateData data)
    {
        for( int i = sources.Count-1; i >= 0; i-- ) {
            if( sources[i] == null
                || !sources[i].isPlaying ) 
            {
                sources.RemoveAt(i);
            }
        }
    }
}
