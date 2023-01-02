using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }

        instance = this;
    }

    public void PlayParticle(GameObject particlePrefab, Vector3 pos)
    {
        GameObject particleObject = Instantiate(particlePrefab, pos, Quaternion.identity);
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

        particleSystem.Play();
    }

    public void PlayParticle(GameObject particlePrefab, Vector3 pos, float rotationZ)
    {
        GameObject particleObject = Instantiate(particlePrefab, pos, Quaternion.Euler(0f, 0f, rotationZ));
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

        particleSystem.Play();
    }

    public void PlayParticle(GameObject particlePrefab, Vector3 pos, Color color)
    {
        GameObject particleObject = Instantiate(particlePrefab, pos, Quaternion.identity);
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
        ParticleColorChanger particleColorChanger = particleObject.GetComponent<ParticleColorChanger>();

        particleColorChanger.SetParticleColor(color);
        particleColorChanger.SetTrailColor(color);

        particleSystem.Play();
    }

    public void PlayParticleForSeconds(GameObject particlePrefab, GameObject targetPrefab, float seconds)
    {
        StartCoroutine(PlayParticleForSecondsCoroutine(particlePrefab, targetPrefab, seconds));
    }

    private IEnumerator PlayParticleForSecondsCoroutine(GameObject particlePrefab, GameObject targetPrefab, float seconds)
    {
        float beginTime = Time.time;
        Vector3 pos;

        while (Time.time < beginTime + seconds)
        {
            pos = targetPrefab.transform.position;

            PlayParticle(particlePrefab, pos);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
