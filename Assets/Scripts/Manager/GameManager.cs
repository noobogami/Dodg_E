using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using GameAnalyticsSDK;
using TapsellSDK;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    internal int score;
    internal bool isPlaying;
    private bool canRevive;
    
    private float revivePanelTimer;
    internal bool reviveRequested;
    
    private float slowMotionTimer;
    private bool isSlowMotion;
    private float slowMotionDuration;
    private bool isReviving;
    
    internal int bestScore;
    internal bool purchased;
    internal int totalScore;
    private int lastAdScore;

    private int targetFrameRate = 60;


    void Awake()
    {
//        QualitySettings.vSyncCount = 0;
//        Application.targetFrameRate = targetFrameRate;
    }
    void Start()
    {
        GameAnalytics.Initialize();
        
        
        ViewManager.instance.ChangeViewToMainMenu();
        
        Init();
    }

    void Update()
    {
        if (isSlowMotion)
        {
            slowMotionTimer += Time.deltaTime;
            //print(slowMotionTimer);

            if (slowMotionTimer > 1)
            {
                StopSlowMotion();
            }
        }
//        if (Application.targetFrameRate != targetFrameRate)
//            Application.targetFrameRate = targetFrameRate;
    }

    private void Init()
    {
        instance = this;
        score = 0;
        isPlaying = false;
        //targetFrameRate = 40;
        PopupHandler.instance.HideMessage();
        TutorialManager.instance.ResetPanels();
        AudioManager.instance.StartMusic();

        GetPlayerPrefs();
        StatHandler.instance.SetStat("BEST SCORE", bestScore,false);
        StatHandler.instance.SetStat("TOTAL SCORE", totalScore,false);

        if (purchased)
        {
            ViewManager.instance.HidePurchaseBtn();
            AdHandler.instance.HideBannerAd();
        }
        else
        {
            ViewManager.instance.ShowPurchaseBtn();
            AdHandler.instance.ShowBannerAd();
        }
        
    }

    private void GetPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("First_Play"))
        {
            FirstPlay();
            return;
        }

        totalScore = PlayerPrefs.GetInt("Total_Score");
        lastAdScore = PlayerPrefs.GetInt("Last_Ad_Score");
        bestScore = PlayerPrefs.GetInt("Best_Score");
        purchased = PlayerPrefs.GetInt("Purchased") == 1;
    }

    private void UpdatePlayerPrefs()
    {
        PlayerPrefs.SetInt("Best_Score", bestScore);
        PlayerPrefs.SetInt("Total_Score", totalScore);
        PlayerPrefs.SetInt("Last_Ad_Score", lastAdScore);
        PlayerPrefs.SetInt("Purchased", purchased ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void FirstPlay()
    {
        PlayerPrefs.SetInt("First_Play", 0);
        PlayerPrefs.SetInt("High_Score", 0);
        PlayerPrefs.SetInt("Purchased", 0);
        PlayerPrefs.SetInt("Total_Scores", 0);
        PlayerPrefs.SetInt("Last_Ad_Score", 0);
        PlayerPrefs.Save();
    }

    public void PlayGame()
    {
        if (!PlayerPrefs.HasKey("TutorialPlayed"))
        {
            ShowTutorial();
            return;
        }
        score = 0;
        isPlaying = true;
        canRevive = true;
        reviveRequested = false;
        isReviving = false;
        GamePlayManager.instance.paused = false;
        ViewManager.instance.ChangeViewToGamePanel(score);
        GamePlayManager.instance.StartPlaying();
    }

    public void Collided()
    {
        if (isReviving) return;
        
        if (canRevive)
        {
            ShowRevivePanel();
        }
        else
        {
            ShowLostPanel();
        }
    }
    public void Pause()
    {
        if (!isPlaying) return;
        ViewManager.instance.ChangeViewToPausePanel(score);
        Debug.Log("Paused");
        isPlaying = false;
        GamePlayManager.instance.paused = true;
    }

    public void Resume()
    {
        isPlaying = true;
        GamePlayManager.instance.paused = false;
        ViewManager.instance.ChangeViewToGamePanel(score, true);

        if (isReviving) slowMotionDuration = 3;
        else slowMotionDuration = 1.5f;
        
        PlaySlowMotion();
    }

    private void PlaySlowMotion()
    {
        isSlowMotion = true;
        slowMotionTimer = 0;
        Time.timeScale = 1 / slowMotionDuration;
        ViewManager.instance.ShowCountDown();
    }

    private void StopSlowMotion()
    {
        Time.timeScale = 1;
        isSlowMotion = false;
        isReviving = false;
    }
    public void ShowRevivePanel()
    {
        if (!isPlaying) return;
        ViewManager.instance.ChangeViewToRevivePanel();
        Debug.Log("Paused");
        isPlaying = false;
        GamePlayManager.instance.paused = true;
        Invoke(nameof(ShowLostPanel), 3);
    }
    public void ShowLostPanel()
    {
        if (reviveRequested) return;
        CancelInvoke(nameof(ShowLostPanel));
        
        ViewManager.instance.ChangeViewToScorePanel();
        Debug.Log("You Lost");
        isPlaying = false;
        GamePlayManager.instance.paused = false;
        
        if ((bestScore < score) && (score != 0))
        {
            bestScore = score;
        }

        totalScore += score;
        
        if (totalScore - lastAdScore > 400 && totalScore != 0 && !purchased)
        {
            print($"Show Ad At Score: {lastAdScore}");
            lastAdScore = totalScore;
            AdHandler.instance.ShowInterstitialAd();
        }
        
        UpdatePlayerPrefs();
        
        StatHandler.instance.SetStat("BEST SCORE", bestScore,false);
        StatHandler.instance.SetStat("TOTAL SCORE", totalScore,false);
        StatHandler.instance.VisuallyUpdateStat();
    }

    internal void UpdateLastAdScoreValue()
    {
        lastAdScore = totalScore;
        UpdatePlayerPrefs();
    }
    public void RequestRevive()
    {
        reviveRequested = true;
        if (purchased)
        {
            ViewManager.instance.HideRevivePanel();
            CancelInvoke(nameof(ShowLostPanel));
            Revive();
            return;
        }
        AdHandler.instance.ShowReviveAd();
    }
    
    internal void Revive()
    {
        //GamePlayManager.instance.DecreaseDifficulty();
        //GamePlayManager.instance.DecreaseDifficulty();
        //GamePlayManager.instance.DecreaseDifficulty();
        
        GamePlayManager.instance.combo = 0;
        ViewManager.instance.SetFireIntensity(GamePlayManager.instance.combo, (GamePlayManager.instance.currentCombo + 1) * 10);
        ViewManager.instance.ChangeComboText(GamePlayManager.instance.combo);
        reviveRequested = false;
        PopupHandler.instance.HideMessage();
        canRevive = false;
        isReviving = true;
        Resume();
    }
    internal void IncreasePoint()
    {
        score += GamePlayManager.instance.combo + 1;
        ViewManager.instance.ChangeScore(score, true);
        if (bestScore < score)
        {
            ViewManager.instance.RecordBeaten();
        }
        print("score = " + score);
    }

    public void RemoveAd()
    {
        purchased = true;
        UpdatePlayerPrefs();
        ViewManager.instance.HidePurchaseBtn();
        AdHandler.instance.HideBannerAd();
    }

    public void ShowTutorial()
    {
        TutorialManager.instance.StartTutorial();
    }

    public void EndTutorial()
    {
        PlayerPrefs.SetInt("TutorialPlayed", 1);
        PlayerPrefs.Save();
        print("Here");
        PlayGame();
    }
}
