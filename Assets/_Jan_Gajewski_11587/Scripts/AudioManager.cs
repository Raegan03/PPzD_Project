using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioState
{
    Casual = 0,
    Fight = 1,
    UI = 2,
    Silent = 3,
    End = 4,
}

public class AudioManager : MonoBehaviour
{
    public AudioState AudioState { get; private set; }
    
    public static AudioManager Instance;

    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private List<AudioMixerGroupBind> audioMixerGroupBinds;
    [Space]
    [SerializeField] private AudioMixerSnapshot casualSnapshot;
    [SerializeField] private AudioMixerSnapshot fightSnapshot;
    [SerializeField] private AudioMixerSnapshot uiSnapshot;
    [SerializeField] private AudioMixerSnapshot silentSnapshot;
    [SerializeField] private AudioMixerSnapshot endSnapshot;
    [Space]
    [SerializeField] private AudioClip _victoryClip;
    [SerializeField] private AudioClip _loseClip;
    [SerializeField] private AudioClip _openDoors;
    [SerializeField] private AudioClip[] _hitClips;
    [SerializeField] private AudioClip[] _dieClips;
    
    [Header("UI Clips")]
    [SerializeField] private AudioClip _hoverButton;
    [SerializeField] private AudioClip _clickButton;

    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        
        Instance = this;
        
        transform.SetParent(null);
        DontDestroyOnLoad(this);
    }

    public void ChangeAudioState(AudioState audioState, float transition)
    {
        switch (audioState)
        {
            case AudioState.Casual:
                casualSnapshot.TransitionTo(transition);
                break;
            case AudioState.Fight:
                fightSnapshot.TransitionTo(transition);
                break;
            case AudioState.UI:
                uiSnapshot.TransitionTo(transition);
                break;
            case AudioState.Silent:
                silentSnapshot.TransitionTo(transition);
                break;
            case AudioState.End:
                endSnapshot.TransitionTo(transition);
                break;
        }

        AudioState = audioState;
    }
    
    public void PlayHoverUIClip()
    {
        Create2DSFX(_hoverButton, SFXAudioGroups.UI, 0f, .75f);
    }
    
    public void PlayClickUIClip()
    {
        Create2DSFX(_clickButton, SFXAudioGroups.UI, 0f, .75f);
    }
    
    public void PlayVictoryClip(float delay = 0f)
    {
        Create2DSFX(_victoryClip, SFXAudioGroups.Main, delay, .5f);
    }
    
    public void PlayLoseClip(float delay = 0f)
    {
        Create2DSFX(_loseClip, SFXAudioGroups.Main, delay);
    }
    
    public void PlayOpenDoorsClip(Vector3 position)
    {
        CreateSFX(_openDoors, position, 
            SFXAudioGroups.Main, 0f);
    }

    public void PlayRandomHitClip(Vector3 position)
    {
        CreateSFX(_hitClips.GetRandomItem(), position, 
            SFXAudioGroups.Hit, 0f, .5f);
    }
    
    public void PlayRandomDieClip(Vector3 position)
    {
        CreateSFX(_dieClips.GetRandomItem(), position, 
            SFXAudioGroups.Hit, 0f, .75f);
    }

    public void CreateSFX(AudioClip clip, Vector3 position, SFXAudioGroups sfxAudioGroup, float spatialBlend, float volume = 1f, float rolloffDistanceMin = 1f)
    {
        GameObject impactSFXInstance = new GameObject();
        impactSFXInstance.transform.SetParent(transform);
        
        impactSFXInstance.transform.position = position;
        AudioSource source = impactSFXInstance.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        source.minDistance = rolloffDistanceMin;
        source.Play();

        source.outputAudioMixerGroup = GetAudioGroup(sfxAudioGroup);

        TimedSelfDestruct timedSelfDestruct = impactSFXInstance.AddComponent<TimedSelfDestruct>();
        timedSelfDestruct.lifeTime = clip.length;
    }
    
    public void CreateLoopSFX(AudioClip clip, Vector3 position, SFXAudioGroups sfxAudioGroup, float spatialBlend, float loopTimes = 2, float volume = 1f, float rolloffDistanceMin = 1f)
    {
        var impactSFXInstance = new GameObject();
        impactSFXInstance.transform.SetParent(transform);
        
        impactSFXInstance.transform.position = position;
        var source = impactSFXInstance.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.volume = volume;
        source.spatialBlend = spatialBlend;
        source.minDistance = rolloffDistanceMin;
        source.Play();

        source.outputAudioMixerGroup = GetAudioGroup(sfxAudioGroup);

        TimedSelfDestruct timedSelfDestruct = impactSFXInstance.AddComponent<TimedSelfDestruct>();
        timedSelfDestruct.lifeTime = clip.length * loopTimes;
    }
    
    public void Create2DSFX(AudioClip clip, SFXAudioGroups sfxAudioGroup, float delay = 0f, float volume = 1f)
    {
        GameObject impactSFXInstance = new GameObject();
        impactSFXInstance.transform.SetParent(transform);

        AudioSource source = impactSFXInstance.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0f;
        source.volume = volume;
        source.PlayScheduled(AudioSettings.dspTime + delay);

        source.outputAudioMixerGroup = GetAudioGroup(sfxAudioGroup);

        TimedSelfDestruct timedSelfDestruct = impactSFXInstance.AddComponent<TimedSelfDestruct>();
        timedSelfDestruct.lifeTime = clip.length;
    }

    public void SetParamOnMixer(string paramName, float targetVolume)
    {
        var targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        mainMixer.SetFloat(paramName, Mathf.Log10(targetValue) * 20);
    }
    
    public float GetParamFromMixer(string paramName)
    {
        mainMixer.GetFloat(paramName, out var currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);

        return currentVol;
    }

    public AudioMixerGroup GetAudioGroup(SFXAudioGroups group)
    {
        var audioMixerGroup = audioMixerGroupBinds
            .Find(x => x.bindGroup == group)
            .audioMixerGroup;
        
        return audioMixerGroup;
    }
}

[Serializable]
public class AudioMixerGroupBind
{
    public SFXAudioGroups bindGroup;
    public AudioMixerGroup audioMixerGroup;
}
