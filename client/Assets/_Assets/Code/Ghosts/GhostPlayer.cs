using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class GhostPlayer : MonoBehaviour
{

	public GameObject ghostPrefab;
	public GhostRecording playbackRecording;
	public string playbackPath;
	public Ghost currentGhost;

	private void Start()
	{
		Load();
	}

	private void Update()
	{
		if(!Application.isEditor)
			return;

		if(!GameMaster.instance.IsPlaying())
			return;
		
		if(Input.GetKeyDown(KeyCode.P))
		{
			if(currentGhost == null)
				Play();
			else
				currentGhost.Rewind();
		}
	}

	private void Play()
	{
		if(playbackRecording == null)
			return;

		GameObject lastSpawn = Instantiate(ghostPrefab);
		currentGhost = lastSpawn.GetComponent<Ghost>();

		currentGhost.StartRunning(playbackRecording);
	}

	private void Load()
	{
		string path = Path.Combine(Application.persistentDataPath, playbackPath);
		string json = File.ReadAllText(path);
		playbackRecording = JsonConvert.DeserializeObject<GhostRecording>(json);
	}
	
    private void OnEnable()
	{

	}

	private void Unsubscribe()
	{

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
