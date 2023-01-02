using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ascender : MonoBehaviour
{
    [HideInInspector]
    public float ascendSpeed { get; set; }

    [HideInInspector]
    public float lifeSeconds { get; set; }

    private void Start()
    {
        StartCoroutine(DestroyAfter(lifeSeconds));
    }

    private void Update()
    {
        transform.Translate(ascendSpeed * Time.deltaTime * Vector3.up);
    }

    private IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
