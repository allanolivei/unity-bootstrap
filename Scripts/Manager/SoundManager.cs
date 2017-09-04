// Copyright (c) 2017-2018 Allan Oliveira Marinho(allanolivei@gmail.com), Inc. All Rights Reserved. 

using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager> {

    public enum SoundType
    {
        UI_BACKGROUND,
        GAME_BACKGROUND,
        UI_EFFECT,
        GAME_EFFECT
    }



    public AudioMixer masterMixer;

    protected AudioMixerSnapshot pause;
    protected AudioMixerSnapshot unpause;

    //public Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    //protected AudioSource source;

    protected List<AudioSource> sources = new List<AudioSource>();
    protected Dictionary<SoundType, AudioMixerGroup> mixerByType = new Dictionary<SoundType, AudioMixerGroup>();

    protected bool isLoaded = false;

    const string musicKey = "MusicVolume";
    const string fxKey = "SFXVolume";


    public AudioSource Play( AudioClip clip, SoundType soundType, float delay = 0.0f )
    {
        return Play(clip, soundType, soundType.Equals(SoundType.UI_BACKGROUND) || soundType.Equals(SoundType.GAME_BACKGROUND), delay);
    }

    public AudioSource Play( AudioClip clip, SoundType soundType, bool withLoop, float delay = 0.0f)
    {
        AudioSource a = GetSourceByPooling();
        a.clip = clip;
        a.loop = withLoop;
        a.playOnAwake = false;
        a.outputAudioMixerGroup = mixerByType[soundType];
        a.volume = 1;
        a.PlayDelayed(delay);
        
        return a;
    }

    public AudioSource GetSourceByClip( AudioClip clip )
    {
        foreach (AudioSource a in sources)
            if (a.isPlaying && a.clip == clip) return a;
        return null;
    }

    public AudioSource GetSourceBySoundType(SoundType soundType)
    {
        AudioMixerGroup mixer = mixerByType[soundType];

        foreach (AudioSource a in sources)
            if (a.outputAudioMixerGroup == mixer && a.isPlaying) return a;

        return default(AudioSource);
    }

    public AudioMixerGroup GetMixerByType( SoundType soundType )
    {
        return mixerByType[soundType];
    }

    public bool HasClip( AudioClip clip )
    {
        return GetSourceByClip(clip) != null;
    }

    public void StopByType( SoundType soundType )
    {
        AudioMixerGroup mixer = mixerByType[soundType];
        
        foreach (AudioSource a in sources)
            if (a.outputAudioMixerGroup == mixer) a.Stop();
    }

    public void StopAll()
    {
        foreach (AudioSource a in sources) a.Stop();
    }

    public void PauseGame()
    {
        if (pause == null) pause = masterMixer.FindSnapshot("pause");
        pause.TransitionTo(.01f);
    }

    public void PlayGame()
    {
        if (unpause == null) unpause = masterMixer.FindSnapshot("unpause");
        unpause.TransitionTo(.01f);
    }

    public bool ToggleMusicVolume()
    {
        bool current = this.HasMusicVolume();
        this.ToggleMusicVolume(!current);
        return !current;
    }

    public void ToggleMusicVolume(bool value)
    {
        //Debug.Log("TOGGLE MUSIC VOLUME: "+value);
        PlayerPrefs.SetFloat(musicKey, value ? 0 : -80);
        masterMixer.SetFloat(musicKey, value ? 0 : -80);
    }

    public bool ToggleSFXVolume()
    {
        bool current = this.HasSFXVolume();
        this.ToggleSFXVolume(!current);
        return !current;
    }

    public void ToggleSFXVolume( bool value )
    {
        //Debug.Log("TOGGLE SFX VOLUME: " + value);
        PlayerPrefs.SetFloat(fxKey, value ? 0 : -80);
        masterMixer.SetFloat(fxKey, value ? 0 : -80);
    }

    public bool HasMusicVolume()
    {
        float volume;
        if ( !isLoaded ) volume = PlayerPrefs.GetFloat(musicKey, 0);
        else masterMixer.GetFloat(musicKey, out volume);
        return volume >= -40;
    }

    public bool HasSFXVolume()
    {
        float volume;
        if( !isLoaded ) volume = PlayerPrefs.GetFloat(fxKey, 0);
        else masterMixer.GetFloat(fxKey, out volume);
        return volume >= -40;
    }

    protected override void Awake()
    {
        base.Awake();



        mixerByType.Add(SoundType.UI_BACKGROUND, masterMixer.FindMatchingGroups("UIMusic")[0]);//uimixer);
        mixerByType.Add(SoundType.GAME_BACKGROUND, masterMixer.FindMatchingGroups("UIMusic/GameMusic")[0]);//musicmixer);
        mixerByType.Add(SoundType.UI_EFFECT, masterMixer.FindMatchingGroups("UIFX")[0]);//uimixer);
        mixerByType.Add(SoundType.GAME_EFFECT, masterMixer.FindMatchingGroups("UIFX/GameFX")[0]);// fxmixer);

        sources.AddRange(gameObject.GetComponents<AudioSource>());
    }

    protected override void SceneUnload( Scene scene ){}

    public void Start()
    {
        masterMixer.SetFloat(fxKey, PlayerPrefs.GetFloat(fxKey, 0));
        masterMixer.SetFloat(musicKey, PlayerPrefs.GetFloat(musicKey, 0));
        //masterMixer.SetFloat(musicKey, -80);
        isLoaded = true;
    }

    

    private AudioSource GetSourceByPooling()
    {
        for( int i = 0; i < sources.Count; i++ )
            if (!sources[i].isPlaying) return sources[i];

        AudioSource a = gameObject.AddComponent<AudioSource>();
        sources.Add(a);
        return a;
    }
    


    /*
    public void Play(string type, AudioClip clip)
    {
        if (source == null) source = gameObject.AddComponent<AudioSource>();

        if (source.clip == clip) return;

        //sounds.Add(type, clip);
        source.clip = clip;
        source.loop = true;
        source.Play();
    }
    */

}

