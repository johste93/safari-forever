using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.Purchasing;
using System.Linq;

public class Shop : MonoBehaviour
{   
    public Carrossell carrossell;
    public ShopCanvas shopCanvas;
    public MenuCanvas menuCanvas;

    public delegate void ShopEvent();
    public static ShopEvent On_CharacterSelected;

    private Dictionary<string, int> bananaPrices; //Key is Animal Index.

    public RectTransform defaultIcon;
    public RectTransform unlockIcon;
    public RectTransform notAvailable;
    public TextMeshProUGUI bananaPriceTextMesh;
    public TextMeshProUGUI iAPPriceTextMesh;
    public TextMeshProUGUI adoptButtonTextMesh;
    public HorizontalLayoutGroup bananaPriceHorizontalLayoutGroup;

    public GameObject bananaIcon;
    public GameObject separator;
    public RectTransform randomCharacterOption;
    public PushToggle randomCharacterToggle;

    public RectTransform randomHatOption;
    public PushToggle randomHatToggle;
    public RectTransform bananaPriceParent;
    public RectTransform iAPPriceParent;

    public void OnToggleRandom(bool useRandom)
    {
        SaveManager.currentSave.useRandomCharacter = useRandom;
        SaveManager.Save();
    }

    public void OnToggleRandomHat(bool useRandom)
    {
        SaveManager.currentSave.useRandomHat = useRandom;
        SaveManager.Save();
    }

    public void Initalize(System.Action<bool> onComplete)
    {
        if(bananaPrices != null)
        {
            onComplete?.Invoke(true);
            return;
        }

        MusicManager.DoFade(Globals.musicConstants.defaultVolume * 0.25f, 0.1f);
        
        randomCharacterOption.gameObject.SetActive(true);
        randomCharacterToggle.SetValue(SaveManager.currentSave.useRandomCharacter, true);

        randomHatOption.gameObject.SetActive(true);
        randomHatToggle.SetValue(SaveManager.currentSave.useRandomHat, true);

        bool failSilently = false;
        TransactionAPI.GetPrices((success, prices)=>
        {
            DialogCanvas.instance.HideLoading();
            randomCharacterOption.GetComponentInChildren<TextMeshProUGUI>().ForceMeshUpdate();
            randomHatOption.GetComponentInChildren<TextMeshProUGUI>().ForceMeshUpdate();

            if(!success)
            {
                onComplete?.Invoke(false);
                return;
            }

            this.bananaPrices = prices;

            onComplete?.Invoke(true);
        }, failSilently);
    }

    public void Exit()
    {
        if(SaveManager.currentSave.currentCharacter != carrossell.GetCurrentIndex())
            carrossell.SetIndex(SaveManager.currentSave.currentCharacter);
            
        SaveAndClose();
    }

    public void Select()
    {
        int index = carrossell.GetCurrentIndex();
        if(!SaveManager.currentSave.unlockedCharacter[index])
        {
            Animal animal = ((Animal)index);
            int price = -1;
            
            if(bananaPrices.ContainsKey(animal.ToString()))
                price = bananaPrices[animal.ToString()];

            string name = animal.ToString();

            if(price < 0)
            {
                //This character is not for sale.
                if(price < -1)
                {
                    //No longer available
                    new Dialog(
                        TranslationKey.Shop_Adopt_NoLongerAvailable_Title,
                        TranslationKey.Shop_Adopt_NoLongerAvailable_Body)
                    .AddNegativeButton(TranslationKey.Generic_Ok, null)
                    .Show();
                }
                else
                {
                    //Not available
                    new Dialog(
                        TranslationKey.Shop_Adopt_NotAvailable_Title,
                        TranslationKey.Shop_Adopt_NotAvailable_Body)
                    .AddNegativeButton(TranslationKey.Generic_Ok, null)
                    .Show();
                }

                return;
            }

            bool titleKeyAvailable = Localization.KeyAvailable(TranslationKey.Shop_Adopt_Title, SaveManager.currentSave.language);
            Language titleLanguage = titleKeyAvailable ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
            bool titleIsRTL = Localization.IsRightToLeftLanguage(titleLanguage);
            string title = Localization.GetTranslationFormat2(TranslationKey.Shop_Adopt_Title, SaveManager.currentSave.language, titleIsRTL ? $"<ltr>{name}</ltr>" : name );

            bool bodyKeyIsAvailable = false;
            Language bodyLanguage;
            bool bodyIsRTL = false;
            string body = "";
            if(animal == Animal.Pingo)
            {
                bodyKeyIsAvailable = Localization.KeyAvailable(TranslationKey.Shop_Pingo_Body, SaveManager.currentSave.language);
                bodyLanguage = bodyKeyIsAvailable ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
                bodyIsRTL = Localization.IsRightToLeftLanguage(bodyLanguage);
                body = Localization.GetTranslation2(TranslationKey.Shop_Pingo_Body, SaveManager.currentSave.language);
            }
            else
            {
                bodyKeyIsAvailable = Localization.KeyAvailable(TranslationKey.Shop_Adopt_Body, SaveManager.currentSave.language);
                bodyLanguage = bodyKeyIsAvailable ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
                bodyIsRTL = Localization.IsRightToLeftLanguage(bodyLanguage);
                body = Localization.GetTranslationFormat2(TranslationKey.Shop_Adopt_Body, SaveManager.currentSave.language, new string[]{price.ToString(), name});
            }

            string bananaPrice = $"<sprite name=\"Bananas_0\" color=#434343FF>{bananaPrices[((Animal)carrossell.GetCurrentIndex()).ToString()]}";

            if(price == 0)
                bananaPrice = Localization.GetTranslation2(TranslationKey.Shop_Adopt_Free, SaveManager.currentSave.language);
           
            //Unlock?
            Dialog dialog = new Dialog(
                title, titleIsRTL, titleLanguage,
                body, bodyIsRTL, bodyLanguage)
                .AddNegativeButton(TranslationKey.Generic_Cancel, null)
                .AddPositiveButton(bananaPrice, SaveManager.currentSave.language, ()=>
                {
                    TryBuyWithBananas(animal);
                }, false, false);

            /*
            if(IAPManager.instance != null && price > 0)
            {
                Product product = IAPManager.instance.controller.products.WithID(IAPIds.GetIAPIdByCharacter(animal));
                if(product != null && product.availableToPurchase && IAPManager.instance.GetState() == IAPManager.IAPManagerState.Initalized)
                {
                    string iAPPrice = product.metadata.localizedPriceString;
                    dialog.AddPositiveButton(iAPPrice, SaveManager.currentSave.language, ()=>
                    {
                        TryBuyWithIAP(animal);
                    }, false, false);
                }
            }
            */

            dialog.Show();

            return;
        }

        SaveAndClose();
    }

