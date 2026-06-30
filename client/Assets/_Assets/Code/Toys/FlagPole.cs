using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlagPole : MonoBehaviour, ISuspendable
{
    public SpriteRenderer flagSprite;

    public Color beforeColor;
    public Color afterColor;

    public Transform pole;
    public Transform flag;

    public Vector3 topPos;
    public Vector3 bottomPos;

    private List<Tween> tweens = new List<Tween>();
	private bool isSuspended;

    private void Awake()
    {
        Reset();
    }
    
    private void Update()
    {
        WaveFlag();
    }

    private void WaveFlag()
    {
		if(isSuspended)
			return;

        flag.eulerAngles = new Vector3(transform.eulerAngles.x, (Mathf.Sin(Time.time*3f) * 20f),transform.eulerAngles.z);
    }

    public void PlayAnimation()
    {
        Reset();

        Audio.Play(SFX.instance.level.flagPole.flagDown, Channel.Game);
        tweens.Add(flag.DOLocalMoveY(bottomPos.y, 0.3f).OnComplete(()=>
        {
            tweens.Add(flag.DOScaleX(1f, 0.3f));
            tweens.Add(flag.DOLocalMoveX(bottomPos.x, 0.3f).OnUpdate(()=>{
                if(flag.localPosition.x < 0)
                    flagSprite.color = afterColor;
            }).OnComplete(()=>
            {
                Audio.Play(SFX.instance.level.flagPole.flagUp, Channel.Game);
                flag.DOLocalMoveY(topPos.y, 0.438f);
            }));
        }));
        
    }

    public void Shake()
    {
        Audio.Play(SFX.instance.level.flagPole.wiggle, Channel.Game);
        pole.localEulerAngles = new Vector3(0,0,0f);
        pole.DOComplete();
        tweens.Add(pole.DOPunchRotation(new Vector3(0,0, 5f), 0.7f, 10).SetEase(Ease.OutElastic));

        flag.DOComplete();
    }

    public void Reset()
    {
        for(int i = 0; i < tweens.Count; i++)
            if(tweens[i] != null)
                tweens[i].Complete();

        flagSprite.color = beforeColor;
        flag.localScale = new Vector3(-1,1,1);
        flag.transform.localPosition = new Vector3(-topPos.x, topPos.y, topPos.z);
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_ExitPlayMode += Reset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_ExitPlayMode -= Reset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
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
		foreach(Tween t in tweens)
			t?.Kill();
	}

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		isSuspended = suspend;
		
		foreach(Tween t in tweens)
		{
			if(suspend)
			{
				t?.Pause();
			}
			else
			{
				t?.Play();
			}
		}
	}
}
