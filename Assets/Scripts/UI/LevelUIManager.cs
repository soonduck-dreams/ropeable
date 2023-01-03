using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    public static LevelUIManager instance;

    [SerializeField]
    private GameObject clearUI;

    [SerializeField]
    private TMP_Text clearText;

    [SerializeField]
    private TMP_Text levelText;

    [SerializeField]
    private TMP_Text ropesUsedText;

    [SerializeField]
    private TMP_Text timeTakenText;

    [SerializeField]
    private GameObject ropesRewardedArea;

    [SerializeField]
    private TMP_Text ropesRewardedText;

    [SerializeField]
    private GameObject bestRecordImage;

    [SerializeField]
    private GameObject pauseUI;

    [SerializeField]
    private GameObject pauseButton;

    [SerializeField]
    private TMP_Text descriptionText;

    [SerializeField]
    private PlayerRopeShooter playerRopeShooter;

    public TMP_Text debugText;

    private StringBuilder levelTextBuilder = new StringBuilder();
    private StringBuilder descriptionTextBuilder = new StringBuilder();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        clearUI.SetActive(false);
        SetLevelText();
        SetRopeRewardedText();
    }

    public void ShowClearUI(int ropeUsedToClear, float secondsTakenToClear)
    {
        if (ropeUsedToClear <= LevelInfoManager.instance.GetRopeRewarded())
        {
            Color color;
            ColorUtility.TryParseHtmlString("#FBF236", out color);

            clearText.text = "Star Cleared";
            clearText.color = color;
            
            ropesRewardedArea.SetActive(true);
        }

        SetRopeUsedText(ropeUsedToClear);
        SetTimeTakenText(secondsTakenToClear);
        pauseButton.SetActive(false);
        clearUI.SetActive(true);
    }

    public void ShowPauseUI()
    {
        GameManager.instance.PauseGame();
        SetDescriptionText(playerRopeShooter.numRopeUsedToClear, GameManager.instance.score);
        pauseUI.SetActive(true);
    }

    public void ClosePauseUI()
    {
        GameManager.instance.UnpauseGame();
        pauseUI.SetActive(false);
    }

    private void SetLevelText()
    {
        levelTextBuilder.Append("Level ");
        levelTextBuilder.Append(LevelInfoManager.instance.curLevel.ToString());

        levelText.text = levelTextBuilder.ToString();
    }

    private void SetRopeUsedText(int ropeUsedToClear)
    {
        ropesUsedText.text = ropeUsedToClear.ToString();
    }

    private void SetTimeTakenText(float secondsTakenToClear)
    {
        timeTakenText.text = secondsTakenToClear.ToString() + "s";
    }

    private void SetRopeRewardedText()
    {
        ropesRewardedText.text = string.Format("use {0} or less ropes\nto star clear",
            LevelInfoManager.instance.GetRopeRewarded().ToString());
    }

    public void SetBestRecordImage(bool isPersonalBest)
    {
        bestRecordImage.SetActive(isPersonalBest);
    }

    private void SetDescriptionText(int ropeUsed, int score)
    {
        descriptionTextBuilder.Clear();
        descriptionTextBuilder.Append("Level ");
        descriptionTextBuilder.Append(LevelInfoManager.instance.curLevel.ToString());
        descriptionTextBuilder.Append("\nRopes used: ");
        descriptionTextBuilder.Append(ropeUsed);
        descriptionTextBuilder.Append("\nSushi gotten: ");
        descriptionTextBuilder.Append(score);
        descriptionTextBuilder.Append("/3");

        descriptionText.text = descriptionTextBuilder.ToString();
    }
}