    private void TryBuyWithIAP(Animal animal)
    {
        /*
        DialogCanvas.instance.ShowLoading();

        IAPPurchases.instance.On_PurchaseComplete = (success)=>
        {
            IAPPurchases.instance.On_PurchaseComplete = null;
            DialogCanvas.instance.HideLoading();

            if(!success)
                return;

            if(carrossell.currentCage != null)
                carrossell.currentCage.SetActive(false);

            SaveAndClose();
        };

        IAPManager.instance.controller.InitiatePurchase(IAPIds.GetIAPIdByCharacter(animal));
        */
    }

    private void TryBuyWithBananas(Animal animal)
    {
        TransactionAPI.UnlockCharacter(animal, (success, response)=>
        {
            DialogCanvas.instance.HideLoading();
            if(!success)
                return;

            if(!response.PurchaseSuccessful)
            {
                switch(response.PurchaseError)
                {
                    case PurchaseError.CantAfford:
                        new Dialog(
                            TranslationKey.Shop_Adopt_CantAfford_Title,
                            TranslationKey.Shop_Adopt_CantAfford_Body)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null)
                        .Show();
                    return;
                    case PurchaseError.NotAvailable:
                        new Dialog(
                            TranslationKey.Shop_Adopt_NotAvailable_Title,
                            TranslationKey.Shop_Adopt_NotAvailable_Body)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null)
                        .Show();
                    return;
                    case PurchaseError.AlreadyPurchased:
                        new Dialog(
                            TranslationKey.Generic_Error,
                            TranslationKey.Shop_Adopt_AlreadyPurchased_Body)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null)
                        .Show();
                    return;
                }
            }

            //Unlock
            SaveManager.currentSave.FetchOnlineProfile((profile)=>
            {
                profile.coins = response.RemainingCoins;

                SaveManager.currentSave.unlockedCharacter[(int)animal] = true;
                if(carrossell.currentCage != null)
                    carrossell.currentCage.SetActive(false);

                SaveAndClose();
            });
        });
    }

    private void SaveAndClose()
    {
        SaveManager.currentSave.currentCharacter = carrossell.GetCurrentIndex();
        SaveManager.Save();

        if(On_CharacterSelected != null)
            On_CharacterSelected();
        
        shopCanvas.Hide();
        menuCanvas.Show();
        MusicManager.DoFade(Globals.musicConstants.defaultVolume, 0.1f);
    }

    private void On_CharacterChanged()
    {
        UpdateButton(SaveManager.currentSave.unlockedCharacter[carrossell.GetCurrentIndex()], (Animal) carrossell.GetCurrentIndex()); 
        //UpdateButton(false, (Animal) carrossell.GetCurrentIndex());
    }

    private void UpdateButton(bool isUnlocked, Animal animal)
    {
        int price = -1;

        if(bananaPrices.ContainsKey(((Animal)carrossell.GetCurrentIndex()).ToString()))
            price = bananaPrices[((Animal)carrossell.GetCurrentIndex()).ToString()];

        defaultIcon.gameObject.SetActive(false);
        unlockIcon.gameObject.SetActive(false);
        notAvailable.gameObject.SetActive(false);
        bananaIcon.SetActive(false);
        separator.SetActive(false);
        iAPPriceParent.gameObject.SetActive(false);

        if(isUnlocked)
        {
            defaultIcon.gameObject.SetActive(true);
        }
        else
        {
            if(price >= 0)
            {
                adoptButtonTextMesh.TranslateFormat(TranslationKey.Shop_Adopt_Title, SaveManager.currentSave.language, FontType.Stylized, false, animal.ToString());
                unlockIcon.gameObject.SetActive(true);
            }
            else
            {
                //Not for sale
                notAvailable.gameObject.SetActive(true);
            }
        }    
    }

    private void OnEnable()
    {
        carrossell.On_CharacterChanged += On_CharacterChanged;
        On_CharacterChanged();
    }

    private void Unsubscribe()
    {
        carrossell.On_CharacterChanged -= On_CharacterChanged;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Update()
    {
        if(!Application.isEditor)
            return;

        if(Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot($"{((Animal) carrossell.GetCurrentIndex())}.png");
            Debug.Log("Saved to: " + Application.persistentDataPath);
        }
    }
}
