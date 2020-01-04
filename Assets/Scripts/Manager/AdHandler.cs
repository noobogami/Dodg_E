using System;
using System.Collections;
using System.Collections.Generic;
using TapsellSDK;
using UnityEngine;
using UnityEngine.AI;

public class AdHandler : MonoBehaviour
{
    internal static AdHandler instance;
    //public int coinAdWaitTime;

    private string ad_Zone_Revive = "5da1ce3017ec1e00013a1283";
    private string ad_Zone_Interstitial = "5da1ce3017ec1e00013a1284";
    private string bannerZoneId = "5da1ce3017ec1e00013a1285";


    private void Awake()
    {
        instance = this;
        Init();
    }

    public void Init()
    {
        Tapsell.Initialize("sfpgmkqcrjkmmarbnajpsknqnlihbrnglogspjfjnlasibqiatjihgtrkhmsfitddedglh");
        Tapsell.SetRewardListener(AdReward);
    }

    internal void ShowBannerAd()
    {
        Tapsell.RequestBannerAd (bannerZoneId,BannerType.BANNER_320x50, Gravity.TOP, Gravity.CENTER,
            (string zoneId)=>{
                Debug.Log("Action: onBannerRequestFilledAction");
                Tapsell.ShowBannerAd(zoneId);
            },
            (string zoneId)=>{
                Debug.Log("Action: onNoBannerAdAvailableAction");
            },
            (TapsellError tapsellError)=>{
                Debug.Log("Action: onBannerAdErrorAction");
            },
            (string zoneId)=>{
                Debug.Log("Action: onNoNetworkAction");
            }, 
            (string zoneId)=>{
                Debug.Log("Action: onHideBannerAction");
            });
    }

    internal void HideBannerAd()
    {
        Tapsell.HideBannerAd(bannerZoneId);
    }
    
    public void RequestCoinAd()
    {
        /*if (!PlayerPrefs.HasKey("lastAdWatched"))
        {
            ShowCoinAd();
            CoinAdWatched();
            return;
        }


        long lastTimePlayed = long.Parse(PlayerPrefs.GetString("lastAdWatched"));
        if (lastTimePlayed == -1)
        {
            ShowCoinAd();
            CoinAdWatched();
            return;
        }

        long secondPassed = TimeManager.instance.Now() - lastTimePlayed;
        if (secondPassed < coinAdWaitTime)
        {
            PopupHandler.ShowDebug(string.Format("برای نمایش تبلیغ {0} دیگر منتظر بمانید",
                Utilities.GetTimeFormat(coinAdWaitTime - secondPassed)));
            return;
        }

        ShowCoinAd();
        CoinAdWatched();*/
    }

    private void CoinAdWatched()
    {
        /*PlayerPrefs.SetString("lastAdWatched", "" + TimeManager.instance.Now());
        PlayerPrefs.Save();
        AnalyticsHandler.AdWatched();*/
    }


    public void ShowReviveAd()
    {
        ViewManager.instance.ShowLoading();
        Tapsell.RequestAd(
            ad_Zone_Revive,
            false,
            AdAvailable,
            ReviveAdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire,
            Open,
            Close);
    }

    public void ShowInterstitialAd()
    {
        ViewManager.instance.ShowLoading();
        Tapsell.RequestAd(
            ad_Zone_Interstitial,
            false,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire,
            Open,
            Close);
    }

    public void ShowPrizeAd()
    {
        /*ShowLoading();
        Tapsell.requestAd(
            ad_zone_winDoublePrize,
            false,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire);*/
    }

    public void ShowKeyAd()
    {
        /*ShowLoading();
        Tapsell.requestAd(
            ad_zone_key,
            false,
            AdAvailable,
            AdNotAvailable,
            AdError,
            AdNetworkNotAvailable,
            AdExpire);*/
    }

    #region Tapsell Actions

    private void AdAvailable(TapsellAd result)
    {
        ViewManager.instance.HideRevivePanel();
        ViewManager.instance.HideLoading();
        TapsellAd ad = result;
        TapsellShowOptions options = new TapsellShowOptions();
        options.backDisabled = true;
        options.showDialog = true;
        Tapsell.ShowAd(ad, options);
    }

