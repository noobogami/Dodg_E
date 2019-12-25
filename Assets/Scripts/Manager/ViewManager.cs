using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    public GameObject mainMenuPanel, gamePanel, adPanel, shopPanel, scorePanel, pausePanel, popupPanel, loading, revivePanel, countDown;

    public Button btnPurchase;

    [Space(10)] public Text stat_Highscore;
    public Text stat_Time;
    public Text stat_MaxCombo;
    public Text stat_WithoutTouch;
    public Text stat_TotalScore;

    [Space(10)] public Text combo;
    [Space(10)] public Text[] scoreTexts;
    public GameObject[] highscoreTags;
    
    internal static ViewManager instance;


    void Awake()
    {
        instance = this;
    }
    
    internal void HidePurchaseBtn()
    {
        btnPurchase.gameObject.SetActive(false);
    }

    internal void ShowPurchaseBtn()
    {
        btnPurchase.gameObject.SetActive(true);
    }

    public void ChangeScore(int score, bool withAnimation = false)
    {
        foreach (var txt in scoreTexts)
        {
            if (withAnimation)
            {
                try
                {
                    txt.GetComponent<Animator>().SetTrigger("Blob");
                }
                catch (Exception e)
                {
                }
            }
            
            txt.text = "" + score;
        }
    }

    public void RecordBeaten()
    {
        foreach (var obj in highscoreTags)
        {
            obj.SetActive(true);
        }
    }

    public void ChangeViewToGamePanel(int score, bool resume = false)
    {
        mainMenuPanel.SetActive(false);
        scorePanel.SetActive(false);
        pausePanel.SetActive(false);
        ChangeScore(score);
        if (resume) return;
        gamePanel.SetActive(true);
        HideHighScoreTag();
    }

    public void ChangeViewToMainMenu()
    {
        ResetAll();
        mainMenuPanel.SetActive(true);
    }
    
    public void ChangeViewToAdPanel()
    {
        ResetAll();
        adPanel.SetActive(true);
    }

    public void ChangeViewToShopPanel()
    {
        ResetAll();
        shopPanel.SetActive(true);
    }

    public void ChangeViewToScorePanel()
    {
        HideRevivePanel();
        scorePanel.SetActive(true);
        ChangeScore(GameManager.instance.score);
    }
    public void ChangeViewToPausePanel(int score)
    {
        pausePanel.SetActive(true);
        ChangeScore(score);
    }
    public void ChangeViewToRevivePanel()
    {
        revivePanel.SetActive(true);
    }
    public void HideRevivePanel()
    {
        revivePanel.SetActive(false);
    }

    public void ShowLoading()
    {
        loading.SetActive(true);
    }
    public void HideLoading()
    {
        loading.SetActive(false);
    }

    public void ShowCountDown()
    {
        countDown.SetActive(true);
    }

    public void HideCountDown()
    {
        countDown.SetActive(false);
    }
    public void ResetAll()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(false);
//        adPanel.SetActive(false);
//        shopPanel.SetActive(false);
        scorePanel.SetActive(false);
        pausePanel.SetActive(false);
        popupPanel.SetActive(true);
        revivePanel.SetActive(false);
        HideAllPopups();

        HideHighScoreTag();
        ResetScoreTexts();
    }


    public void HideAllPopups()
    {
        for (int i = 0; i < popupPanel.transform.childCount; i++)
        {
            popupPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void ResetScoreTexts()
    {
        foreach (var txt in scoreTexts)
        {
            txt.text = "0";
        }
    }
    public void HideHighScoreTag()
    {foreach (var obj in highscoreTags)
        {
            obj.SetActive(false);
        }
    }


    public void ChangeComboText(int value)
    {
        if (value != 0)
        {
            combo.gameObject.SetActive(true);
            combo.text = "" + (value + 1);
            return;
        }
        combo.gameObject.SetActive(false);
    }


    [Header("Fire")] 
    public ParticleSystem psFireSpark;
    public ParticleSystem psFireGlow;

    private float emission_start= 0.3f, emission_end = 1;
    private float m_life_min_start = 1, m_life_min_end = 3, m_life_max_start = 3, m_life_max_end = 5;
    private float m_speed_min_start = 1, m_speed_min_end = 5, m_speed_max_start = 5, m_speed_max_end = 10;
    private float m_size_min_start = 0, m_size_min_end = 0.2f, m_size_max_start = 0.2f, m_size_max_end = 0.5f;
    
    internal void SetFireIntensity(int combo, int max)
    {
        float p = Mathf.Clamp(combo * 1.0f / max, 0, 1); 
        
        var emission = psFireGlow.emission;
        emission.rateOverTime = p * (emission_end - emission_start) + emission_start;

        var main = psFireSpark.main;
        var m_life = main.startLifetime;
        
        m_life.constantMin = p * (m_life_min_end - m_life_min_start) + m_life_min_start;
        m_life.constantMax = p * (m_life_max_end - m_life_max_start) + m_life_max_start;

        main.startLifetime = m_life;
        
        var m_speed = main.startSpeed;
        m_speed.constantMin = p * (m_speed_min_end - m_speed_min_start) + m_speed_min_start;
        m_speed.constantMax = p * (m_speed_max_end - m_speed_max_start) + m_speed_max_start;

        main.startSpeed = m_speed;
        
        
        var m_size = main.startSize;
        m_size.constantMin = p * (m_size_min_end - m_size_min_start) + m_size_min_start;
        m_size.constantMax = p * (m_size_max_end - m_size_max_start) + m_size_max_start;
        
        main.startSize = m_size;
    }

    public GameObject aboutUsPanel;
    public void ShowAboutUs()
    {
        aboutUsPanel.SetActive(true);
    }
    public void HideAboutUs()
    {
        aboutUsPanel.SetActive(false);
    }

    /*internal void DecreaseFireIntensity(bool reset = false)
    {
        var m_speed = main.startSpeed;
        m_speed.constantMin += 0.12f;
        m_speed.constantMax += 0.3f;

        if (reset || m_speed.constantMax < 5)
        {
            m_speed.constantMin = 1;
            m_speed.constantMax = 5;
        }

        main.startSpeed = m_speed;
        
        
        var m_size = main.startSize;
        m_size.constantMin += 0.006f;
        m_size.constantMax += 0.009f;
        
        if (reset || m_size.constantMax < 0.2)
        {
            m_size.constantMin = 0f;
            m_size.constantMax = 0.2f;
        }
        
        main.startSize = m_size;
    }*/
}
