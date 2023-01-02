using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager instance;

    [SerializeField]
    private GameObject mainScreen;

    [SerializeField]
    private GameObject levelSelectScreen;

    [SerializeField]
    private GameObject gridContent;

    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private LevelReadyUIManager levelReadyUI;

    private int lastLevelCleared;
    private bool[] isStarCleared;
    private int[] numLeastRopeUsedToClear;
    private float[] shortestSecondsTakenToClear;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        LevelInfoManager.instance.GetStats(out lastLevelCleared, out isStarCleared,
            out numLeastRopeUsedToClear, out shortestSecondsTakenToClear);
        UpdateLvSelect();
        OpenWithOpenMode();
    }

    public void CloseMain()
    {
        mainScreen.SetActive(false);
    }

    public void OpenMain()
    {
        mainScreen.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        levelSelectScreen.SetActive(false);
    }

    public void OpenLevelSelect()
    {
        levelSelectScreen.SetActive(true);
    }

    public void UpdateLvSelect()
    {
        GameObject button;
        LevelSelectButton buttonInfo;

        for (int i = 1; i <= 75; i++)
        {
            button = Instantiate(buttonPrefab, gridContent.transform);
            buttonInfo = button.GetComponent<LevelSelectButton>();

            buttonInfo.SetInfo(i, lastLevelCleared, isStarCleared[i], numLeastRopeUsedToClear[i], shortestSecondsTakenToClear[i]);
            buttonInfo.SetInteractable(buttonInfo.canPlay);
            buttonInfo.SetButtonColor(buttonInfo.canPlay);
            buttonInfo.Display();
            buttonInfo.SetClickEvent();
        }
    }

    private void OpenWithOpenMode()
    {
        switch(MainUIOpenMode.openMode)
        {
            case MainUIOpenMode.OpenMode.Main:
                levelReadyUI.gameObject.SetActive(false);
                levelSelectScreen.SetActive(false);
                break;

            case MainUIOpenMode.OpenMode.LevelSelect:
                levelReadyUI.gameObject.SetActive(false);
                mainScreen.SetActive(false);
                break;
        }
    }
}
