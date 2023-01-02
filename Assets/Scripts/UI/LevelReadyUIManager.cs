using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelReadyUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text levelNo;

    [SerializeField]
    private TMP_Text levelTitle;

    [SerializeField]
    private Image starClearImage;

    [SerializeField]
    private TMP_Text bestRecord;

    [SerializeField]
    private Button playButton;

    public void Set(int levelNo, string levelTitle, bool isStarCleared, int leastRopeUsed, float shortestSecondsTaken)
    {
        this.levelNo.text = "Level " + levelNo.ToString();
        this.levelTitle.text = levelTitle;
        this.starClearImage.gameObject.SetActive(isStarCleared);
        this.bestRecord.text = leastRopeUsed.ToString() + " ropes used\n" + string.Format("{0:F3}", shortestSecondsTaken) + "sec taken";

        playButton.onClick.AddListener(PlayLevel);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void PlayLevel()
    {
        SceneLoader.LoadSceneWithLoadingScreen(levelNo.text.Replace(' ', '_'));
        //SceneManager.LoadScene(levelNo.text.Replace(' ', '_'));
    }
}