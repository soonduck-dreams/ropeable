using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField]
    private LoginUIManager loginUIManager;

    public string username { get; private set; }

    private void Start()
    {
        LoginToPlayfabServer();
    }

    private void LoginToPlayfabServer()
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
}