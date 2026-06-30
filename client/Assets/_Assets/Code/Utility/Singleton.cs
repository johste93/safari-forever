using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
	public static T instance { get
	{
		if(_instance == null)
			_instance = (T) FindObjectOfType(typeof(T));
			
		return _instance;
	}}

	private static T _instance;
}
