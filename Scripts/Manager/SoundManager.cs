using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    /*
     * Instruções de uso:
     * 1) Crie um AudioMixer no seu projeto com pelo menos 2 grupos (sugestão: SFX e Music)
     * 2) Exponha (expose) o volume dos dois grupos criados e os renomeie para SFXVolume e MusicVolume
     * 3) Crie um objeto para ser o SoundManager e acople este script nele
     * 4) Adicione todos os grupos e snapshots criados no componente deste script no objeto criado no passo anterior
     * 5) Para usar os métodos do SoundManager, use SoundManager.GetInstance().MÉTODO();
     */

    public AudioMixer masterMixer;
    [Space]
    public AudioGroup[] audioMixerGroups;
    [Space]
    public string[] snapshotNames;

    protected AudioMixerSnapshot pause;
    protected AudioMixerSnapshot unpause;

    protected List<AudioSource> sources = new List<AudioSource>();
    protected Dictionary<string, AudioMixerGroup> myMixerGroups = new Dictionary<string, AudioMixerGroup>();
    protected AudioMixerSnapshot[] snapshots;

    protected bool isLoaded = false;

    const string musicKey = "MusicVolume";
    const string fxKey = "SFXVolume";

    public AudioSource Play(AudioClip clip, string mixerGroupName, bool withLoop, float delay)
    {
        AudioSource a = GetSourceByPooling();
        a.clip = clip;
        a.loop = withLoop;
        a.playOnAwake = false;
        a.outputAudioMixerGroup = myMixerGroups[mixerGroupName];
        a.volume = 1;
        a.PlayDelayed(delay);

        return a;
    }

    public AudioSource Play(AudioClip clip, string mixerGroupName)
    {
        return Play(clip, mixerGroupName, false, 0);
    }

    public AudioSource GetSourceByClip(AudioClip clip)
    {
        foreach (AudioSource a in sources)
            if (a.isPlaying && a.clip == clip) return a;
        return null;
    }

    public AudioSource GetSourceBySoundType(string mixerGroupName)
    {
        AudioMixerGroup mixer = myMixerGroups[mixerGroupName];

        foreach (AudioSource a in sources)
            if (a.outputAudioMixerGroup == mixer && a.isPlaying) return a;

        return default(AudioSource);
    }

    public AudioMixerGroup GetMixerByType(string mixerGroupName)
    {
        return myMixerGroups[mixerGroupName];
    }

    public bool HasClip(AudioClip clip)
    {
        return GetSourceByClip(clip) != null;
    }

    public void StopByType(string mixerGroupName)
    {
        AudioMixerGroup mixer = myMixerGroups[mixerGroupName];

        foreach (AudioSource a in sources)
            if (a.outputAudioMixerGroup == mixer) a.Stop();
    }

    public void StopAll()
    {
        foreach (AudioSource a in sources) a.Stop();
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

    public void ToggleSFXVolume(bool value)
    {
        //Debug.Log("TOGGLE SFX VOLUME: " + value);
        PlayerPrefs.SetFloat(fxKey, value ? 0 : -80);
        masterMixer.SetFloat(fxKey, value ? 0 : -80);
    }

    public bool HasMusicVolume()
    {
        float volume;
        if (!isLoaded) volume = PlayerPrefs.GetFloat(musicKey, 0);
        else masterMixer.GetFloat(musicKey, out volume);
        return volume >= -40;
    }

    public bool HasSFXVolume()
    {
        float volume;
        if (!isLoaded) volume = PlayerPrefs.GetFloat(fxKey, 0);
        else masterMixer.GetFloat(fxKey, out volume);
        return volume >= -40;
    }

    public void SnapshotTransition(string targetSnapshot, float transitionDuration)
    {
        GetSnapshot(targetSnapshot).TransitionTo(transitionDuration);
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioGroups();
        InitializeSnapshots();
        sources.AddRange(gameObject.GetComponents<AudioSource>());
    }

    protected void InitializeAudioGroups()
    {
        foreach (var item in audioMixerGroups)
        {
            myMixerGroups.Add(item.name, masterMixer.FindMatchingGroups(item.path)[0]);
        }
    }

    protected void InitializeSnapshots()
    {
        snapshots = new AudioMixerSnapshot[snapshotNames.Length];
        for (int i = 0; i < snapshotNames.Length; i++)
        {
            snapshots[i] = masterMixer.FindSnapshot(snapshotNames[i]);
        }
    }

    protected AudioMixerSnapshot GetSnapshot(string name)
    {
        for (int i = 0; i < snapshotNames.Length; i++)
        {
            if (snapshotNames[i] == name)
            {
                return snapshots[i];
            }
        }
        Debug.LogWarning("Nome de Snapshot inválido!");
        return null;
    }

    protected override void SceneUnload(Scene scene) { }

    public void Start()
    {
        masterMixer.SetFloat(fxKey, PlayerPrefs.GetFloat(fxKey, 0));
        masterMixer.SetFloat(musicKey, PlayerPrefs.GetFloat(musicKey, 0));
        isLoaded = true;
    }

    private AudioSource GetSourceByPooling()
    {
        for (int i = 0; i < sources.Count; i++)
            if (!sources[i].isPlaying) return sources[i];

        AudioSource a = gameObject.AddComponent<AudioSource>();
        sources.Add(a);
        return a;
    }

}

