using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspendableParticle : MonoBehaviour, ISuspendable
{
	public ParticleSystem particle;
	
	private void OnEnable()
	{
		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
	}

	private void Unsubscribe()
	{
		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

    public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		if(suspend)
			particle.Pause();
		else
			particle.Play();
	}
}
