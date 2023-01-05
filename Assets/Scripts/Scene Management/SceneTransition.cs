using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator crossfade;

    public enum EndWith
    {
        Nothing,
        Crossfade
    }

    [HideInInspector] public static EndWith endWith = EndWith.Nothing;

    public delegate void LoadSceneAction();

    private void Start()
    {

        switch (endWith)
        {
            case EndWith.Nothing:
                break;

            case EndWith.Crossfade:
                StartCoroutine(CrossfadeEnd());
                break;
        }
    }

    public IEnumerator CrossfadeStart(LoadSceneAction loadSceneAction, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        crossfade.gameObject.SetActive(true);
        crossfade.SetTrigger("Start");

        yield return new WaitForSeconds(0.5f);
        loadSceneAction();
    }

    public IEnumerator CrossfadeEnd()
    {
        crossfade.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        crossfade.gameObject.SetActive(false);
    }
}
