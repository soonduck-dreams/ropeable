using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightManager : MonoBehaviour
{
    private Light2D globalLight;
    private float defaultIntensity = 0.7f;

    private void Start()
    {
        globalLight = GetComponent<Light2D>();
    }

    public void GlowUp(float intensity, float seconds)
    {
        StartCoroutine(GlowUpCoroutine(intensity, seconds));
    }

    private IEnumerator GlowUpCoroutine(float intensity, float seconds)
    {
        float startTime = Time.time;

        globalLight.intensity = intensity;

        while (Time.time < startTime + seconds)
        {
            yield return null;
        }

        globalLight.intensity = defaultIntensity;
    }
}
