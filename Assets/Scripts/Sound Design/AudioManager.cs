using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    private Sound nowPlayingBackground = null;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        CreateAudioSources();
    }

    public void PlayEffects(string name)
    {
        PlayEffects(name, false);
    }

    public void PlayEffects(string name, bool turnDownBackground)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name && sound.soundType == Sound.SoundType.Effects);

        if (s == null)
        {
            return;
        }

        RandomizeClip(s);
        PlaySoundSource(s);

        if (turnDownBackground)
        {
            StartCoroutine(TurnDownBackgroundWhilePlaying(s));
        }
    }

    public void PlayBackground(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name && sound.soundType == Sound.SoundType.Background);

        if (s == null)
        {
            return;
        }

        Sound[] otherBackgrounds = Array.FindAll(sounds, sound => sound.name != name && sound.soundType == Sound.SoundType.Background);

        foreach (Sound o in otherBackgrounds)
        {
            o.source.Stop();
        }

        RandomizeClip(s);
        PlaySoundSource(s);
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            return;
        }

        s.source.Stop();
    }

    public void StopAllSounds()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }

    private void CreateAudioSources()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        }
    }

    private void PlaySoundSource(Sound s)
    {
        if (s.loop && s.source.isPlaying)
        {
            return;
        }

        s.source.Play();

        if (s.soundType == Sound.SoundType.Background)
        {
            nowPlayingBackground = s;
        }
    }

    private void SetProperties(Sound s)
    {
        s.source.volume = s.volume;
        s.source.pitch = s.pitch
                            + (s.pitchDeviation * 2f * UnityEngine.Random.value - s.pitchDeviation);
        s.source.loop = s.loop;
    }

    private void RandomizeClip(Sound s)
    {
        s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        SetProperties(s);
    }

    private IEnumerator TurnDownBackgroundWhilePlaying(Sound s)
    {
        Sound prevPlayingBackground = nowPlayingBackground;

        float originalVolume;
        const float turnDownMultiplier = 0.3f;

        originalVolume = nowPlayingBackground.volume;
        nowPlayingBackground.source.volume = originalVolume * turnDownMultiplier;

        while (s.source.isPlaying)
        {
            if (nowPlayingBackground.name != prevPlayingBackground.name)
            {
                prevPlayingBackground = nowPlayingBackground;

                originalVolume = nowPlayingBackground.volume;
                nowPlayingBackground.source.volume = originalVolume * turnDownMultiplier;
            }

            yield return null;
        }

        nowPlayingBackground.source.volume = originalVolume;
    }
}
