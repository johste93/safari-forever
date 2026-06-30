using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;

public class MetalBlock : MonoBehaviour, ISuspendable
{
    private LevelEntity entity;
    private bool isSuspended;
    private bool playerHasStartedToRun;

    public GameObject electricity;

    public Sprite[] electicitySprites;
    public SpriteRenderer electicityBack;
    public SpriteRenderer electicityFront;

    public Color[] electicityBackColor;
    public Color[] electicityFrontColor;

    private int index;
    private int framesOfPower;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
    }

    public bool PlayerHasStartedToRun()
    {
        return playerHasStartedToRun;
    }

    public bool IsPowered()
    {
        return framesOfPower >= 2;
    }

    private void Update()
    {
        if(!GameMaster.instance.IsPlaying())
			return;

        if(isSuspended)
            return;

        if(Time.frameCount % 2 != 0)
            return;

        index++;
        if(index > 4)
            index = 0;

        electicityFront.transform.localEulerAngles = new Vector3(0,0, 90f * index);
        electicityBack.transform.localEulerAngles = electicityFront.transform.localEulerAngles;

        electicityFront.transform.localScale = new Vector3(1.2f + Random.Range(-0.2f, 0.2f), 1.2f + Random.Range(-0.2f, 0.2f), 1f);
        electicityBack.transform.localScale = electicityFront.transform.localScale;

        electicityFront.sprite = electicitySprites[Random.Range(0, electicitySprites.Length)];
        electicityBack.sprite = electicityFront.sprite;

        electicityFront.color = electicityFrontColor[Random.Range(0, electicityFrontColor.Length)];
        electicityBack.color = electicityBackColor[Random.Range(0, electicityBackColor.Length)];

        electicityBack.transform.localPosition = Random.insideUnitCircle.normalized * 0.075f;
    }

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
			return;

        if(isSuspended)
            return;

        if(!entity.inputNode.HasConnections())
            return;

        if(entity.inputNode.HasConnections() && entity.inputNode.IsPowered())
            framesOfPower++;
        else
            framesOfPower = 0;

        SetIsPowered(framesOfPower >= 2);
    }

    public void SetIsPowered(bool dangerous)
    {
        electricity.SetActive(dangerous);
    }

    private void Reset()
    {
        framesOfPower = 0;
        playerHasStartedToRun = false;
        SetIsPowered(false);
    }

    public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
	}

    private void On_EnterPlayMode() => Reset();

    private void On_ExitPlayMode() => Reset();

    private void On_LevelReset(bool manual) => Reset();

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        playerHasStartedToRun = true;
    }

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;

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
}
