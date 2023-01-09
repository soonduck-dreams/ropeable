using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColorChanger : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void SetParticleColor(Color color)
    {
        var col = ps.colorOverLifetime;

        Gradient gradient = new Gradient();
        gradient.SetKeys( new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f)},
                          new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)});

        col.color = gradient;
    }

    public void SetTrailColor(Color color)
    {
        var trails = ps.trails;

        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                          new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        trails.colorOverLifetime = gradient;
    }
}
