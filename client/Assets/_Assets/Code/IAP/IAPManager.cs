/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public class IAPManager : ImmortalSingleton<IAPManager>, IStoreListener 
{
    public IStoreController controller;
    public IExtensionProvider extensions;

    private IAPManagerState state;

    public delegate void PurchaseComplete(PurchaseEventArgs args);
    public PurchaseComplete On_PurchaseComplete;
    public delegate void PurchaseFailed(Product product, PurchaseFailureReason reason);
    public PurchaseFailed On_PurchaseFailed;

    public IAPManagerState GetState()
    {
        return state;
    }

    protected void Start() 
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach(string iAPId in Globals.iAPConstants.characterIds)
        {
            builder.AddProduct(iAPId, ProductType.NonConsumable);
        }

        UnityPurchasing.Initialize (this, builder);
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;

        state = IAPManagerState.Initalized;
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed (InitializationFailureReason error)
    {
        Debug.LogError(error.ToString());

        state = IAPManagerState.InitializeFailed;
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
    {
        On_PurchaseComplete?.Invoke(e);

        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed (Product product, PurchaseFailureReason reason)
    {
        On_PurchaseFailed?.Invoke(product, reason);
        Debug.LogError(reason.ToString());

        new Dialog("Purchase Failed", false, Globals.localizationConstants.defaultLanguage, reason.ToString(), false, Globals.localizationConstants.defaultLanguage)
        .AddNeutralButton(TranslationKey.Generic_Ok, null)
        .Show();
    }

    public void RestorePurchases()
    {
        Debug.Log("Restore Purchases...");

        if(state != IAPManagerState.Initalized)
        {
            Debug.LogError("iAPManager not initalized!");
            return;
        }

        if(Application.isEditor)
            return;

        DialogCanvas.instance.ShowLoading();

#if UNITY_IOS
        extensions.GetExtension<IAppleExtensions> ().RestoreTransactions (result => 
        {
            DialogCanvas.instance.HideLoading();
            if (result) {
                // This does not mean anything was restored,
                // merely that the restoration process succeeded.

                Debug.Log("Restore complete!");
            } else {
                // Restoration failed.
                Debug.LogError("Restoration failed");
            }
        });
#elif UNITY_ANDROID
        extensions.GetExtension<IGooglePlayStoreExtensions> ().RestoreTransactions (result => 
        {
            DialogCanvas.instance.HideLoading();
            if (result) {
                // This does not mean anything was restored,
                // merely that the restoration process succeeded.

                Debug.Log("Restore complete!");
            } else {
                // Restoration failed.
                Debug.LogError("Restoration failed");
            }
        });
#endif
    }

    public enum IAPManagerState
    {
        Uninitalized,
        Initalized,
        InitializeFailed
    }
}
*/