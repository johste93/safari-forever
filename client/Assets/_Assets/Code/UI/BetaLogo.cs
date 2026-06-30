using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BetaLogo : MonoBehaviour
{
    private List<Tween> tweens = new List<Tween>();
    public void OnClick()
    {
        foreach (Tween t in tweens)
        {
            t.Complete();
        }
        tweens = new List<Tween>();

        //transform.DOComplete();
        tweens.Add(transform.DOPunchScale(new Vector3(0.2f, 0.4f, 0), 0.3f, 5));
        /*
        tweens.Add(DOVirtual.DelayedCall(0.4f, ()=>{
			Messages.BetaDisclaimer();
		}));
        */
    }

	private void KillAllTweens()
	{
        foreach (Tween t in tweens)
        {
            t.Kill();
        }
        tweens = new List<Tween>();
    }

	private void OnDestroy()
	{
		KillAllTweens();
	}	
}
