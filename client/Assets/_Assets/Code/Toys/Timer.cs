using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Timer : MonoBehaviour
{
    public TextMeshPro textMeshPro;

    private LevelEntity entity;

    private float timeLeft;


    private int lastFrameOfPower = -1;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        timeLeft = ((NumberGizmo) entity.gizmo).GetValue();
        textMeshPro.text = Mathf.CeilToInt(timeLeft- Mathf.Epsilon).ToString();
    }

    private void Update()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(timeLeft < 0f)
        {
            entity.outputNode.EmitPower(false);
            return;
        }
        
        timeLeft -= Time.deltaTime * SaveManager.currentSave.gameSpeed;

        textMeshPro.text = Mathf.CeilToInt(timeLeft - Mathf.Epsilon).ToString();

        if(timeLeft < 0f)
        {
            transform.DOComplete();
            transform.DOPunchScale(Vector2.one * 0.3f, 0.3f, 1);
            entity.outputNode.EmitPower(true);
        }
        else
        {
            entity.outputNode.EmitPower(false);
        }
    }

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(entity.inputNode.IsPowered())
        {
            //If previous frame was not powered. Reset Counter.
            if(lastFrameOfPower+1 != Time.frameCount)
            {
                Reset();
                transform.DOComplete();
                transform.DOPunchScale(Vector2.one * 0.3f, 0.3f, 1);
            }
            
            lastFrameOfPower = Time.frameCount;
        }
    }

    private void Reset()
    {
        lastFrameOfPower = -1;
        transform.localScale = Vector3.one;
        timeLeft = ((NumberGizmo) entity.gizmo).GetValue();
        textMeshPro.text = Mathf.CeilToInt(timeLeft- Mathf.Epsilon).ToString();
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_ExitPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_ExitPlayMode -= Reset;
        GameMaster.On_LevelReset -= On_LevelReset;
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
