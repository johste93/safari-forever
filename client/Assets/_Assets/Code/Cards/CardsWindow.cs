using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class CardsWindow : MonoBehaviour
{
    public RectTransform window;
    public GameObject cardPrefab;
    public CanvasGroup canvasGroup;
    public UIButton cancelButton;
    
    public Transform parent;
    public RectTransform[] cardPositions;

    private const float animationDuration = 0.2f;
    
    private System.Action onComplete;
    private Coroutine dealCardsRoutine;
    private Card[] cards;

    private int cardsRemaining;

    private int cardPrice = 999999;

    public void TurnCard(int cardIndex)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>{
            
            if(profile == null)
                return;

            canvasGroup.interactable = false;
            cancelButton.SetInteractable(false);

            bool titleKeyAvailable = Localization.KeyAvailable(TranslationKey.Cards_Dialog_Title, SaveManager.currentSave.language);
            Language titleLanguage = titleKeyAvailable ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
            bool titleIsRTL = Localization.IsRightToLeftLanguage(titleLanguage);
            string title = Localization.GetTranslation2(TranslationKey.Cards_Dialog_Title, SaveManager.currentSave.language );

            bool bodyKeyIsAvailable = Localization.KeyAvailable(TranslationKey.Cards_Dialog_Description, SaveManager.currentSave.language);
            Language bodyLanguage = bodyKeyIsAvailable ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
            bool bodyIsRTL = Localization.IsRightToLeftLanguage(bodyLanguage);
            string body = Localization.GetTranslationFormat2(TranslationKey.Cards_Dialog_Description, SaveManager.currentSave.language, new string[]{cardPrice.ToString(), profile.coins.ToString()});
            Dialog dialog = new Dialog(
                title, titleIsRTL, titleLanguage,
                body, bodyIsRTL, bodyLanguage)
                .AddNegativeButton(TranslationKey.Generic_Cancel, ()=>
                {
                    canvasGroup.interactable = true;
                    cancelButton.SetInteractable(true);
                })
                .AddPositiveButton(TranslationKey.Generic_Yes, ()=>
                {
                    if(cardPrice > profile.coins)
                    {
                        HandleError(CardError.CantAfford);
                        canvasGroup.interactable = true;
                        cancelButton.SetInteractable(true);
                        return;
                    }

                    DialogCanvas.instance.ShowLoading();

                    CardsAPI.PickCard(cardIndex, (success, response)=>
                    {
                        DialogCanvas.instance.HideLoading();

                        if(!success)
                        {
                            canvasGroup.interactable = true;
                            cancelButton.SetInteractable(true);
                            return;
                        }

                        if(HandleError(response.Error))
                        {
                            canvasGroup.interactable = true;
                            cancelButton.SetInteractable(true);
                            return;
                        }

                        profile.coins -= cardPrice;
                        CurrencyButton.instance?.UpdateCoins(profile.coins);

                        cards[cardIndex].SetReward(response.Reward);
                        cards[cardIndex].View(false, ()=>
                        {
                            //Make card no longer interactable to avoid player trying to turn it again while it fades away.
                            cards[cardIndex].canvasGroup.interactable = false;

                            if(response.DidWin)
                            {
                                SaveManager.currentSave.unlockedHats[(int)response.Reward] = true;
                                SaveManager.Save();
                                Audio.Play(SFX.instance.ui.slotMachine.win, Channel.UI);
                                Close();
                                PresentReward((Hat) response.Reward);
                            }
                            else
                            {
                                Audio.Play(SFX.instance.ui.slotMachine.loose, Channel.UI);
                                canvasGroup.interactable = true;
                                cancelButton.SetInteractable(true);
                            }     
                        });
                    });
                });
                dialog.Show();
            });
    }

    public void DealCards(bool[] availableSlots)
    {
        dealCardsRoutine = StartCoroutine(DealCardsRoutine(availableSlots));
    }

    private IEnumerator DealCardsRoutine(bool[] availableSlots)
    {
        cardsRemaining = 0;
        cards = new Card[9];

        if(availableSlots[0])
            yield return DealCard(0);
        if(availableSlots[1])    
            yield return DealCard(1);
        if(availableSlots[2])    
            yield return DealCard(2);
        if(availableSlots[5])
            yield return DealCard(5);
        if(availableSlots[8])
            yield return DealCard(8);
        if(availableSlots[7])
            yield return DealCard(7);
        if(availableSlots[6])
            yield return DealCard(6);
        if(availableSlots[3])
            yield return DealCard(3); 
        if(availableSlots[4])
            yield return DealCard(4);
    }

    private IEnumerator DealCard(int cardIndex)
    {
        cardsRemaining++;

        GameObject cardObj = Instantiate(cardPrefab, parent);
            
        RectTransform rT = cardObj.GetComponent<RectTransform>();
        rT.anchoredPosition = Vector2.zero;
        rT.anchorMax = Vector2.one*0.5f;
        rT.anchorMin = Vector2.one*0.5f;
        rT.pivot = Vector2.one*0.5f;

        cards[cardIndex] = cardObj.GetComponent<Card>();
        cards[cardIndex].Initalize(cardIndex, cardPositions[cardIndex].anchoredPosition);
        cards[cardIndex].Elevate(0f);
        cards[cardIndex].Lower(animationDuration);

        yield return new WaitForSeconds(animationDuration/2f);
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

    private void OnDisable()
    {
        KillAllTweens();
        parent.DestroyChildren();
    }

    private void KillAllTweens()
    {
        if(dealCardsRoutine != null)
            StopCoroutine(dealCardsRoutine);

        dealCardsRoutine = null;

        foreach(Card card in cards)
        {
            card?.KillAllTweens();
        }
    }

    public void Show(System.Action onComplete)
    {
        this.onComplete = onComplete;
        DialogCanvas.instance.ShowLoading();

        SaveManager.currentSave.FetchOnlineProfile((profile)=>{
            
            if(profile == null)
                return;
            
            CardsAPI.GetCardsSelection((success, response)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                    return;

                if(HandleError(response.Error))
                    return;

                window.DOKill();
                window.anchoredPosition = new Vector3(0, -400f, 0);

                Canvas.ForceUpdateCanvases();

                window.gameObject.SetActive(true);
                window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);

                cardPrice = response.CardPrice;

                DealCards(response.Cards);
            });
        });
    }

    public void Close()
	{
        KillAllTweens();
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
        onComplete?.Invoke();
        canvasGroup.interactable = true;
        cancelButton.SetInteractable(true);
	}

    private bool HandleError(CardError error)
    {
        switch(error)
        {
            case CardError.CantAfford:
                new Dialog(TranslationKey.Generic_Error, TranslationKey.Lottery_CantAfford)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                    .Show();
            return true;
            case CardError.NoMoreRewards:
                new Dialog(TranslationKey.Generic_Error, TranslationKey.Lottery_NoMoreRewards)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                    .Show();
            return true;

            case CardError.CardIndexOutOfBounds:
            case CardError.CardAlreadyTurned:
            case CardError.NoMoreCardsToTurn:
                Debug.LogError(error.ToString());
                new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                    .Show(true);
            return true;
        }

        return false;
    }
}
