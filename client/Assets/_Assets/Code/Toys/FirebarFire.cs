using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebarFire : MonoBehaviour, ISuspendable
{
    public GameObject center;

    public Fire fire;

    private LevelEntity entity;
    
    private float t;
    private Vector2 startPos;
    private const float speed = 0.25f;

    private bool paused = true;
	private bool isSuspended;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        //center.SetActive((Vector2)transform.position == entity.GetCenterPosition());
        //fire.spriteRenderer.gameObject.SetActive(!center.activeInHierarchy);
    }

    private void Update()
    {
        if(!fire.circleCollider2D.enabled)
            return;

        if(paused)
            return;

		if(isSuspended)
			return;

        RotateAroundCenter();   
    }

    private void RotateAroundCenter()
    {
        t += Time.deltaTime * SaveManager.currentSave.gameSpeed * speed * (int)entity.GetSerializableData().gizmoRotation;
        transform.position = GetPos(t, GetLength());
    }

    private Vector3 GetPos (float t, float radius )
    {
         float ang = t * 360f;
         Vector3 result;
         Vector3 center = entity.GetCenterPosition();
         result.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
         result.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
         result.z = center.z;
         return result;
     }

    private float GetLength() //Use this as a relative value.
    {
        if(entity == null)
            return 0;

        return Vector2.Distance(entity.GetCenterPosition(), transform.position);
    }

    private void On_EnterPlayMode()
    {
        fire.circleCollider2D.enabled = !center.activeInHierarchy;
        startPos = transform.position;
        Reset();
    }

    private void On_ExitPlayMode()
    {
        fire.circleCollider2D.enabled = false;
        Reset();
    }

    private void Reset()
    {
        float angle = MathIsHard.DirectionToEuler(startPos - (Vector2)entity.GetCenterPosition());
        t = angle/360f;
        transform.position = GetPos(t, GetLength());

        fire.gameObject.SetActive(true);

        paused = entity.inputNode.links.Count > 0 && !entity.inputNode.IsPowered();
    }

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        paused = entity.inputNode.links.Count > 0 && !entity.inputNode.IsPowered();
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

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
