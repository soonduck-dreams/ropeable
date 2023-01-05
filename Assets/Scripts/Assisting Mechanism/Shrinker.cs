using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrinker : MonoBehaviour
{
    [SerializeField] private float magnitude;
    [SerializeField] private float speed;

    private float shrinkScale;
    private float amplitudeMultiplier;
    private float translater;

    private void Start()
    {
        amplitudeMultiplier = (1 - magnitude) / 2;
        translater = 1 - (1 - magnitude) / 2;
    }

    private void Update()
    {
        shrinkScale = amplitudeMultiplier * Mathf.Cos(Time.time * speed) + translater;
        transform.localScale = new Vector3(shrinkScale, shrinkScale, 1f);
    }
}
