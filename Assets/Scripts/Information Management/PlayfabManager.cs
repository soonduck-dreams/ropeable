using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField]
    private LoginUIManager loginUIManager;

    // 데모용 저장 데이터를 나타내는 키. 정식판에서는 DEMO_를 뺄 예정
    private readonly string userLevelDataKey = "DEMO_UserLevelData";

    public string username { get; private set; }

    public void LoginToPlayfabServer()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("PlayFabManager: PlayFab 서버에 성공적으로 로그인했습니다.");

        username = null;

        if (result.InfoResultPayload.PlayerProfile != null)
        {
            username = result.InfoResultPayload.PlayerProfile.DisplayName;
        }

        if (username == null)
        {
            loginUIManager.ShowCreateNewAccount();
        }
        else
        {
            loginUIManager.ShowLoginSuccessful();
        }
    }

    public void InitUsername(string username)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnInitUsernameSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnInitUsernameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        username = result.DisplayName;
        loginUIManager.ShowLoginSuccessful();
        Debug.Log("PlayFabManager: 유저 이름을 최초 설정했습니다.");
    }

    public void SendLocalUserData(int lastLevelCleared, UserLevelData[] userLevelDataList)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"LastLevelCleared", lastLevelCleared.ToString() },
                {userLevelDataKey, JsonConvert.SerializeObject(userLevelDataList) }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnSendLocalUserDataSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnSendLocalUserDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("PlayFabManager: 서버에 유저 데이터를 저장했습니다.");
    }

    public void ReceiveServerUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnReceiveServerUserDataSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnReceiveServerUserDataSuccess(GetUserDataResult result)
    {
        if (result.Data == null)
        {
            Debug.Log("PlayFabManager: 서버 유저 데이터가 없어서 받지 않았습니다.");
            return;
        }

        if (!result.Data.ContainsKey("LastLevelCleared") || !result.Data.ContainsKey(userLevelDataKey))
        {
            Debug.Log("PlayFabManager: 서버 유저 데이터가 불완전하여 받지 않았습니다.");
            return;
        }

        Debug.Log("PlayFabManager: 서버로부터 유저 데이터를 성공적으로 받았습니다.");

        int lastLevelCleared = int.Parse(result.Data["LastLevelCleared"].Value);
        UserLevelData[] userLevelDataList =
            JsonConvert.DeserializeObject<UserLevelData[]>(result.Data[userLevelDataKey].Value);

        SaveLoadManager.instance.RecieveUserData(lastLevelCleared, userLevelDataList);
    }

    public void SendToLeaderboard(string leaderboardName, int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnSendToLeaderboardSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnSendToLeaderboardSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("PlayFabManager: 리더보드에 기록을 성공적으로 등록했습니다.");
    }

    public void ReceiveLeaderboard(string leaderboardName)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request, OnReceiveLeaderboard,
            error => Debug.Log(error.GenerateErrorReport()));
    }

    private void OnReceiveLeaderboard(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            Debug.Log(item.Position + " " + item.DisplayName + " " + item.StatValue);
        }
    }
}