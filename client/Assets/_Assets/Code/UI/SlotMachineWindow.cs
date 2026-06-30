using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;


public class SlotMachineWindow : MonoBehaviour
{
    public TextMeshProUGUI remaining;

    public SlotMachine slotMachine;
    public UIButton cancelButton;

    public RectTransform window;
    public TMP_InputField inputField;

    private int currentAmountOfBananas;

    private System.Action onComplete;

    private Tween delay;

    private int amount;

    private int minimumBet;
    private int maximumBet;

    public void Spinn()
	{
        if(!slotMachine.IsReady())
            return;

        if(!slotMachine.crank.interactable)
            return;

        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
                return;

            //Check if we can afford the amount specified.
            if(amount > currentAmountOfBananas)
            {
                HandleError(SlotMachineError.CantAfford);
                return;
            }

            //check if we already have all hats
            int totalNumberOfHats = System.Enum.GetNames(typeof(Hat)).Length-1;
            int numberOfUnlockedHats = SaveManager.currentSave.unlockedHats.Where(x => x == true).Count()-1;

            if(numberOfUnlockedHats >= totalNumberOfHats)
            {
                HandleError(SlotMachineError.NoMoreRewards);
                return;
            }

            cancelButton.SetInteractable(false);
            slotMachine.Spinn(()=>
            {
                SlotMachineAPI.Spinn(amount, (success, response)=>{
                    
                    if(!success)
                    {
                        cancelButton.SetInteractable(true);
                        foreach(Reel reel in slotMachine.reels)
                            reel.SetSymbol((Symbol) Random.Range(0, System.Enum.GetNames(typeof(Symbol)).Length));
                        
                        return;
                    }

                    if(response.Error != SlotMachineError.None)
                    {
                        HandleError(response.Error);

                        cancelButton.SetInteractable(true);
                        foreach(Reel reel in slotMachine.reels)
                            reel.SetSymbol((Symbol) Random.Range(0, System.Enum.GetNames(typeof(Symbol)).Length));

                        return;
                    }

                    //Update remaining coins       
                    profile.coins -= amount;
                    UpdateCoins(profile.coins);

                    if(amount > currentAmountOfBananas)
                        amount = Mathf.Max(currentAmountOfBananas, minimumBet);

                    inputField.text = amount.ToString();
                    slotMachine.crank.interactable = currentAmountOfBananas >= amount;

                    if(response.DidWin)
                    {
                        SaveManager.currentSave.unlockedHats[(int)response.Reward] = true;
                        slotMachine.Win(()=>{
                            Audio.Play(SFX.instance.ui.slotMachine.win, Channel.UI);
                            cancelButton.SetInteractable(true);
                            Close();
                            PresentReward(response.Reward);
                        });
                    }
                    else
                    {
                        slotMachine.Loose(()=>{
                            Audio.Play(SFX.instance.ui.slotMachine.loose, Channel.UI);
                            cancelButton.SetInteractable(true);
                        });
                    }  
                });
            });
        });
    }

    private void UpdateCoins(int coins)
    {
        currentAmountOfBananas = coins;
        bool usedLanguageIsRightToLeft = Localization.IsRightToLeftLanguage(Localization.KeyAvailable(TranslationKey.Lottery_RemainingBananas, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage);
        remaining.TranslateFormat(TranslationKey.Lottery_RemainingBananas, SaveManager.currentSave.language, FontType.Stylized, false, usedLanguageIsRightToLeft ? $"<ltr>{coins.ToString()}</ltr>" : coins.ToString());

        CurrencyButton.instance?.UpdateCoins(coins);
    }

    private void PresentReward(Hat hat)
    {
        ((RewardsView)PopupCanvas.instance.rewardsView).Initalise(new List<UnrecivedReward>(){ 
			new UnrecivedReward() {
				TransactionType = TransactionType.WonHat,
				Hat = hat
			}
		});

		PopupCanvas.instance.rewardsView.Show(()=>
		{
			if(!PopupCanvas.instance.HasActiveViews(PopupCanvas.instance.rewardsView))
			{
				PopupCanvas.instance.CloseCanvas();
				onComplete?.Invoke();
			}
		});
    }

    public void IncreaseAmount()
    {
        int increase = 100;

        amount += increase;

        if(amount > maximumBet)
            amount = maximumBet;

        if(amount > currentAmountOfBananas)
            amount = Mathf.Max(currentAmountOfBananas, minimumBet);

        inputField.text = amount.ToString();
        slotMachine.crank.interactable = currentAmountOfBananas >= amount;
    }

    public void DecreaseAmount()
    {
        int decrease = 100;

        amount -= decrease;

        if(amount > currentAmountOfBananas)
            amount = Mathf.Max(currentAmountOfBananas, minimumBet);

        if(amount < minimumBet)
            amount = minimumBet;

        inputField.text = amount.ToString();
        slotMachine.crank.interactable = currentAmountOfBananas >= amount;
    }

    public void OnEndEdit(string value)
    {
        if(!string.IsNullOrWhiteSpace(value))
        {
            if(!int.TryParse(value, out amount))
                amount = minimumBet;

            if(amount > maximumBet)
                amount = maximumBet;
        }
        else
            amount = minimumBet;

        if(amount > currentAmountOfBananas)
            amount = Mathf.Max(currentAmountOfBananas, minimumBet);

        inputField.text = amount.ToString();
        slotMachine.crank.interactable = currentAmountOfBananas >= amount;
    }

    public void Show(System.Action onComplete)
    {
        this.onComplete = onComplete;
        DialogCanvas.instance.ShowLoading();

        SaveManager.currentSave.FetchOnlineProfile((profile)=>{
            
            if(profile == null)
                return;

            UpdateCoins(profile.coins);            
            
            SlotMachineAPI.GetBets((success, response)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                    return;

                minimumBet = response.MinimumBet;
                maximumBet = response.MaximumBet;

                amount = minimumBet;
                inputField.text = amount.ToString();
                slotMachine.crank.interactable = currentAmountOfBananas >= amount;

                window.DOKill();
                window.anchoredPosition = new Vector3(0, -400f, 0);

                Canvas.ForceUpdateCanvases();

                window.gameObject.SetActive(true);
                window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);
            });
        });
    }

	public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
        onComplete?.Invoke();
	}

    private void OnDestroy()
    {
        delay?.Kill();
        delay = null;
    }

    private bool HandleError(SlotMachineError error)
    {
        switch(error)
        {
            case SlotMachineError.CantAfford:
                new Dialog(TranslationKey.Generic_Error, TranslationKey.Lottery_CantAfford)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                    .Show();
            return true;
            case SlotMachineError.LessThanMinimumBet:
                new Dialog(TranslationKey.Generic_Error, TranslationKey.Lottery_LessThanMinimumBet)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                    .Show();
            return true;
            case SlotMachineError.NoMoreRewards:
                new Dialog(TranslationKey.Generic_Error, TranslationKey.Lottery_NoMoreRewards)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                    .Show();
            return true;
        }

        return false;
    }
}
