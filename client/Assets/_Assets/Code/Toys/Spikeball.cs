using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StandardCharacterController2D.Spikeball.V1;
using DG.Tweening;

public class Spikeball : MonoBehaviour, ISuspendable, ITeleportable
{
    public Transform child;
    public float speed = 150;
    public Spikeball_CharacterController2D controller2D;
    public CapsuleCollider2D capsuleCollider2D;
    public GameObject poff;
    public LayerMask whatIsGround;

    private LevelEntity entity;
    private Vector2 offset = new Vector2(0.5f, 0.5f);
    private bool isDead;

    private int framesOfPower;
	private bool isSuspended;

    public GameObject spikeballGraphics;
    public List<Transform> children;

    private Coroutine routine;
    
    private void Update()
    {
        if(framesOfPower < 2 && entity.inputNode.HasConnections())
            return;

        float dir = 0f;
        if(GameMaster.instance.IsPlaying())
            dir = ((MotionController) controller2D.MotionController).MovingDirection;
        else
            dir = entity.GetSerializableData().gizmoDirection.Value.ToVector().x;

        children[0].eulerAngles += new Vector3(0f,0f, Time.deltaTime * speed * -dir);

        for(int i = 1; i < children.Count; i++)
        {
            children[i].eulerAngles = children[0].eulerAngles; 
        }
    }

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        controller2D.Suspend(true);
        controller2D.BoxCollider2D.enabled = capsuleCollider2D.enabled = false;   

        AdjustHeight();     
    }

    private void AdjustHeight()
    {
        int height = entity.GetHeight();
        controller2D.BoxCollider2D.size = new Vector2(0.8f, 0.8f + (1 * (height-1)));
        controller2D.BoxCollider2D.offset = new Vector2(0f, 0.5f * (height-1) );

        capsuleCollider2D.size = new Vector2(0.4f, 0.4f + (1 * (height-1)));
        capsuleCollider2D.offset = new Vector2(0f, 0.5f * (height-1) );

        children = new List<Transform>();
        for(int i = 0; i < height; i++)
        {
            GameObject lastSpawn = Instantiate(spikeballGraphics, transform);
            lastSpawn.transform.localPosition = new Vector3(0, 1 * i, 0);
            children.Add(lastSpawn.transform);
        }
    }

    public Vector3 GetPortalOffset()
    {
        return new Vector3(0, -0.2f, 0);
    }

    public Vector2 GetSize()
    {
        return controller2D.BoxCollider2D.size;
    }

    public void ValidateSpawnLocation()
    {
        routine = this.Delay1Frame(()=>{
            bool spawnblocked = Physics2D.OverlapCircle(transform.position, 0.1f, whatIsGround);

            //Check if we spawned inside a block!
            if (spawnblocked)
                Die();
        });
    }

    public void Die()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(isDead)
            return;

        isDead = true;

        controller2D.BoxCollider2D.enabled = capsuleCollider2D.enabled = false;
        
        for(int i = 0; i < children.Count; i++)
        {
            Instantiate(poff, children[i].position, Quaternion.identity);
            children[i].gameObject.SetActive(false);
        }

        controller2D.Suspend(true);
    }

    private void On_EnterPlayMode()
    {
        isDead = false;
        controller2D.Suspend(framesOfPower < 2 && entity.inputNode.HasConnections());
        controller2D.BoxCollider2D.enabled = capsuleCollider2D.enabled = true;
        ((MotionController) controller2D.MotionController).MovingDirection = entity.GetSerializableData().gizmoDirection.Value.ToVector().x;
        float dir = ((MotionController) controller2D.MotionController).MovingDirection;
        controller2D.StateController.SetState(new Idle());
        ValidateSpawnLocation();
    }

    private void Reset()
    {
        if(routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        framesOfPower = 0;
        isDead = false;
        controller2D.enabled = true;
        controller2D.BoxCollider2D.enabled = capsuleCollider2D.enabled = true;
        
        controller2D.MotionController.SetInternalVelocity(Vector2.zero);
        controller2D.MotionController.SetConveyorVelocity(Vector2.zero);
        ((MotionController) controller2D.MotionController).MovingDirection = entity.GetSerializableData().gizmoDirection.Value.ToVector().x;

        controller2D.StateController.SetState(new Idle());

        controller2D.Suspend(framesOfPower < 2 && entity.inputNode.HasConnections());

        controller2D.Rigidbody2D.position = entity.GetSerializableData().bottomLeft + offset;
        transform.position = controller2D.Rigidbody2D.position;
        controller2D.CollisionController.UpdateCollisions();

        for(int i = 0; i < children.Count; i++)
        {
            children[i].gameObject.SetActive(true);
        }

        ValidateSpawnLocation();
    }

    private void On_ExitPlayMode()
    {
        Reset();
        controller2D.Suspend(true);
        controller2D.BoxCollider2D.enabled = capsuleCollider2D.enabled = false;
    }

    private void On_LevelReset(bool manual) => Reset();

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

        controller2D.Suspend(framesOfPower < 2 && entity.inputNode.HasConnections());
    }

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
        transform.DOKill();
    }

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
	}

	public void On_SuspensionEvent(bool suspend)
	{
		//if trying to unsuspend.
		if(suspend == false)
		{
			//Check if we are in play mode
			if(GameMaster.instance.IsPlaying())
				Suspend(false);
		}
		else
			Suspend(true);
	}
}
