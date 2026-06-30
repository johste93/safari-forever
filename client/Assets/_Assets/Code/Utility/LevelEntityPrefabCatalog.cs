using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class LevelEntityPrefabCatalog
{
	private static List<PrefabId> prefabIds;

	public static GameObject GetPrefab(string id)
	{
		if(prefabIds == null)
			BuildCatalog();

		return prefabIds.First(x => x.id == id).prefab;
	}

	private static void BuildCatalog()
	{
		GameObject[] prefabs = Resources.LoadAll<GameObject>("LevelEntities");
		
		prefabIds = new List<PrefabId>();
		foreach(GameObject prefab in prefabs)
		{
			prefabIds.Add(new PrefabId(prefab));
		}
	}
}
