using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBlock : MonoBehaviour, IRopeable, IListener, ISpeaker, IPositionLocalizer, IMagnetTarget
{
    public GameObject destroyer;

    public enum SpeakAbout
    {
        Nothing,
        RopeHang,
        RopeCut
    }

    public SpeakAbout speakAbout = SpeakAbout.Nothing;

    private List<IListener> listeners;

    private float fallDestroyThreshold = -10f;

    private void Awake()
    {
        listeners = new List<IListener>();
    }

    public void Start()
    {
        ISpeaker speaker = destroyer.GetComponent<ISpeaker>();

        if (speaker == null)
        {
            return;
        }

        speaker.AddListener(this);
    }

    public void Update()
    {
        if (transform.position.y < fallDestroyThreshold)
        {
            Destroy(gameObject);
        }
    }

    public string GetTerrainType()
    {
        return "guard";
    }

    public void OnRopeHang()
    {
        if (speakAbout == SpeakAbout.RopeHang)
        {
            SpeakToListeners();
        }
    }

    public void OnRopeCut()
    {
        if (speakAbout == SpeakAbout.RopeCut)
        {
            SpeakToListeners();
        }
    }

    public void OnListen()
    {
        Rigidbody2D rg = GetComponent<Rigidbody2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;

        rg.constraints = RigidbodyConstraints2D.None;

        color.a = 0.5f;
        sr.color = color;
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

    public Vector3 WorldToLocalPos(Vector3 worldPos)
    {
        return (Vector2)transform.InverseTransformPoint(worldPos);
    }

    public Vector3 LocalToWorldPos(Vector3 localPos)
    {
        return (Vector2)transform.TransformPoint(localPos);
    }
}