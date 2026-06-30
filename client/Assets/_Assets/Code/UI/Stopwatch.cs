using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FSM_CharacterController2D;
using System;

public class Stopwatch : MonoBehaviour, ISuspendable
{
	public static Stopwatch instance;
    public TextMeshProUGUI textMesh;

	private Session[] sessions;

	private int currentRoom;
	private bool record;
	private double previousRecord = -1;

	private double total = 0;

	private bool isSupended;

	private void Awake()
	{
		instance = this;
		ResetAll();
	}

	private void On_PlayerStartedRunning(FSM_CharacterController controller)
	{
		BeginCurrentRoom();

		record = true;

		if(GameMaster.instance.GetCurrentMode() == GameMode.Campaign)
			if(GameMaster.instance.currentlyPlayingLevel != null && GameMaster.instance.currentlyPlayingLevel.campaignInfo != null)
			{
				int world = (int)GameMaster.instance.currentlyPlayingLevel.campaignInfo.world;
				int index = GameMaster.instance.currentlyPlayingLevel.campaignInfo.campaignIndex;

				previousRecord = SaveManager.currentSave.campaignProgress[world][index].seconds + (SaveManager.currentSave.campaignProgress[world][index].milliseconds / 100d);
			}
	}

	private void Update()
	{
		SetTextMesh(CalculateTotal());
	}

	private void SetTextMesh(double total)
	{
		textMesh.text = $"<mspace=15>{ToString(total)}";
		textMesh.color = total < previousRecord || previousRecord < 0 ? Color.white : Color.white.SetVibrance(0.75f);
	}

	private void On_EnterPlayMode()
	{
		textMesh.gameObject.SetActive(true);
	}

	private void On_EnterGoal()
	{
		record = false;
		EndCurrentRoom();
		SetTextMesh(GetTotal());
	}

	private void On_ExitPlayMode()
	{
		record = false;
		EndCurrentRoom();
		ResetAll();
		textMesh.gameObject.SetActive(false);
	}

	private void On_LevelReset(bool manual)
	{
		record = false;
		ResetCurrentRoom();
		SetTextMesh(GetTotal());
	}

	private void On_PlayerDied(FSM_CharacterController controller)
	{
		record = false;
		EndCurrentRoom();
		SetTextMesh(GetTotal());
	}

	public void ResetAll()
	{
		sessions = new Session[4];
		for(int i = 0; i < sessions.Length; i++)
			sessions[i] = new Session();

		SetTextMesh(0f);
	}

	public double CalculateTotal(bool logging = false)
	{
		double unroundedResult = 0f;
		//double prerounded = 0f;
		double resultRoundedAtEnd = 0f;

		foreach(Session s in sessions)
		{
			unroundedResult += s.GetElapsedTime(false);
			//prerounded += s.GetElapsedTime(true);
		}

		//double resultRounded = Math.Round(result * 100d) / 100d;
		resultRoundedAtEnd = Math.Round(unroundedResult * 100d) / 100d;

		//if(resultRoundedAtEnd > 99.999d)
			//resultRoundedAtEnd = 99.999d;

        return resultRoundedAtEnd;
	}

	public double GetTotal()
	{
		return total;
	}

	public static string ToString(double total)
	{
		int seconds = (int) Math.Truncate(total);
		int miliseconds = (int)((total - seconds)*100);
		return $"{seconds.ToString("D2")}:{miliseconds.ToString("D2")}";
	}

	public void Suspend(bool suspend)
	{
		isSupended = suspend;
		sessions[currentRoom].Suspend(isSupended);
	}

    public void On_SuspensionEvent(bool suspend)
	{
		Suspend(suspend);
	}

	public void On_ChangedRoom()
	{
		currentRoom = LevelBuilder.instance.GetCurrentRoomIndex();
	}


	private void OnEnable()
	{
		Goal.On_EnterGoal += On_EnterGoal;
		GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;
		GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;
		GameMaster.On_PlayerDied += On_PlayerDied;
		GameMaster.On_LevelReset += On_LevelReset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;

		LevelBuilder.On_ChangedRoom += On_ChangedRoom;
	}

	private void Unsubscribe()
	{
		Goal.On_EnterGoal -= On_EnterGoal;
		GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
		GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
		GameMaster.On_PlayerDied -= On_PlayerDied;
		GameMaster.On_LevelReset -= On_LevelReset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;

		LevelBuilder.On_ChangedRoom -= On_ChangedRoom;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

	private void BeginCurrentRoom()
	{
		sessions[currentRoom].Begin();
	}

	private void ResetCurrentRoom()
	{
		sessions[currentRoom].Reset();
	}

	private void EndCurrentRoom()
	{
		sessions[currentRoom].End();
		total = CalculateTotal(true);
	}

	private class Session
	{
		private List<Segment> segments = new List<Segment>();
		private Segment currentSegment;
		private bool isRecording;

		public void Begin()
		{
			if(isRecording)
				return;

			isRecording = true;
			Suspend(false);
		}

		public void End()
		{
			if(!isRecording)
				return;

			Suspend(true);
			isRecording = false;
		}

		public void Suspend(bool suspend)
		{
			if(!isRecording)
				return;

			if(suspend)
			{
				currentSegment.Stop();
			}
			else
			{
				currentSegment = new Segment();
				currentSegment.Start();
				segments.Add(currentSegment);
			}
		}

		public void Reset()
		{
			segments = new List<Segment>();
			isRecording = false;
			currentSegment = null;
		}

		public double GetElapsedTime(bool round = false)
		{
			double result = 0f;

			foreach(Segment s in segments)
			{
				double elapsedTime = s.GetElapsedTime();
				result += elapsedTime;
			}
				
			if(round)
				result = Math.Round(result * 100d) / 100d;

			return result;
		}

		private class Segment
		{
			public double startTime = 0;
			public double endTime = 0;
			public bool isRecording = false;

			public void Start()
			{
				if(isRecording)
					return;

				startTime = Time.time;
				isRecording = true;
			}

			public void Stop()
			{
				if(!isRecording)
					return;

				isRecording = false;
				endTime = Time.time;
			}

			public double GetElapsedTime()
			{
				double timeSinceStart = Time.time - startTime;
				double result = isRecording ? timeSinceStart : (endTime - startTime);
				return result;
			}
		}
	}
}
