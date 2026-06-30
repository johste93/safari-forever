using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PunchButton : MonoBehaviour
{
    public void OnClick()
    {
		transform.DOComplete();
       	transform.DOPunchScale(new Vector3(0.2f, 0.4f, 0), 0.3f, 5);
    }

	private void KillAllTweens()
	{
		transform.DOKill();
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}
}
