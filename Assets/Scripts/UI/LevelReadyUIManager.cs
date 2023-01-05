using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelReadyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject homeButton;
    [SerializeField] private TMP_Text levelNo;
    [SerializeField] private TMP_Text levelTitle;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject unclearedCase;
    [SerializeField] private GameObject clearedCase;
    [SerializeField] private TMP_Text ropeText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text starClearText;
    [SerializeField] private GameObject leaderboardUI;

    private int rawLevelNo;

    public void Set(int levelNo, string levelTitle, bool isStarCleared, int leastRopeUsed, float shortestSecondsTaken)
    {
        this.levelNo.text = "Level " + levelNo.ToString();
        this.levelTitle.text = levelTitle;

        rawLevelNo = levelNo;

        if (leastRopeUsed == 0)
        {
            clearedCase.SetActive(false);
            unclearedCase.SetActive(true);
        }
        else
        {
            ropeText.text = leastRopeUsed.ToString();
            timeText.text = shortestSecondsTaken.ToString() + "s";

            if (isStarCleared)
            {
                starClearText.text = "STAR CLEARED!";
            }

            unclearedCase.SetActive(false);
            clearedCase.SetActive(true);
        }

        playButton.onClick.AddListener(PlayLevel);
    }

    public void Show()
    {
        homeButton.SetActive(false);
        leaderboardUI.SetActive(false);
        gameObject.SetActive(true);
    }

    private void PlayLevel()
    {
        SceneLoader.LoadSceneWithLoadingScreen(levelNo.text.Replace(' ', '_'));
        //SceneManager.LoadScene(levelNo.text.Replace(' ', '_'));
    }

    public int GetLevel()
    {
        return rawLevelNo;
    }
}