using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    [SerializeField] private PlayfabManager playfabManager;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private GameObject standby;
    [SerializeField] private GameObject createNewAccount;
    [SerializeField] private GameObject loginSuccessful;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_Text loginSuccessfulText;
    [SerializeField] private GameObject loginFailed;

    private void Start()
    {
        playfabManager.LoginToPlayfabServer();
    }

    public void ShowCreateNewAccount()
    {
        HideStandby();
        createNewAccount.SetActive(true);
    }

    public void ShowLoginSuccessful()
    {
        HideStandby();
        HideCreateNewAccount();

        loginSuccessfulText.text = "Welcome, " + PlayfabManager.username;
        loginSuccessful.SetActive(true);
        GoToMainScene();
    }

    public void ShowLoginFailed()
    {
        HideStandby();
        loginFailed.SetActive(true);
    }

    public void SubmitNameButton()
    {
        if (usernameInputField.text.Length <= 2 || usernameInputField.text.Length >= 26)
        {
            return;
        }

        playfabManager.InitUsername(usernameInputField.text);
    }

    private void HideStandby()
    {
        standby.SetActive(false);
    }

    private void HideCreateNewAccount()
    {
        createNewAccount.SetActive(false);
    }

    private void GoToMainScene()
    {
        SceneTransition.endWith = SceneTransition.EndWith.Crossfade;
        StartCoroutine(sceneTransition.CrossfadeStart( () => { SceneManager.LoadScene("MainMenu"); }, 3f));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}