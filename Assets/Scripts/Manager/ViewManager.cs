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

}
