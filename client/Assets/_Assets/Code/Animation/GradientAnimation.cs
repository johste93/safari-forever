using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientAnimation : MonoBehaviour {

	public float speed;
	private UIGradient uIGradient;

	private float t;

	private void Awake()
	{
		uIGradient = GetComponent<UIGradient>();
	}

	private void Update()
	{	
		t += Time.deltaTime * speed;

		if(t > 1f)
			t = 0f;
		
		if(t < 0f)
			t = 1f;

		uIGradient.m_angle = Mathf.Lerp(-180, 180, t);
		uIGradient.enabled = false;
		uIGradient.enabled = true;
	}
}
