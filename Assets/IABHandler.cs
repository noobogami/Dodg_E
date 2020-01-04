using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BazaarPlugin;

public class IABHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BazaarIAB.init("MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwDMbImiv5V94ZoYpq3GUBbJvpfX/e8fYy2iaUMYQAx4doU/m9MoVWM+d9ooklMPHFWtrCGcAfAvARFrY4qfHgb4sWmGh1gsS+ctbKylOgVVbpnjNC7rOyFQgyPmHNAgr2HcFAtTepWWIAjbVRpMFVpKJkqDPS92FyCbW4Sa18HOSbp7nsA31OUELsVf89lRn4Mpzrd5FQbfw/pxv4xgCn5gZFQROQcmmWl5mNpOEnECAwEAAQ==");
        
        
//        IABEventManager.billingSupportedEvent += billingSupportedEvent;
//        IABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
//        IABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
//        IABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
//        IABEventManager.querySkuDetailsSucceededEvent += querySkuDetailsSucceededEvent;
//        IABEventManager.querySkuDetailsFailedEvent += querySkuDetailsFailedEvent;
//        IABEventManager.queryPurchasesSucceededEvent += queryPurchasesSucceededEvent;
//        IABEventManager.queryPurchasesFailedEvent += queryPurchasesFailedEvent;
        IABEventManager.purchaseSucceededEvent += PurchaseSucceeded;
//        IABEventManager.purchaseFailedEvent += purchaseFailedEvent;
        IABEventManager.consumePurchaseSucceededEvent += ConsumeSucceeded;
//        IABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
    }

    public void RemoveAdRequest()
    {
        print("Remove Ad Requested");
        BazaarIAB.purchaseProduct("Remove_Ad");
    }

    private void PurchaseSucceeded(BazaarPurchase result)
    {
        print("Remove Ad Purchased");
        BazaarIAB.consumeProduct(result.ProductId);
    }

    private void ConsumeSucceeded(BazaarPurchase result)
    {
        print("Remove Ad Consumed");
        GameManager.instance.RemoveAd();
    }

    
}
