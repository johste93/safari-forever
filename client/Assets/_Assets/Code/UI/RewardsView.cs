using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;
using System.Text.RegularExpressions;

public class RewardsView : PopupView
{
    public AudioClip slapSFX;
    public AudioClip counterSFX;
    public TextMeshProUGUI bananaCount;
    public TextMeshProUGUI amountRecived;
    public TextMeshProUGUI reason;
    public GameObject Counter;
    public List<GameObject> illustrations;

    private GameObject currentIllustration;
    private Queue<UnrecivedReward> rewardQueue;
    private UnrecivedReward currentReward;

    private List<Tween> currentlyPlayingTweens = new List<Tween>();

    public CanvasGroup subCanvasGroup;
    public Image rewardBackground;

    public override void Show(System.Action onExit, bool instant = false, System.Action onComplete = null)
    {
        if(this.onExit == null && onExit != null)
            this.onExit = onExit;

        if(!PopupCanvas.instance.container.activeInHierarchy)
        {
            PopupCanvas.instance.container.SetActive(true);
            PopupCanvas.instance.background.color = PopupCanvas.instance.background.color.SetAlpha(0);
        }

        rectTransform.SetAsLastSibling();
        rectTransform.DOKill();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 50f);

        tweens.Add(rectTransform.DOAnchorPosY(0f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay));

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.gameObject.SetActive(true);
        subCanvasGroup.alpha = 1f;
        rewardBackground.color = new Color(rewardBackground.color.r, rewardBackground.color.g, rewardBackground.color.b, 1f);

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        tweens.Add(canvasGroup.DOFade(1f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay).OnComplete(()=>
        {
            onComplete?.Invoke();
        }));
    }

    public override void Close(bool instant = false, System.Action onComplete = null)
    {
        rectTransform.DOKill();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0f);

		tweens.Add(rectTransform.DOAnchorPosY(50f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay));

		canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
		canvasGroup.DOKill();
        canvasGroup.alpha = 1f;


        subCanvasGroup.alpha = 1f;
        tweens.Add(subCanvasGroup.DOFade(0f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay).OnComplete(()=>
        {
            tweens.Add(rewardBackground.DOFade(0f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay).OnComplete(()=>
            {
                //PopupCanvas.instance.rewardsView.transform.SetSiblingIndex(1);
                canvasGroup.alpha = 0f;
                onComplete?.Invoke();
                canvasGroup.gameObject.SetActive(false);
            }));
        }));

       
    }

    public void Next()
    {
        //If playing animation
        if(currentlyPlayingTweens.Count > 0)
        {
            foreach(Tween t in currentlyPlayingTweens)
                t?.Kill();

            currentlyPlayingTweens = new List<Tween>();

            bananaCount.transform.localScale = Vector3.one;
            bananaCount.text = currentReward.BalanceAfter.ToString(); 
            bananaCount.transform.DOPunchScale(Vector3.one * 0.3f, 0.1f);
            currentIllustration.transform.localScale = Vector3.one;
            currentIllustration.transform.GetChild(0).eulerAngles = Vector3.zero;
            reason.transform.localScale = Vector3.one;
            amountRecived.transform.localScale = Vector3.one;
            Audio.Play(counterSFX, Channel.UI);
            
            return;
        }

        if(rewardQueue.Count == 0)
        {
            base.Exit();
            return;
        }

        currentReward = rewardQueue.Dequeue();
        SetReward(currentReward);
    }

    public void Initalise(List<UnrecivedReward> unrecivedRewards)
    {
        List<UnrecivedReward> sorted = unrecivedRewards.OrderBy(x => x.RecivedOn).ToList();

        //We need to handle likes differently. Get all likes first, then combine them then add them to que.
        Dictionary<UnrecivedReward, int> unrecivedLikes = new Dictionary<UnrecivedReward, int>();

        for(int i = 0; i < sorted.Count; i++)
        {
            if(sorted[i].TransactionType != TransactionType.RecivedLike)
                continue;

            unrecivedLikes.Add(sorted[i], i);
        }
        
        Dictionary<string, int> likesRecived = new Dictionary<string, int>();
        List<UnrecivedReward> likes = new List<UnrecivedReward>();


        foreach(KeyValuePair<UnrecivedReward, int> kVP in unrecivedLikes)
        {
            for(int i = kVP.Value; i < sorted.Count; i++)
            {
                sorted[i].BalanceBefore -= kVP.Key.ChangeInBalance;
                sorted[i].BalanceAfter -= kVP.Key.ChangeInBalance;
            }

            sorted.Remove(kVP.Key);

            if(likesRecived.ContainsKey(kVP.Key.LevelName))
            {
                likesRecived[kVP.Key.LevelName] += kVP.Key.ChangeInBalance;
            }
            else
            {
                likesRecived.Add(kVP.Key.LevelName, kVP.Key.ChangeInBalance);
            }
        }

        foreach(KeyValuePair<string, int> kVP in likesRecived)
        {
            UnrecivedReward reward = new UnrecivedReward()
            {
                ChangeInBalance = kVP.Value,
                
                TransactionType = TransactionType.RecivedLike,
                LevelName = kVP.Key
            };

            reward.BalanceBefore = sorted.Count > 0 ? sorted[sorted.Count-1].BalanceAfter : 0;
            reward.BalanceAfter = reward.BalanceBefore + kVP.Value;

            sorted.Add(reward);
        }

        rewardQueue = new Queue<UnrecivedReward>(sorted);

        Next();
    }

    public void SetReward(UnrecivedReward unrecivedReward)
    {
        switch(unrecivedReward.TransactionType)
        {
            case TransactionType.WonHat:

                Counter.SetActive(false);
                amountRecived.text = string.Empty;
                string[] segments = Regex.Split(unrecivedReward.Hat.ToString(), @"(?<!^)(?=[A-Z])");
                for(int i = 0; i < segments.Length; i++)
                {
                    amountRecived.text += segments[i];
                    if(i < segments.Length-1)
                        amountRecived.text += " ";
                }
                amountRecived.isRightToLeftText = false;
                amountRecived.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, amountRecived.font, SaveManager.currentSave.language);

            break;
            default:

                Counter.SetActive(true);

                bananaCount.text = unrecivedReward.BalanceBefore.ToString();

                amountRecived.text = $"+{unrecivedReward.ChangeInBalance}";
                amountRecived.isRightToLeftText = false;
                amountRecived.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, amountRecived.font, SaveManager.currentSave.language);

            break;
        }

        SetReason(unrecivedReward);
        currentIllustration = SetIllustration(unrecivedReward);

        reason.transform.localScale = Vector3.zero;
        amountRecived.transform.localScale = Vector3.zero;

        currentlyPlayingTweens.Add(DOVirtual.DelayedCall(0.5f, ()=>
        {
            float duration = 0.3f;
            float interval = 0.5f;

            //Play Animation
            currentlyPlayingTweens.Add(currentIllustration.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
            currentlyPlayingTweens.Add(currentIllustration.transform.GetChild(0).DORotate(new Vector3(1, 1, 360), duration).SetRelative(true).SetEase(Ease.OutBack));

            Audio.Play(slapSFX, Channel.UI);

            currentlyPlayingTweens.Add(DOVirtual.DelayedCall(interval, ()=>
            {
                currentlyPlayingTweens.Add(amountRecived.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));  
                currentlyPlayingTweens.Add(DOVirtual.DelayedCall(interval, ()=>
                {
                    currentlyPlayingTweens.Add(reason.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
                    currentlyPlayingTweens.Add(DOVirtual.DelayedCall(interval, ()=>{
                        CountUp(unrecivedReward, duration);
                    }));
                }));
            }));
        }));
    }

    private void CountUp(UnrecivedReward unrecivedReward, float duration)
    {   
        if(unrecivedReward.TransactionType == TransactionType.WonHat)
            return;

        float t = Mathf.Clamp01(unrecivedReward.ChangeInBalance/100f);
        float bananasPrSecond = Mathf.Lerp(10f, 20f, t);
        int previousCount = unrecivedReward.BalanceBefore;

        currentlyPlayingTweens.Add(
            DOVirtual.Float(unrecivedReward.BalanceBefore, unrecivedReward.BalanceAfter, bananasPrSecond, (balance)=>{
                
                int floored = Mathf.FloorToInt(balance);

                if(floored > previousCount)
                {
                    Audio.Play(counterSFX, Channel.UI);
                    bananaCount.text = floored.ToString();
                    bananaCount.transform.DOComplete();
                    currentlyPlayingTweens.Add(bananaCount.transform.DOPunchScale(new Vector3(0.1f, 0.4f, 0f), 1f/bananasPrSecond));
                    previousCount = floored; 
                }
            })
            .SetSpeedBased(true)
            .SetEase(Ease.InSine)
            .OnComplete(()=>{
                bananaCount.text = unrecivedReward.BalanceAfter.ToString(); 
                currentlyPlayingTweens = new List<Tween>();
            })
        );
    }

    private GameObject SetIllustration(UnrecivedReward unrecivedReward)
    {
        foreach(GameObject illustration in illustrations)
        {
            illustration.SetActive(false);
            illustration.transform.localScale = Vector3.zero;
        }

        int index = 0;
        switch(unrecivedReward.TransactionType)
        {
            default:
            case TransactionType.BeatLevel: 
                index = 0;
            break;
            case TransactionType.BeatNewLevel: 
            case TransactionType.BeatPromotedLevel: 
                index = 1;
            break;
            case TransactionType.DailyChallengeParticipation: 
                index = 3;
            break;
            case TransactionType.DailyChallengeThirdPlace:  
                index = 4;
            break;
            case TransactionType.DailyChallengeSecondPlace:  
                index = 5;
            break;
            case TransactionType.DailyChallengeFirstPlace:  
                index = 6;
            break;

            case TransactionType.Gift:
            case TransactionType.Refund:
            case TransactionType.RecivedLike: 

                index = 0;
                
                if(unrecivedReward.ChangeInBalance >= 3)
                    index = 1;

                if(unrecivedReward.ChangeInBalance >= 5)
                    index = 2;

                if(unrecivedReward.ChangeInBalance >= 15)
                    index = 3;

                if(unrecivedReward.ChangeInBalance >= 25)
                    index = 4;

                if(unrecivedReward.ChangeInBalance >= 50)
                    index = 5;

                if(unrecivedReward.ChangeInBalance >= 100)
                    index = 6;

            break;
            case TransactionType.WonHat:

                switch(unrecivedReward.Hat)
                {
                    case Hat.Santa:
                        index = 7;
                    break;
                    case Hat.Shades:
                        index = 8;
                    break;
                    case Hat.Thinfoil:
                        index = 9;
                    break;
                    case Hat.Wizzard:
                        index = 10;
                    break;
                    case Hat.Witch:
                        index = 11;
                    break;
                    case Hat.Pirate:
                        index = 12;
                    break;
                    case Hat.Showbiz:
                        index = 13;
                    break;
                    case Hat.Halo:
                        index = 14;
                    break;
                    case Hat.TopHat:
                        index = 15;
                    break;
                    case Hat.Viking:
                        index = 16;
                    break;
                    case Hat.Horns:
                        index = 17;
                    break;
                    case Hat.Sombrero:
                        index = 18;
                    break;
                    case Hat.Conical:
                        index = 19;
                    break;
                    case Hat.Boot:
                        index = 20;
                    break;
                    case Hat.Comrade:
                        index = 21;
                    break;
                    case Hat.Crown:
                        index = 22;
                    break;
                    case Hat.Mustache:
                        index = 23;
                    break;
                    case Hat.Beanie:
                        index = 24;
                    break;
                    case Hat.SouWester: 
                        index = 25;
                    break;
                    case Hat.Private:
                        index = 26;
                    break;
                }

            break;
        }

        illustrations[index].SetActive(true);
        return illustrations[index];
    }

    private void SetReason(UnrecivedReward unrecivedReward)
    {
        switch(unrecivedReward.TransactionType)
        {
            case TransactionType.BeatLevel: 
            case TransactionType.BeatNewLevel: 
            case TransactionType.BeatPromotedLevel: 
                reason.TranslateFormat(TranslationKey.Rewards_BeatLevel_Description, SaveManager.currentSave.language, FontType.Regular, false, unrecivedReward.LevelName);
                break;
            case TransactionType.RecivedLike: 
                reason.TranslateFormat(TranslationKey.Rewards_RecivedLike_Description, SaveManager.currentSave.language, FontType.Regular, false, unrecivedReward.LevelName);
                break;
            case TransactionType.DailyChallengeParticipation: 
                reason.Translate(TranslationKey.Rewards_DailyChallengeParticipation_Description, SaveManager.currentSave.language, FontType.Regular, false);
                break;
            case TransactionType.DailyChallengeThirdPlace:  
                reason.Translate(TranslationKey.Rewards_DailyChallengeThirdPlace_Description, SaveManager.currentSave.language, FontType.Regular, false);
                break;
            case TransactionType.DailyChallengeSecondPlace:  
                reason.Translate(TranslationKey.Rewards_DailyChallengeSecondPlace_Description, SaveManager.currentSave.language, FontType.Regular, false);
                break;
            case TransactionType.DailyChallengeFirstPlace:  
                reason.Translate(TranslationKey.Rewards_DailyChallengeFirstPlace_Description, SaveManager.currentSave.language, FontType.Regular, false);
                break;
            case TransactionType.WonHat:
                reason.Translate(TranslationKey.Rewards_Cards_Description, SaveManager.currentSave.language, FontType.Regular, false);
                break;
            default:
            case TransactionType.Gift:
            case TransactionType.Refund:
                reason.text = unrecivedReward.Description; //Assume description is Globals.localizationConstants.defaultLanguage
                reason.isRightToLeftText = Localization.IsRightToLeftLanguage(Globals.localizationConstants.defaultLanguage);
                reason.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Regular, reason.font, Globals.localizationConstants.defaultLanguage);
                //Dont change alignment.
                break;
        }
    }

    protected override void KillAllTweens()
    {
        base.KillAllTweens();

        foreach(Tween t in currentlyPlayingTweens)
            t?.Kill();
    }
}
