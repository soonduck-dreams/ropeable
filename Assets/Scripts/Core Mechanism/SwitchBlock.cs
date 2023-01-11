using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GuardBlock;

public class SwitchBlock : MonoBehaviour, IRopeable, ISpeaker
{
    private List<IListener> listeners;

    private void Awake()
    {
        listeners = new List<IListener>();
    }

    private void Start()
    {
        Darken();
    }

    public string GetTerrainType()
    {
        return "switch";
    }

    public void OnRopeHang()
    {
        SpeakToListeners();
        Lighten();
    }

    public void OnRopeCut()
    {
        Darken();
    }

    public void AddListener(IListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(IListener listener)
    {
        listeners.Remove(listener);
    }

    public void SpeakToListeners()
    {
        foreach (IListener listener in listeners)
        {
            listener.OnListen();
        }
    }

    private void Lighten()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;

        color.a = 1f;
        sr.color = color;
    }

    private void Darken()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;

        color.a = 0.5f;
        sr.color = color;
    }
}
