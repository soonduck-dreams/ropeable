using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private PlayfabManager playfabManager;
    [SerializeField] private UserTraitManager userTraitManager;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button openChangeNamePanelButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_Text currentUsernameText;
    [SerializeField] private Toggle displayElapsedTimeToggle;

    public static bool displayElapsedTimeWhilePlaying;

    private Resolution[] resolutions;
    private int defaultResolutionIndex = 0;

    private void OnDisable()
    {
        InitResolutionDropdown();
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex");
            resolutionDropdown.RefreshShownValue();
            SetResolution(PlayerPrefs.GetInt("resolutionIndex"));
        }
        else
        {
            resolutionDropdown.value = defaultResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            SetResolution(defaultResolutionIndex);
        }

        fullscreenToggle.isOn = PlayerPrefs.GetInt("isFullscreen", 1) == 1 ? true : false;
        SetFullscreen(PlayerPrefs.GetInt("isFullscreen", 1) == 1 ? true : false);

        qualityDropdown.value = PlayerPrefs.GetInt("qualityIndex", 5);
        SetQuality(PlayerPrefs.GetInt("qualityIndex", 5));

        RefreshShownCurrentUsernameText(PlayfabManager.username);

        displayElapsedTimeToggle.isOn = PlayerPrefs.GetInt("displayElapsedTimeWhilePlaying", 0) == 1 ? true : false;
        displayElapsedTimeWhilePlaying = PlayerPrefs.GetInt("displayElapsedTimeWhilePlaying", 0) == 1 ? true : false;
    }

    private void InitResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        defaultResolutionIndex = 0;

        float minResolutionAreaDiff = -1f;
        float nowResolutionAreaDiff;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width >= 640 && resolutions[i].height >= 360
                && (options.Count == 0 || resolutions[i].width != resolutions[i - 1].width || resolutions[i].height != resolutions[i - 1].height))
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                nowResolutionAreaDiff = Mathf.Abs(resolutions[i].width * resolutions[i].height
                    - Screen.currentResolution.width * Screen.currentResolution.height);

                if (minResolutionAreaDiff == -1f || nowResolutionAreaDiff < minResolutionAreaDiff)
                {
                    minResolutionAreaDiff = nowResolutionAreaDiff;
                    defaultResolutionIndex = options.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
    }

    public void SetResolution(int resolutionIndex)
    {
        int width = int.Parse(resolutionDropdown.options[resolutionIndex].text.Split('x')[0]);
        int height = int.Parse(resolutionDropdown.options[resolutionIndex].text.Split('x')[1]);

        Screen.SetResolution(width, height, Screen.fullScreen);

        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("isFullscreen", isFullscreen ? 1 : 0);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("qualityIndex", qualityIndex);
    }

    public void ConfirmChangedNameButton()
    {
        if (usernameInputField.text.Length <= 2 || usernameInputField.text.Length >= 26)
        {
            return;
        }

        playfabManager.ChangeUsername(usernameInputField.text);
    }

    public void RefreshShownCurrentUsernameText(string username)
    {
        currentUsernameText.text = username;
    }

    public void SetDisplayElapsedTimeWhilePlaying(bool display)
    {
        displayElapsedTimeWhilePlaying = display;

        PlayerPrefs.SetInt("displayElapsedTimeWhilePlaying", display ? 1 : 0);
    }

    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
    }

    public void SetCanChangeName(bool canChangeName)
    {
        openChangeNamePanelButton.interactable = canChangeName;

        if (!canChangeName)
        {
            openChangeNamePanelButton.GetComponentInChildren<TMP_Text>().text = "name change restricted";
            openChangeNamePanelButton.GetComponentInChildren<TMP_Text>().fontSize = 35f;
        }
    }
}