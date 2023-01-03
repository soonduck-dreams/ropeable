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

    public static readonly int maxLevel = 50;

    // 레벨별 고정 정보
    private int[] ropeRewarded = new int[maxLevel + 1];
    private string[] levelTitle = new string[maxLevel + 1];  

    // 플레이어 Stats
    private static int lastLevelCleared = -1;
    private static bool[] isStarCleared = null;
    private static int[] numLeastRopeUsedToClear = null;
    private static float[] shortestSecondsTakenToClear = null;

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
        InitStatsIfNeeded();

        if (isForMainUI)
        {
            return;
        }

        curLevel = int.Parse(SceneManager.GetActiveScene().name.Split('_')[1]);
    }

    private void SetRopeRewarded()
    {
        for (int i = 1; i <= maxLevel; i++)
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

    private void InitStatsIfNeeded()
    {
        if (lastLevelCleared == -1)
        {
            lastLevelCleared = 0;
        }

        if (isStarCleared == null)
        {
            isStarCleared = new bool[maxLevel + 1];
            for (int i = 1; i <= maxLevel; i++)
            {
                isStarCleared[i] = false;
            }
        }

        if (numLeastRopeUsedToClear == null)
        {
            numLeastRopeUsedToClear = new int[maxLevel + 1];
            for (int i = 1; i <= maxLevel; i++)
            {
                numLeastRopeUsedToClear[i] = 0;
            }
        }

        if (shortestSecondsTakenToClear == null)
        {
            shortestSecondsTakenToClear = new float[maxLevel + 1];
            for (int i = 1; i <= maxLevel; i++)
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

    private void LoadStats()
    {
        if (SaveLoadManager.allowToSaveAndLoadOnline)
        {
            SaveLoadManager.instance.RequestToLoadUserData();
        }
    }

    private void SaveStats()
    {
        if (SaveLoadManager.allowToSaveAndLoadOnline)
        {
            SaveLoadManager.instance.RequestToSaveUserData(lastLevelCleared, isStarCleared, numLeastRopeUsedToClear, shortestSecondsTakenToClear);
        }
    }

    public void SetStats(int lastLevelCleared, bool[] isStarCleared, int[] numLeastRopeUsedToClear, float[] shortestSecondsTakenToClear)
    {
        LevelInfoManager.lastLevelCleared = lastLevelCleared;
        LevelInfoManager.isStarCleared = isStarCleared;
        LevelInfoManager.numLeastRopeUsedToClear = numLeastRopeUsedToClear;
        LevelInfoManager.shortestSecondsTakenToClear = shortestSecondsTakenToClear;
    }

    public void GetStats(out int lastLevelCleared, out bool[] isStarCleared, out int[] numLeastRopeUsedToClear, out float[] shortestSecondsTakenToClear)
    {
        lastLevelCleared = LevelInfoManager.lastLevelCleared;
        isStarCleared = LevelInfoManager.isStarCleared;
        numLeastRopeUsedToClear = LevelInfoManager.numLeastRopeUsedToClear;
        shortestSecondsTakenToClear = LevelInfoManager.shortestSecondsTakenToClear;
    }
}