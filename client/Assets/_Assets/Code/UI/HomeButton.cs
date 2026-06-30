using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomeButton : MonoBehaviour
{
	public RectTransform anchorTransform;
       
    private void OnEnable()
    {
#if UNITY_WEBGL
		gameObject.SetActive(false);
#else
        anchorTransform.gameObject.SetActive(GameMaster.instance.GetCurrentMode() != GameMode.Create);
#endif
    }

    public void OnClick()
	{
		Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
		anchorTransform.DOComplete();
        anchorTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);

		if(GlobalSingleton.mode == GameMode.Create)
		{
			if(!LevelBuilder.instance.IsLevelEmpty())
			{
				GameMaster.instance.ResetLevel(true);
				Garage.SaveWorkInProgressLevel();
			}
		}
		
		SceneLoader.Load(SafariScene.Menu);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClick();
        }	
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		anchorTransform.DOKill();
	}
}
