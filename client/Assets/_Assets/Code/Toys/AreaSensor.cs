using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class AreaSensor : MonoBehaviour, ISuspendable
{
	private FSM_CharacterController controller;

	public Transform graphic;
	private LevelEntity entity;
    private LayerMask mask;

	public Transform eye1;
	public Transform eye2;

	public SpriteRenderer eyeRenderer1;
	public SpriteRenderer eyeRenderer2;

	public Color off;
	public Color on;

    public BoxCollider2D boxCollider2D;
    public SpriteRenderer[] corners;

	private bool isSuspended;

	private Tween tween;
	private Tween colorTween;

	private float size1;
	private float size2;

	private float maxSize = 1.5f;

    private NumberGizmo numberGizmo;

    private bool emitPower;

	private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        mask = mask.CombineLayerMask(Globals.gameConstants.whatIsPlayer);
        mask = mask.CombineLayerMask(LayerMask.GetMask("Enemy"));

        size1 = 0f;
		size2 = maxSize/2f;

        numberGizmo = (NumberGizmo) entity.gizmo;

        UpdateColor(false);
        UpdateTriggerSize();
    }

	private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        if(this.controller != null)
            return;

        this.controller = controller;
    }

    public void On_Tap()
    {
        UpdateLineRenderer();
        UpdateTriggerSize();
    }

    public void OnFinishedMoving(LevelEntity levelEntity)
    {
        if(entity != levelEntity)
            return;

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        int size = Mathf.RoundToInt(numberGizmo.GetValue());

        Vector2 halfSize = new Vector2(size, size)/2f;
        Vector2 centerPosition = entity.GetSerializableData().centerPosition;

        Vector2Int bottomLeft = (centerPosition - halfSize).FloorVector2ToInt();
        Vector2Int topRight = (centerPosition + halfSize).FloorVector2ToInt();

        Vector3[] positions = new Vector3[]{
            bottomLeft.ToVector(),
            new Vector3(bottomLeft.x, topRight.y),
            topRight.ToVector(),
            new Vector3(topRight.x, bottomLeft.y),
            bottomLeft.ToVector()
        };

        entity.SetLineRenderPositions(positions);
    }

    private void UpdateTriggerSize()
    {
        int size = Mathf.RoundToInt(numberGizmo.GetValue());
        boxCollider2D.size = new Vector2(size - 0.05f, size - 0.05f);

        float offset = (numberGizmo.index * 1f) + 0.25f;
        corners[0].transform.localPosition = new Vector2(-offset, offset);
        corners[1].transform.localPosition = new Vector2(offset, offset);
        corners[2].transform.localPosition = new Vector2(-offset, -offset);
        corners[3].transform.localPosition = new Vector2(offset, -offset);
    }

	private void Update()
	{
		if(isSuspended)
			return;
		
        if(!emitPower)
        {
            size1 += Time.deltaTime * 2f;
            
            if(size1 > maxSize)
                size1 = 0f;

            size2 += Time.deltaTime * 2f;

            if(size2 > maxSize)
                size2 = 0f;
        }
        else
        {
            size1 = maxSize/2f;
            size2 = maxSize;
        }

        eye1.transform.localScale = new Vector3(size1, size1, 1f);
        eyeRenderer1.color = eyeRenderer1.color.SetAlpha( EaseCurve.InExpo(1f, 0f, size1/maxSize) );

        eye2.transform.localScale = new Vector3(size2, size2, 1f);
        eyeRenderer2.color = eyeRenderer2.color.SetAlpha( EaseCurve.InExpo(1f, 0f, size2/maxSize) );

		
		if(!GameMaster.instance.IsPlaying())
            return;

		DoLogicUpdate();
		
		/*
		if(controller == null)
			return;
		
		Vector2 dirToPlayer = controller.transform.position - transform.position;
		eye.transform.localPosition = Vector2.MoveTowards(eye.transform.localPosition, dirToPlayer.normalized * 0.125f, 5f * Time.deltaTime);
		//rotator.transform.up = Vector2.MoveTowards(rotator.transform.up, dirToPlayer, 5f * Time.deltaTime);
		*/
	}

	private void DoLogicUpdate()
    {
        UpdateColor(emitPower);
        entity.outputNode.EmitPower(emitPower);
    }

	public void OnTriggerEnter2D(Collider2D other)
	{
        if (!GameMaster.instance.IsPlaying())
            return;

		//Debug.Log(other.gameObject.name);
		if(!mask.Contains(other.gameObject.layer))
			return;

		SendSignal();
	}

    public void OnTriggerExit2D(Collider2D other)
	{
        if (!GameMaster.instance.IsPlaying())
            return;

		//Debug.Log(other.gameObject.name);
		if(!mask.Contains(other.gameObject.layer))
			return;

		emitPower = false;
	}

	private void SendSignal()
	{
		emitPower = true;

		tween?.Complete();
        tween = graphic.DOPunchScale(Vector2.one * 0.2f, 0.3f, 1);
	}

    private void UpdateColor(bool enabled)
    {
        if(enabled)
        {
            eyeRenderer1.color = on;
            eyeRenderer2.color = on;
            foreach(SpriteRenderer corner in corners)
                corner.color = on.SetAlpha(corner.color.a);
        }
        else
        {
            eyeRenderer1.color = off;
            eyeRenderer2.color = off;
            foreach(SpriteRenderer corner in corners)
                corner.color = off.SetAlpha(corner.color.a);
        }
    }

	private void Reset()
    {
        emitPower = false;
        size1 = 0f;
		size2 = maxSize/2f;
		UpdateColor(false);
		//colorTween?.Kill();
    }

    private void On_LevelReset(bool manualReset) => Reset();

    private void OnEnable()
    {
		GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += Reset;
		
		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;

        entity.gizmo.On_Tap += On_Tap;
        LevelEntity.On_EntityMoved += OnFinishedMoving;
    }

    private void Unsubscribe()
    {
		GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;

        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= Reset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;

        entity.gizmo.On_Tap -= On_Tap;
        LevelEntity.On_EntityMoved -= OnFinishedMoving;
    }

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		tween?.Kill();
		colorTween?.Kill();
	}


	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;

		if(suspend)
			tween?.Pause();
		else
			tween?.Play();
	}
}
