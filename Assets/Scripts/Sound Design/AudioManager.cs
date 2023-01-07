using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

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

    private void CreateAudioSources()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            return;
        }

        RandomizeClip(s);
        PlaySoundSource(s);
    }

    public void PlayBackgroundExclusively(string name)
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

    private void PlaySoundSource(Sound s)
    {
        if (s.loop && s.source.isPlaying)
        {
            return;
        }

        s.source.Play();
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

    private void CookSound(Sound s)
    {
        s.source.volume = s.volume;
        s.source.pitch = s.pitch
                            + (s.pitchDeviation * 2f * UnityEngine.Random.value - s.pitchDeviation);
        s.source.loop = s.loop;
    }

    private void RandomizeClip(Sound s)
    {
        s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];
        CookSound(s);
    }
}
