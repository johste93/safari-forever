using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour, ISuspendable {

	public float speed = 10f;
	private bool isSuspended;

	private void Update()
	{
		if(isSuspended)
			return;

		transform.eulerAngles += new Vector3(0f,0f, speed * Time.deltaTime);
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

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
	}
}
