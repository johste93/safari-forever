using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class GhostRecorder : MonoBehaviour
{
    private FSM_CharacterController player;
	private GhostRecording currentRecording;


	private void Update()
	{
		if(player == null)
			return;

		Vector3 pos = player.transform.position;

		int wallslidingModifier = player.stateController.currentState.Equals(State.WallSliding) || player.stateController.currentState.Equals(State.FallingOfWall) ? -1 : 1;
		float faceDir = Mathf.Sign(player.motion.runningDirection) * wallslidingModifier;

		pos.z = faceDir;
		currentRecording.path.Add(pos);
	}

	private void On_PlayerStartedRunning(FSM_CharacterController controller)
	{
		Reset();
		player = controller;
	}

	private void On_EnterGoal()
	{
		if(currentRecording == null)
			return;

		currentRecording.SaveToDisk();
	}

	private void Reset()
	{
		player = null;
		currentRecording = new GhostRecording();
	}

	private void On_LevelReset(bool manual) => Reset();


	private void OnEnable()
	{
		GameMaster.On_EnterPlayMode += Reset;
		GameMaster.On_ExitPlayMode += Reset;
		GameMaster.On_LevelReset += On_LevelReset;
		GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

		Goal.On_EnterGoal += On_EnterGoal;
	}

	private void Unsubscribe()
	{
		GameMaster.On_EnterPlayMode -= Reset;
		GameMaster.On_ExitPlayMode -= Reset;
		GameMaster.On_LevelReset -= On_LevelReset;
		GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;

		Goal.On_EnterGoal -= On_EnterGoal;
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
