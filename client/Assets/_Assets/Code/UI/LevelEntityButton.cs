using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelEntityButton : MonoBehaviour
{
    public GameObject prefab;
	private LevelEntity lastSpawnedEntity;
	private bool fingerIndexAssigned = false;

	public void OnBeginDrag()
	{
		bool unique = prefab.GetComponent<LevelEntity>().unique;
		if(unique)
		{
			string id = prefab.GetComponent<LevelEntity>().uniqueId;

			LevelEntity uniqueSpawned = LevelBuilder.instance.GetLevelEntities().FirstOrDefault(x => x.id == id);
			if(uniqueSpawned != null)
			{
				fingerIndexAssigned = false;
				lastSpawnedEntity = uniqueSpawned;
				return;
			}
		}
		
        GameObject lastSpawn = Instantiate(prefab, new Vector3(0, -1000, 0), Quaternion.identity);
		lastSpawnedEntity = lastSpawn.GetComponent<LevelEntity>();
		fingerIndexAssigned = false;
	}

	private void On_TouchMaintained(TouchInfo touch)
	{
		if(lastSpawnedEntity != null && !fingerIndexAssigned)
		{
			lastSpawnedEntity.GetDragHandle().AssignFinger(touch.fingerIndex);
			fingerIndexAssigned = true;
		}	
	}

	private void On_TouchEnd(TouchInfo touch)
	{
		if(lastSpawnedEntity == null)
			return;

		lastSpawnedEntity.GetDragHandle().On_TouchEnd(touch);
		
		lastSpawnedEntity = null;
	}

	private void OnEnable()
	{
		TouchInput.On_TouchMaintained += On_TouchMaintained;
		TouchInput.On_TouchEnd += On_TouchEnd;
	}

	private void Unsubscribe()
	{
		TouchInput.On_TouchMaintained -= On_TouchMaintained;
		TouchInput.On_TouchEnd -= On_TouchEnd;
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
