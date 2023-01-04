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
    private TMP_Text clearLevelText;

    [SerializeField]
    private TMP_Text clearRopesUsedText;

    [SerializeField]
    private TMP_Text clearTimeTakenText;

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
    private TMP_Text pauseLevelText;

    [SerializeField]
    private TMP_Text pauseRopesUsedText;

    [SerializeField]
    private TMP_Text pauseTimeTakenText;

    [SerializeField]
    private TMP_Text pauseItemGottenText;

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

    public void ShowPauseUI(int ropeUsedToClear, float secondsTakenToClear, int itemGotten)
    {
        GameManager.instance.PauseGame();
        pauseLevelText.text = "Level " + LevelInfoManager.instance.curLevel.ToString();
        pauseRopesUsedText.text = ropeUsedToClear.ToString();
        pauseTimeTakenText.text = secondsTakenToClear.ToString() + "s";
        pauseItemGottenText.text = itemGotten.ToString() + "/3";
        
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

        clearLevelText.text = levelTextBuilder.ToString();
    }

    private void SetRopeUsedText(int ropeUsedToClear)
    {
        clearRopesUsedText.text = ropeUsedToClear.ToString();
    }

    private void SetTimeTakenText(float secondsTakenToClear)
    {
        clearTimeTakenText.text = secondsTakenToClear.ToString() + "s";
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

    public void OnPauseButton()
    {
        ShowPauseUI(playerRopeShooter.numRopeUsedToClear, playerRopeShooter.secondsTakenToClear, GameManager.instance.score);
    }

    public void OnLeaderboardButton()
    {
        if (SaveLoadManager.allowToSaveAndLoadOnline)
        {
            SaveLoadManager.instance.RequestToGetLeaderboard(LevelInfoManager.instance.curLevel);
        }
    }
}
