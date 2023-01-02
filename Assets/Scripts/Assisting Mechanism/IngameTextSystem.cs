using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using UnityEngine;

public class IngameTextSystem : MonoBehaviour
{
    [SerializeField]
    private CameraMover cameraMover;

    [SerializeField]
    private float[] pauseSeconds;

    [SerializeField]
    private string[] lines;

    [SerializeField]
    private float textSpeed = 0.1f;

    private TMP_Text textComponent;

    private bool played;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();

        textComponent.text = string.Empty;

        played = false;
    }

    private void Update()
    {
        if (played)
        {
            return;
        }

        if (!cameraMover.isOutOfCamera(transform.position, 0f))
        {
            StartCoroutine(TypeLines());
            played = true;
        }
    }

    private IEnumerator TypeLines()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            yield return new WaitForSeconds(pauseSeconds[i]);
            StartCoroutine(TypeLine(lines[i]));
        }
    }

    private IEnumerator TypeLine(string str)
    {
        textComponent.text = string.Empty;

        foreach (char c in str.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}