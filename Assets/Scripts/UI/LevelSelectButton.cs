using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public int levelNo;
    public bool isCleared;
    public bool isStarCleared;
    public bool canPlay;
    public int numLeastRopeUsedToClear;
    public float shortestSecondsTakenToClear;

    private TMP_Text levelNoText;
    private Button button;

    [SerializeField]
    private LevelReadyUIManager levelReadyUI;

    private void Awake()
    {
        levelNoText = GetComponentInChildren<TMP_Text>();
        button = GetComponent<Button>();
        levelReadyUI = FindObjectOfType<LevelReadyUIManager>();
    }

    public void SetInfo(int levelNo, int lastLevelCleared, bool isStarCleared, int numLeastRopeUsedToClear, float shortestSecondsTakenToClear)
    {
        this.levelNo = levelNo;
        this.canPlay = levelNo <= lastLevelCleared + 1;
        this.isCleared = levelNo <= lastLevelCleared;
        this.isStarCleared = isStarCleared;
        this.numLeastRopeUsedToClear = numLeastRopeUsedToClear;
        this.shortestSecondsTakenToClear = shortestSecondsTakenToClear;
    }

    public void Display()
    {
        levelNoText.text = levelNo.ToString();
    }

    public void SetClickEvent()
    {
        button.onClick.AddListener(SetLevelReadyUI);
        button.onClick.AddListener(levelReadyUI.Show);
    }

    public void SetButtonColor(bool canPlay)
    {
        Image image = GetComponent<Image>();

        if(canPlay)
        {
            //
        }
        else
        {
            image.color = Color.gray;
        }
    }

    public void SetInteractable(bool canPlay)
    {
        button.interactable = canPlay;
    }

    private void SetLevelReadyUI()
    {
        levelReadyUI.Set(levelNo, LevelInfoManager.instance.GetLevelTitle(levelNo), isStarCleared, numLeastRopeUsedToClear, shortestSecondsTakenToClear);
    }
}