using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField]
    private PlayfabManager playfabManager;

    public static SaveLoadManager instance;
    public readonly static bool allowToSaveAndLoadOnline = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void RequestToSaveUserData(int lastLevelCleared, bool[] isStarCleared, int[] numLeastRopeUsedToClear, float[] shortestSecondsTakenToClear)
    {
        UserLevelData[] userLevelDataList = new UserLevelData[isStarCleared.Length];

        for (int i = 0; i < userLevelDataList.Length; i++)
        {
            userLevelDataList[i] =
                new UserLevelData(i, isStarCleared[i], numLeastRopeUsedToClear[i], shortestSecondsTakenToClear[i]);
        }

        playfabManager.SendLocalUserData(lastLevelCleared, userLevelDataList);
    }

    public void RequestToLoadUserData()
    {
        playfabManager.ReceiveServerUserData();
    }

    public void RecieveUserData(int lastLevelCleared, UserLevelData[] userLevelDataList)
    {
        bool[] isStarCleared = new bool[userLevelDataList.Length];
        int[] numLeastRopeUsedToClear = new int[userLevelDataList.Length];
        float[] shortestSecondsTakenToClear = new float[userLevelDataList.Length];

        for (int i = 0; i < userLevelDataList.Length; i++)
        {
            isStarCleared[i] = userLevelDataList[i].isStarCleared;
            numLeastRopeUsedToClear[i] = userLevelDataList[i].numLeastRopeUsedToClear;
            shortestSecondsTakenToClear[i] = userLevelDataList[i].shortestSecondsTakenToClear;
        }

        LevelInfoManager.instance.SetStats(lastLevelCleared, isStarCleared, numLeastRopeUsedToClear, shortestSecondsTakenToClear);
    }

    public void RequestToSendToLeaderboard(int level, int numLeastRopeUsedToClear, float shortestSecondsTakenToClear)
    {
        string leaderboardName = "LevelScore" + level.ToString();
        int score = LeaderboardScore(numLeastRopeUsedToClear, shortestSecondsTakenToClear);
        
        playfabManager.SendToLeaderboard(leaderboardName, score);
    }

    public void RequestToGetLeaderboard(int level)
    {
        string leaderboardName = "LevelScore" + level.ToString();

        playfabManager.ReceiveLeaderboard(leaderboardName);
    }

    private int LeaderboardScore(int ropeRecord, float timeRecord)
    {
        string timeString = timeRecord.ToString().Split('.')[0] + timeRecord.ToString().Split('.')[1];

        return ropeRecord * 1000000 + int.Parse(timeString);
    }
}