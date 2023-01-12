using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using TMPro;
using System;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField] private LoginUIManager loginUIManager;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private UserTraitManager userTraitManager;

    [SerializeField] private GameObject leaderboardRowPrefab;
    [SerializeField] private Transform leaderboardRowsParent;

    private readonly string userLevelDataKey = "UserLevelData";
    private readonly string userTraitDataKey = "UserTraitData";

    public static string username { get; private set; }

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
            OnLoginFailed);
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

    private void OnLoginFailed(PlayFabError error)
    {
        Debug.Log("PlayFabManager: PlayFab 서버 로그인에 실패했습니다.");
        loginUIManager.ShowLoginFailed();
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

    public void ChangeUsername(string username)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnChangeUsernameSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnChangeUsernameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        username = result.DisplayName;
        settingsManager.RefreshShownCurrentUsernameText(username);
        Debug.Log("PlayFabManager: 유저 이름을 변경했습니다.");
    }

    public void SendUserLevelData(int lastLevelCleared, UserLevelData[] userLevelDataList)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"LastLevelCleared", lastLevelCleared.ToString() },
                {userLevelDataKey, JsonConvert.SerializeObject(userLevelDataList) }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnSendUserLevelDataSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnSendUserLevelDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("PlayFabManager: 서버에 유저 레벨 데이터를 저장했습니다.");
    }

    public void ReceiveUserLevelData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnReceiveUserLevelDataSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnReceiveUserLevelDataSuccess(GetUserDataResult result)
    {
        if (result.Data == null)
        {
            Debug.Log("PlayFabManager: 유저 레벨 데이터가 없어서 받지 않았습니다.");
            LevelInfoManager.instance.InitStatsIfNeeded();
            MainUIManager.instance.Open();
            return;
        }

        if (!result.Data.ContainsKey("LastLevelCleared") || !result.Data.ContainsKey(userLevelDataKey))
        {
            Debug.Log("PlayFabManager: 유저 레벨 데이터가 불완전하여 받지 않았습니다.");
            LevelInfoManager.instance.InitStatsIfNeeded();
            MainUIManager.instance.Open();
            return;
        }

        Debug.Log("PlayFabManager: 유저 레벨 데이터를 성공적으로 받았습니다.");

        int lastLevelCleared = int.Parse(result.Data["LastLevelCleared"].Value);
        UserLevelData[] userLevelDataList =
            JsonConvert.DeserializeObject<UserLevelData[]>(result.Data[userLevelDataKey].Value);

        SaveLoadManager.instance.RecieveUserLevelData(lastLevelCleared, userLevelDataList);
    }

    public void SendLeaderboard(string leaderboardName, int score)
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

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnSendLeaderboardSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnSendLeaderboardSuccess(UpdatePlayerStatisticsResult result)
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

        PlayFabClientAPI.GetLeaderboard(request, OnReceiveLeaderboardSuccess,
            error => Debug.Log(error.GenerateErrorReport()));
    }

    private void OnReceiveLeaderboardSuccess(GetLeaderboardResult result)
    {
        foreach (Transform item in leaderboardRowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject row = Instantiate(leaderboardRowPrefab, leaderboardRowsParent);
            TMP_Text[] rowInfo = row.GetComponentsInChildren<TMP_Text>();

            int realStatValue = 1000000000 - item.StatValue;

            rowInfo[0].text = "#" + (item.Position + 1).ToString();
            rowInfo[1].text = item.DisplayName;
            rowInfo[2].text = (realStatValue / 1000000).ToString();
            rowInfo[3].text = ((realStatValue % 1000000) / 1000f).ToString() + "s";
        }

        Debug.Log("PlayfabManager: 리더보드 정보를 가져왔습니다.");
    }

    public void SendUserTraitData(UserTraitData userTraitData)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {userTraitDataKey, JsonConvert.SerializeObject(userTraitData) }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnSendUserTraitDataSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnSendUserTraitDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("PlayFabManager: 서버에 유저 특징 데이터를 저장했습니다.");
    }

    public void ReceiveUserTraitData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnReceiveUserTraitDataSuccess,
            error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnReceiveUserTraitDataSuccess(GetUserDataResult result)
    {
        if (result.Data == null)
        {
            Debug.Log("PlayFabManager: 유저 특징 데이터가 없어서 받지 않았습니다.");
            userTraitManager.InitUserTraitData();
            return;
        }

        if (!result.Data.ContainsKey(userTraitDataKey))
        {
            Debug.Log("PlayFabManager: 유저 특징 데이터가 불완전하여 받지 않았습니다.");
            userTraitManager.InitUserTraitData();
            return;
        }

        Debug.Log("PlayFabManager: 유저 특징 데이터를 성공적으로 받았습니다.");

        UserTraitData userTraitData =
            JsonConvert.DeserializeObject<UserTraitData>(result.Data[userTraitDataKey].Value);

        SaveLoadManager.instance.ReceiveUserTraitData(userTraitData);
    }
}