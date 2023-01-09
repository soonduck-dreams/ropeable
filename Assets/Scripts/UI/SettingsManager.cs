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
    [SerializeField] private Slider backgroundSlider;
    [SerializeField] private Slider effectSlider;
    [SerializeField] private Button openChangeNamePanelButton;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_Text currentUsernameText;
    [SerializeField] private Toggle displayElapsedTimeToggle;

    public static bool displayElapsedTimeWhilePlaying;

    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;

    private void Start()
    {
        InitResolutionDropdown();
        LoadSettings();

        SetCanChangeName();
        gameObject.SetActive(false);
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
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            SetResolution(currentResolutionIndex);
        }

        fullscreenToggle.isOn = PlayerPrefs.GetInt("isFullscreen", 1) == 1 ? true : false;
        SetFullscreen(PlayerPrefs.GetInt("isFullscreen", 1) == 1 ? true : false);

        qualityDropdown.value = PlayerPrefs.GetInt("qualityIndex", 5);
        SetQuality(PlayerPrefs.GetInt("qualityIndex", 5));

        backgroundSlider.value = PlayerPrefs.GetFloat("backgroundVolume", 0f);
        SetBackgroundVolume(PlayerPrefs.GetFloat("backgroundVolume", 0f));

        effectSlider.value = PlayerPrefs.GetFloat("effectVolume", 0f);
        SetEffectVolume(PlayerPrefs.GetFloat("effectVolume", 0f));

        RefreshShownCurrentUsernameText(PlayfabManager.username);

        displayElapsedTimeToggle.isOn = PlayerPrefs.GetInt("displayElapsedTimeWhilePlaying", 0) == 1 ? true : false;
        displayElapsedTimeWhilePlaying = PlayerPrefs.GetInt("displayElapsedTimeWhilePlaying", 0) == 1 ? true : false;
    }

    private void InitResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width >= 640 && resolutions[i].height >= 360
                && (options.Count == 0 || resolutions[i].width != resolutions[i - 1].width || resolutions[i].height != resolutions[i - 1].height))
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = options.Count - 1;
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

    public void SetBackgroundVolume(float volume)
    {
        // BG Audio Mixer
        PlayerPrefs.SetFloat("backgroundVolume", volume);
    } // TODO

    public void SetEffectVolume(float volume)
    {
        // Effect Audio Mixer
        PlayerPrefs.SetFloat("effectVolume", volume);
    } // TODO

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

    private void SetCanChangeName()
    {
        openChangeNamePanelButton.interactable = userTraitManager.userTraitData.canChangeName;

        if (!userTraitManager.userTraitData.canChangeName)
        {
            openChangeNamePanelButton.GetComponentInChildren<TMP_Text>().text = "name change restricted";
            openChangeNamePanelButton.GetComponentInChildren<TMP_Text>().fontSize = 35f;
        }
    }
}