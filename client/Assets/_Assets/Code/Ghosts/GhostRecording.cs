using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class GhostRecording
{
	// x, y is position. z is facing direction
    public List<Vector3> path;


	public GhostRecording()
	{
		path = new List<Vector3>();
	}

	public void SaveToDisk()
	{
		string json = JsonConvert.SerializeObject(this);
		string path = Path.Combine(Application.persistentDataPath, System.Guid.NewGuid().ToString() + ".json");;
		File.WriteAllText(path, json);
		Debug.Log("Recording Saved To Disk");
	}
}
