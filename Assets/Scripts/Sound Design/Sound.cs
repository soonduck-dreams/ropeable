using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public enum SoundType
    {
        Background,
        Effects
    }

    public SoundType soundType;

    public string name;
    public AudioClip[] clips;
    public bool loop;

    [Range(0f, 1.2f)] public float volume;
    [Range(.1f, 3f)] public float pitch;
    public float pitchDeviation = 0f;

    [HideInInspector]
    public AudioSource source;
}