    private void ReviveAdNotAvailable(string zoneId)
    {
        ViewManager.instance.HideRevivePanel();
        ViewManager.instance.HideLoading();

        if (zoneId == ad_Zone_Interstitial) return;
        
        PopupHandler.instance.ShowMessage("ERROR 3:\nAT THIS TIME THERE IS NO AD TO SHOW YOU", true, PopupHandler.instance.HideMessage);
        GameManager.instance.reviveRequested = false;
        GameManager.instance.ShowLostPanel(); //TODO: Delete this shit!
        
        Debug.LogError($"[AdHandler] Ad Not Available for {zoneId}");
        //PopupHandler.ShowDebug("در حال حاضر تبلیغی برای نمایش وجود نداره.");
    }
    private void AdNotAvailable(string zoneId)
    {
        ViewManager.instance.HideRevivePanel();
        ViewManager.instance.HideLoading();

        if (zoneId == ad_Zone_Interstitial) return;
        
        PopupHandler.instance.ShowMessage("ERROR 3:\nAT THIS TIME THERE IS NO AD TO SHOW YOU", true, PopupHandler.instance.HideMessage);
        GameManager.instance.ShowLostPanel(); //TODO: Delete this shit!
        
        Debug.LogError($"[AdHandler] Ad Not Available for {zoneId}");
        //PopupHandler.ShowDebug("در حال حاضر تبلیغی برای نمایش وجود نداره.");
    }

    private void AdError(TapsellError error)
    {
        ViewManager.instance.HideRevivePanel();
        ViewManager.instance.HideLoading();
        
        if (error.zoneId == ad_Zone_Interstitial) return;
        
        PopupHandler.instance.ShowMessage("ERROR 4:\nTHERE IS A PROBLEM WITH AD SERVICE", true, PopupHandler.instance.HideMessage);
        GameManager.instance.ShowLostPanel(); //TODO: Delete this shit!
        
        Debug.LogError($"[AdHandler] Ad Error! {error.message}");
        //PopupHandler.ShowDebug("نمایش تبلیغ از سمت نمایش دهنده با مشکل مواجه شد");
    }

    private void AdNetworkNotAvailable(string zoneId)
    {
        ViewManager.instance.HideRevivePanel();
        ViewManager.instance.HideLoading();
        
        PopupHandler.instance.ShowMessage("ERROR 2:\nWE CAN'T GET AD PLEASE CHECK YOUR INTERNET CONNECTION", true, PopupHandler.instance.HideMessage);
        
        GameManager.instance.ShowLostPanel(); //TODO: Delete this shit!
        
        Debug.LogError($"[AdHandler] No Internet! {zoneId}");
        //PopupHandler.ShowDebug("ارتباط اینترنت برقرار نشد لطفا اتصال خودتون رو بررسی کنید.");
    }

    private void AdExpire(TapsellAd result)
    {   
        Debug.LogError($"[AdHandler] Expired! {result.zoneId}");
    }
    
    private void Close(TapsellAd obj)
    {
        ViewManager.instance.HideRevivePanel();
        ViewManager.instance.HideLoading();
    }

    private void Open(TapsellAd obj)
    {
        ViewManager.instance.HideRevivePanel();
        ViewManager.instance.HideLoading();
    }

    #endregion


    private void AdReward(TapsellAdFinishedResult result)
    {
        if (result.rewarded && result.completed)
        {
            if (result.zoneId == ad_Zone_Revive)
            {
                //PopupHandler.ShowDebug("تعداد سکه های دریافت شده دو برابر شد!");
                 
                PopupHandler.instance.ShowMessage("NOW YOU CAN PLAY MORE", true, GameManager.instance.Revive);
            }
            if (result.zoneId == ad_Zone_Interstitial)
            {
                GameManager.instance.UpdateLastAdScoreValue();
            }
        }
        else if (result.zoneId == ad_Zone_Revive)
        {
            PopupHandler.instance.ShowMessage("ERROR 1:\nYOU LOST YOUR CHANCE FOR REVIVE BY CLOSING THE AD", true, PopupHandler.instance.HideMessage);
        
            GameManager.instance.ShowLostPanel(); //TODO: Delete this shit!
        }
    }
}