using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class BoostWindow : MonoBehaviour
{
    public RectTransform window;
    public TMP_InputField inputField;
    public TextMeshProUGUI effectTextMesh;
    public TextMeshProUGUI title;

    //public GameObject placeholderIcon;
    public GameObject currencyIcon;

    private LevelElement triggeringLevelElement;

    List<Tuple<int, long>> rankings;
    private int amount;
    private int currentRank;
    private int newRank;

    private int currentAmountOfBananas;

    private string levelId;
    private string levelName;
    private int coinsInvested;
    private long createdOn;
    private System.Action<int> onComplete;

    private Tween delay;

    public void OnClickConfirm()
	{
        Close();

        if(amount == 0)
            return;

        DialogCanvas.instance.ShowLoading();

        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            BoostAPI.BoostLevel(levelId, amount, (success)=>
            {
                if(!success)
                {
                    DialogCanvas.instance.HideLoading();
                    return;
                }

                profile.coins -= amount;
                SaveManager.Save();   

                coinsInvested += amount;  

                CurrencyButton.instance?.UpdateCoins(profile.coins);

                BrowserView browserView = ((BrowserView) PopupCanvas.instance.browserView);
        
                if(browserView.boostedLoaded)
                {
                    browserView.boostedList.ClearLevels();
                }

                if(browserView.boostableLoaded)
                {
                    browserView.boostableLevels.ClearLevels();
                }
                
                browserView.boostableLoaded = false;
                browserView.boostedLoaded = false;


                delay = DOVirtual.DelayedCall(Time.deltaTime * 2, ()=>
                {
                    DialogCanvas.instance.HideLoading();

                    if(browserView.showMyBoostableLevels)
                    {
                        browserView.LoadLevelsForTab(BrowserTab.BoostableLevels);
                    }
                    else
                    {
                        browserView.LoadLevelsForTab(BrowserTab.Boosted);
                    }

                    onComplete?.Invoke(coinsInvested);
                });
            });
        });
    }

    public void IncreaseAmount()
    {
        int increase = 10;

        if(amount < 10)
            increase = 1;

        if(amount + increase < int.MaxValue)
            amount += increase;

        if(amount > currentAmountOfBananas)
            amount = currentAmountOfBananas;
        
        UpdateText();
    }

    public void DecreaseAmount()
    {
        int decrease = 10;

        if(amount <= 10)
            decrease = 1;

        if(amount - decrease >= 0)
            amount -= decrease;
        
        UpdateText();
    }

    private void UpdateText(bool updateInputField = true)
    {
        //Calculate new rank
        newRank = FindRank(coinsInvested + amount, createdOn);

        //placeholderIcon.SetActive(inputField.text.Length == 0);
        currencyIcon.SetActive(inputField.text.Length > 0);

        if(updateInputField)
            inputField.text = amount.ToString();

        effectTextMesh.TranslateFormat(TranslationKey.Boost_EffectDescription, SaveManager.currentSave.language, FontType.Stylized, true, new object[]{ amount, currentRank, newRank });
    }

    public void OnValueChanged(string value)
    {
        if(!string.IsNullOrWhiteSpace(value))
            amount = int.Parse(value);
        else
            amount = 0;

        UpdateText(false);
    }

    public void Show(string levelId, int coinsInvested, string levelName, long createdOn, System.Action<int> onComplete)
    {
        this.levelId = levelId;
        this.coinsInvested = coinsInvested;

        this.levelName = levelName;
        this.createdOn = createdOn;
        this.onComplete = onComplete;

        DialogCanvas.instance.ShowLoading();

        SaveManager.currentSave.FetchOnlineProfile((profile)=>{
            currentAmountOfBananas = profile.coins;

            BoostAPI.FetchRankings((success, rankins)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                    return;

                
                this.rankings = rankins.Rankings;

                //Calculate positon
                currentRank = FindRank(coinsInvested, createdOn);

                title.TranslateFormat(TranslationKey.Boost_Title, SaveManager.currentSave.language, FontType.Stylized, true, new object[]{ levelName });

                amount = 10;

                inputField.text = amount.ToString();

                UpdateText();

                window.DOKill();
                window.anchoredPosition = new Vector3(0, -400f, 0);

                Canvas.ForceUpdateCanvases();

                window.gameObject.SetActive(true);
                window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);
            });
        });
    }

    public void Show(LevelElement levelElement, System.Action<int> onComplete)
    {
        this.triggeringLevelElement = levelElement;
        Show( levelElement.GetLevel().LevelId, levelElement.GetLevel().CoinsInvested, levelElement.GetLevel().Name, levelElement.GetLevel().CreatedOn.Ticks, onComplete );
    }

	public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
	}

    private int FindRank(int amountInvested, long createdOn)
    {
        for(int i = 0; i < rankings.Count; i++)
        {
            //Starting at the beginning:
            //Skip every level thats boosted more than the one we are looking for.
            if(amountInvested < rankings[i].Item1)
                continue;

            //If the amount invested is the same as the one we are looking for.
            if(amountInvested == rankings[i].Item1)
            {
                //If this level or identical to this level.
                if(rankings[i].Item2 == createdOn)
                {
                    return i+1;
                }

                //Skip every level older than this one.
                if(rankings[i].Item2 < createdOn)
                    continue;
            }
            
            return i+1;
        }

        return rankings.Count+1;
    }

    private void OnDestroy()
    {
        delay?.Kill();
        delay = null;
    }
}
