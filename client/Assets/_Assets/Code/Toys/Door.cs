using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

	public GameObject closedDoor;
    public GameObject openDoor;

    public void Open()
    {
		Audio.Play(SFX.instance.level.door.open, Channel.Game);
		closedDoor.SetActive(false);
        openDoor.SetActive(!closedDoor.activeInHierarchy);
    }

	private void Reset()
	{
		closedDoor.SetActive(true);
        openDoor.SetActive(!closedDoor.activeInHierarchy);
	}

	private void On_LevelReset(bool manual)
	{
		Reset();
	}

	private void On_ExitPlayMode()
	{
		Reset();
	}
    
    private void OnEnable()
	{
		GameMaster.On_LevelReset += On_LevelReset;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;
	}

	private void Unsubscribe()
	{
		GameMaster.On_LevelReset -= On_LevelReset;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
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
