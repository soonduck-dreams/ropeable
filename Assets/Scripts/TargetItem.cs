using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetItem : MonoBehaviour, ISpeaker
{
    public Color itemColor;

    [SerializeField]
    private GameObject targetGottenParticlePrefab;

    [SerializeField]
    private GameObject ascendingTextPrefab;

    private List<IListener> listeners;

    private void Awake()
    {
        listeners = new List<IListener>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float ascendingTextOffset = 1f;
        Vector3 ascendingTextPosition = transform.position + ascendingTextOffset * Vector3.up;

        if(collision.tag == "Player")
        {
            GameManager.instance.OnTargetItemEaten();
            SpeakToListeners();
            ParticleManager.instance.PlayParticle(targetGottenParticlePrefab, transform.position, itemColor);
            TextUtility.DisplayAscendingText(GameManager.instance.score.ToString(),
                                                ascendingTextPrefab, ascendingTextPosition,
                                                itemColor, 1f, 1f);
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
