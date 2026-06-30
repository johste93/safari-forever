using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingImage : MonoBehaviour, ISuspendable {

	public Vector2 speed;
	private Vector2 offset;
	private bool isSuspended;

	private void Update () 
	{
		if(isSuspended)
			return;

		offset += speed * Time.deltaTime;
		GetComponent<Image>().material.SetTextureOffset ("_MainTex", offset);
	}

	public void Suspend(bool suspend)
	{
		isSuspended = suspend;
	}

	public void On_SuspensionEvent(bool suspended)
	{
		Suspend(suspended);
	}

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
}
