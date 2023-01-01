using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInfoManager : MonoBehaviour
{
    public static LevelInfoManager instance;

    public bool isForMainUI = false;

    [SerializeField]
    private PlayerRopeShooter playerRopeShooter;

    [HideInInspector]
    public int curLevel;

    // 1 ~ 75 레벨별 고정 정보
    private int[] ropeRewarded = new int[76];
    private string[] levelTitle = new string[76];  

    // 플레이어 Stats
    private int lastLevelCleared;
    private bool[] isStarCleared;
    private int[] numLeastRopeUsedToClear;
    private float[] shortestSecondsTakenToClear;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        SetRopeRewarded();
        SetLevelTitle();
        LoadStats();

        if (isForMainUI)
        {
            return;
        }

        curLevel = int.Parse(SceneManager.GetActiveScene().name.Split('_')[1]);
    }

    private void Update()
    {
        if (isForMainUI)
        {
            return;
        }

        /*DebugUtility.Log("lastLevelCleared", lastLevelCleared,
            "isStarCleared", isStarCleared[curLevel],
            "numLeastRopeUsedToClear", numLeastRopeUsedToClear[curLevel],
            "shortestSecondsTaken", shortestSecondsTakenToClear[curLevel]);*/
    }

    private void SetRopeRewarded()
    {
        for (int i = 1; i <= 75; i++)
        {
            ropeRewarded[i] = 10;
        }
    }

    private void SetLevelTitle()
    {
        for (int i = 0; i < levelTitle.Length; i++)
        {
            levelTitle[i] = ""; 
        }

        levelTitle[1] = "Left-Click to Hang";
        levelTitle[2] = "Right-Click to Cut";
        levelTitle[3] = "Scroll Up to Leap";
        levelTitle[4] = "Scroll Down to Fall";
    }

    public int GetRopeRewarded()
    {
        return ropeRewarded[curLevel];
    }

    public int GetRopeRewarded(int level)
    {
        return ropeRewarded[level];
    }

    public string GetLevelTitle()
    {
        return levelTitle[curLevel];
    }

    public string GetLevelTitle(int level)
    {
        return levelTitle[level];
    }

    private void LoadStats()
    {
        if(!SaveLoadManager.instance.LoadInteger("lastLevelCleared", out lastLevelCleared))
        {
            lastLevelCleared = 0;
        }

        if (!SaveLoadManager.instance.LoadArray("isStarCleared", out isStarCleared))
        {
            isStarCleared = new bool[76];

            for (int i = 1; i <= 75; i++)
            {
                isStarCleared[i] = false;
            }
        }

        if (!SaveLoadManager.instance.LoadArray("numLeastRopeUsedToClear", out numLeastRopeUsedToClear))
        {
            numLeastRopeUsedToClear = new int[76];

            for (int i = 1; i <= 75; i++)
            {
                numLeastRopeUsedToClear[i] = 0;
            }
        }

        if (!SaveLoadManager.instance.LoadArray("shortestSecondsTakenToClear", out shortestSecondsTakenToClear))
        {
            shortestSecondsTakenToClear = new float[76];

            for (int i = 1; i < 75; i++)
            {
                shortestSecondsTakenToClear[i] = 0f;
            }
        }
    }

    public void OnClear()
    {
        UpdateStatsOnClear();
        SaveStats();
    }

    private void UpdateStatsOnClear()
    {
        if (GameManager.instance.gameState != GameManager.GameState.Cleared)
        {
            return;
        }

        if (curLevel > lastLevelCleared)
        {
            lastLevelCleared = curLevel;
        }

        if (playerRopeShooter.numRopeUsedToClear <= GetRopeRewarded())
        {
            isStarCleared[curLevel] = true;
        }

        if (GameManager.instance.IsPersonalBest(playerRopeShooter.numRopeUsedToClear,
                           numLeastRopeUsedToClear[curLevel],
                           playerRopeShooter.secondsTakenToClear,
                           shortestSecondsTakenToClear[curLevel]))
        {
            numLeastRopeUsedToClear[curLevel] = playerRopeShooter.numRopeUsedToClear;
            shortestSecondsTakenToClear[curLevel] = playerRopeShooter.secondsTakenToClear;
            LevelUIManager.instance.SetBestRecordImage(true);
        }
        else
        {
            LevelUIManager.instance.SetBestRecordImage(false);
        }
    }

    private void SaveStats()
    {
        SaveLoadManager.instance.SaveInteger("lastLevelCleared", lastLevelCleared);
        SaveLoadManager.instance.SaveArray("isStarCleared", isStarCleared);
        SaveLoadManager.instance.SaveArray("numLeastRopeUsedToClear", numLeastRopeUsedToClear);
        SaveLoadManager.instance.SaveArray("shortestSecondsTakenToClear", shortestSecondsTakenToClear);
    }

    public void GetStats(out int lastLevelCleared, out bool[] isStarCleared, out int[] numLeastRopeUsedToClear, out float[] shortestSecondsTakenToClear)
    {
        lastLevelCleared = this.lastLevelCleared;
        isStarCleared = this.isStarCleared;
        numLeastRopeUsedToClear = this.numLeastRopeUsedToClear;
        shortestSecondsTakenToClear = this.shortestSecondsTakenToClear;
    }
}