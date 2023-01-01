using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetItem : MonoBehaviour, ISpeaker
{
    public Color particleColor;

    [SerializeField]
    private GameObject targetGottenParticlePrefab;

    private List<IListener> listeners;

    private void Awake()
    {
        listeners = new List<IListener>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            GameManager.instance.OnTargetItemEaten();
            SpeakToListeners();
            ParticleManager.instance.PlayParticle(targetGottenParticlePrefab, transform.position, particleColor);
            Destroy(gameObject);
        }
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
}
