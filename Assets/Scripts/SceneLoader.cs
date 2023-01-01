using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class SceneLoader : MonoBehaviour
{
    private static string sceneName;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TMP_Text loadingText;

    public static void LoadSceneWithLoadingScreen(string sceneName)
    {
        SceneLoader.sceneName = sceneName;
        SceneManager.LoadScene("Loading");
    }

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneLoader.sceneName);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progressValue;
            loadingText.text = "LOADING...";

            yield return null;
        }
    }
}
