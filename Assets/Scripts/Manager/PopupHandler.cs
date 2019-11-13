using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    public static PopupHandler instance;
    
    public GameObject popupObject;
    public Text messageText;
    public Button btnOk, btnCancel;


    void Awake()
    {
        instance = this;
    }

    internal void ShowMessage(string message, bool hasOk = true, UnityAction btnOkFunction = null, bool hasCancel = false, UnityAction btnCancelFunction = null)
    {
        popupObject.SetActive(true);
        messageText.text = message;
        
        if (hasCancel)
        {
            btnCancel.gameObject.SetActive(true);
            btnCancel.onClick.AddListener(btnCancelFunction);
        }

        if (hasOk)
        {
            btnOk.gameObject.SetActive(true);
            btnOk.onClick.AddListener(btnOkFunction);
        }
    }

    public void HideMessage()
    {
        btnOk.onClick.RemoveAllListeners();
        btnCancel.onClick.RemoveAllListeners();
        popupObject.SetActive(false);
        btnOk.gameObject.SetActive(false);
        btnCancel.gameObject.SetActive(false);
    }
}
