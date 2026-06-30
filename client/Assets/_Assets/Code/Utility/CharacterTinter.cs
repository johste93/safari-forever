using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTinter : MonoBehaviour {

	public SpriteRenderer[] spriteRenderers;

	public void SetColor(Color newColor)
	{
		foreach(SpriteRenderer sR in GetComponentsInChildren<SpriteRenderer>())
		{
			sR.color = newColor;
		}
	}
}
