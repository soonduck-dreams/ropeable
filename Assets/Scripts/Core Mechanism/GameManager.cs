using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerRopeShooter playerRopeShooter;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private CameraMover cameraMover;
    [SerializeField] private GlobalLightManager globalLightManager;

    public static GameManager instance;

    public int score { get; private set; }

    public enum GameState
    {
        Playing,
        Cleared,
        Paused,
        Gameover,
        Standby
    }
    public GameState gameState { get; private set; }

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
        score = 0;

        gameState = GameState.Playing;

        PlayBackgroundDependingOnLevel();
    }

    public void OnPlayerDie()
    {
        if(gameState != GameState.Playing)
        {
            return;
        }

        globalLightManager.GlowUp(1.2f, 0.05f);
        playerHealth.PlayDieParticle();
        playerHealth.DestroyPlayer();
        cameraMover.ShakeCamera(0.05f, 3f);
        Gameover();
    }

    public void Gameover()
    {
        gameState = GameState.Gameover;
        SceneTransition.endWith = SceneTransition.EndWith.Crossfade;

        LevelUIManager.instance.HidePauseButton();
        
        StartCoroutine(sceneTransition.CrossfadeStart(Restart, 2f));
    }

    private void Restart()
    {
        UnpauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void AddScore()
    {
        score++;
    }

    public void OnTargetItemEaten()
    {
        AddScore();

        AudioManager.instance.PlayEffects("GetItemGeneral");

        if (score == 1)
        {
            AudioManager.instance.PlayEffects("GetFirstItem", true);
        }
        else if(score == 2)
        {
            AudioManager.instance.PlayEffects("GetSecondItem", true);
        }

        if (score >= 3)
        {
            OnLevelClear();
        }
    }

    private void OnLevelClear()
    {
        gameState = GameState.Cleared;
        playerRopeShooter.StopAndRecordTimeTaken();
        LevelInfoManager.instance.OnClear();
        LevelUIManager.instance.ShowClearUI(playerRopeShooter.numRopeUsedToClear, playerRopeShooter.secondsTakenToClear);
        AudioManager.instance.PlayEffects("LevelClear", true);
    }

    public void GoToNextLevel()
    {
        string nextLevelSceneName = "Level_" + (LevelInfoManager.instance.curLevel + 1).ToString();

        SceneLoader.LoadSceneWithLoadingScreen(nextLevelSceneName);
        //SceneManager.LoadScene(nextLevelSceneName);
    }

    public void RetryLevel()
    {
        switch (gameState)
        {
            case GameState.Playing:
            case GameState.Paused:
                LevelUIManager.instance.ClosePauseUI();
                OnPlayerDie();
                break;

            case GameState.Cleared:
                Restart();
                break;

            case GameState.Gameover:
                break;
        }        
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        gameState = GameState.Standby;
        AudioManager.instance.StopAllSounds();
        MainUIOpenMode.openMode = MainUIOpenMode.OpenMode.LevelSelect;
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        gameState = GameState.Paused;
        Time.timeScale = 0f;

        if (playerRopeShooter.timeMeasurer != null)
        {
            playerRopeShooter.timeMeasurer.PauseMeasure();
        }
    }

    public void UnpauseGame()
    {
        if (gameState == GameState.Paused)
        {
            gameState = GameState.Playing;
        }

        Time.timeScale = 1f;

        if (playerRopeShooter.timeMeasurer != null && playerRopeShooter.numRopeUsedToClear > 0)
        {
            playerRopeShooter.timeMeasurer.StartMeasure();
        }
    }

    public bool IsPersonalBest(int ropeThisTime, int ropePreviousBest, float secondsThisTime, float secondsPreviousBest)
    {
        if (ropeThisTime < ropePreviousBest || ropePreviousBest == 0)
        {
            return true;
        }

        if (ropeThisTime == ropePreviousBest && secondsThisTime < secondsPreviousBest)
        {
            return true;
        }

        return false;
    }

    private void PlayBackgroundDependingOnLevel()
    {
        string bgmName = "";

        if (LevelInfoManager.instance.curLevel <= 20)
        {
            bgmName = "PlayBgmEasy1";
        }
        else if (LevelInfoManager.instance.curLevel <= 40)
        {
            bgmName = "PlayBgmNormal1";
        }
        else if (LevelInfoManager.instance.curLevel <= 50)
        {
            bgmName = "PlayBgmExpert1";
        }

        if (bgmName == string.Empty)
        {
            return;
        }

        AudioManager.instance.PlayBackground(bgmName);
    }
}