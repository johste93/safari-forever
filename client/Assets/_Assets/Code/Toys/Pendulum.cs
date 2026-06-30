using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour, ISuspendable
{
	private LevelEntity entity;
	public GameObject chainPrefab;
	public Transform center;
	public Transform pendel;
	public new Collider2D collider;

	private int dir;
	float time = 0;
	private bool isSuspended;
	private int framesOfPower;

    private void Awake()
    {
		collider.enabled = false;
		entity = GetComponentInParent<LevelEntity>();
		float leftDistance = Vector2.Distance(entity.GetCenterPosition(), entity.GetSerializableData().bottomLeft);
		float rightDistance = Vector2.Distance(entity.GetCenterPosition(), entity.GetSerializableData().topRight);
		float length = Mathf.Max(leftDistance, rightDistance);

		center.position = entity.GetCenterPosition();
		pendel.localPosition = new Vector3(0, -length, 0);

		dir = leftDistance > rightDistance ? -1 : 1;
		center.eulerAngles = new Vector3(0,0,89 * dir);

		SpawnChain(length);
	}

	private float GetLength()
	{
		return Vector2.Distance(entity.GetSerializableData().bottomLeft, entity.GetSerializableData().topRight);
	}

	private void SpawnChain(float length)
	{
		for(int i = 0; i < length*3; i++)
		{
			GameObject chain = Instantiate(chainPrefab, center);
			chain.transform.localPosition = new Vector3(0, -(i*0.33f), 0);
		}
	}

	private void Update()
	{
		if(!collider.enabled)
			return;

		if(isSuspended)
			return;

		if(framesOfPower < 2 && entity.inputNode.HasConnections())
			return;

		time += Time.deltaTime * SaveManager.currentSave.gameSpeed;
		float t = Mathf.PingPong(time, 1f);
		center.eulerAngles = new Vector3(0,0,EaseCurve.InOutQuad(89 * dir, 89 * -dir, t));
	}

	private void On_EnterPlayMode()
	{
		collider.enabled = true;
		time = 0f;
		Reset();
	}

	private void Reset()
	{
		center.eulerAngles = new Vector3(0,0,89 * dir);
		time = 0f;
		framesOfPower = 0;
	}

	private void On_ExitPlayMode()
	{
		collider.enabled = false;
		Reset();
	}

	private void On_LevelReset()
	{
		Reset();
	}

	private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(isSuspended)
			return;

        if(entity.inputNode.HasConnections() && entity.inputNode.IsPowered())
            framesOfPower++;
        else
            framesOfPower = 0;
    }

	private void On_LevelReset(bool manual) => Reset();

	private void OnEnable()
	{
		GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;
		GameMaster.On_LevelReset += On_LevelReset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
	}

	private void Unsubscribe()
	{
		GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
		GameMaster.On_LevelReset -= On_LevelReset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
	}
}
