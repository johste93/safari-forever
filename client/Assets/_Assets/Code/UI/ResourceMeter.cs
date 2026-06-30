using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ResourceMeter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;

    public Color defaultTextColor;
    public Color aboveLimitColor;

    public void OnClick()
    {
        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        transform.DOComplete();
        transform.DOPunchScale(Vector3.one * 0.1f, 0.3f).OnComplete(()=>
        {
            new Dialog(TranslationKey.Editor_ResourceMeter_Info_Title, TranslationKey.Editor_ResourceMeter_Info_Message)
            .AddNeutralButton(TranslationKey.Generic_Ok, null)
            .Show();
        });
    }

    private void UpdateMeter()
    {
        text.text = LevelBuilder.instance.GetCost() + "/" + Globals.gameConstants.blockBudget;

        bool aboveLimit = LevelBuilder.instance.GetCost() > Globals.gameConstants.blockBudget;
        image.color = aboveLimit ? aboveLimitColor : Color.white;
        text.color = aboveLimit ? Color.white : defaultTextColor;

        if(aboveLimit)
        {
            transform.DOComplete();
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        }
    }

    private void OnEnable()
    {
        text.text = LevelBuilder.instance.GetCost() + "/" + Globals.gameConstants.blockBudget;
        LevelBuilder.On_CostUpdate += UpdateMeter;
    }

    private void Unsubscribe()
    {
        LevelBuilder.On_CostUpdate -= UpdateMeter;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		transform.DOKill();
	}
}
