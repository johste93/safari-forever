using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokePuff : MonoBehaviour {

	private float ttl = 0f;

	private void Awake()
	{
		foreach(ParticleSystem pS in GetComponentsInChildren<ParticleSystem>())
		{
			if(pS.main.duration > ttl)
				ttl = pS.main.duration;
		}

		StartCoroutine(DestroySelf());
	}

	private IEnumerator DestroySelf()
	{
		yield return new WaitForSeconds(ttl);
		Destroy(gameObject);
	}
}
