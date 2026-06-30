using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MusicCollection : ScriptableObject
{
    private const string RESOURCE_PATH = "";
	private const string FILENAME = "MusicCollection";

	private static string path { get { return RESOURCE_PATH + FILENAME; } }

	public static MusicCollection instance
	{
		get
		{
			if (!_instance)
				Load();

			return _instance;
		}
	}

	public static void Preload()
	{
		if (!_instance)
			Load();
	}

	private static void Load()
	{
		_instance = Resources.Load<MusicCollection>(path);

		if (!_instance)
		{
			_instance = CreateInstance<MusicCollection>();

			Debug.LogWarning("MusicCollection ScriptableObject not found in Resources! See: " + path);
		}
	}

	private static MusicCollection _instance;

    public static Tracks tracks { get { return instance._tracks; } }
	public Tracks _tracks = new Tracks();

	[System.Serializable]
	public class Tracks
	{
        public Track[] tracks;

        public Track Get(Music music)
        {
			return tracks.FirstOrDefault(x => x.key == music);
        }
    }
}
