using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializeableColor
{
	public SerializeableColor()
	{
		this.color = Color.black;
	}

	public SerializeableColor(Color color)
	{
		this.color = color;
	}

	[Newtonsoft.Json.JsonIgnore] public Color color 
	{
		get{
			return new Color(r,g,b,a);
		}
		set{
			r = value.r;
			g = value.g;
			b = value.b;
			a = value.a;

			hex = "#" + ColorUtility.ToHtmlStringRGB( value );
		}
	}
	public float r,g,b,a;
	public string hex;
}
