using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class SFX : ScriptableObject
{
    private const string RESOURCE_PATH = "";
	private const string FILENAME = "SFX";

	private static string path { get { return RESOURCE_PATH + FILENAME; } }

	public static SFX instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Resources.Load<SFX>(path);

				if (!_instance)
				{
					_instance = CreateInstance<SFX>();

					Debug.LogWarning("Globals ScriptableObject not found in Resources! See: " + path);
				}
			}

			return _instance;
		}
	}
	private static SFX _instance;

    public CharacterSFXPool character;
	public UISFXPool ui;
	public LevelSFXPool level;
}
