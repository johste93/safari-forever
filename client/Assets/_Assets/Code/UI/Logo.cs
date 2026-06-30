using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Logo : MonoBehaviour
{
    public Transform graphic;

    public void OnClick()
    {
		transform.DOComplete();
       	transform.DOPunchScale(new Vector3(0.2f, 0.4f, 0), 0.3f, 5).OnComplete(()=>{
            DialogCanvas.instance.ShowCredithWindow();
        });
    }

    private void Start()
    {
        graphic.DOLocalMoveY(-25f, 2f).SetRelative(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

	private void KillAllTweens()
	{
        transform.DOKill();
		graphic.DOKill();
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif              
        }
    }
}
