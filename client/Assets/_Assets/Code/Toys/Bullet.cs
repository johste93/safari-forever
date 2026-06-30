using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;
using System.Linq;

public class Bullet : MonoBehaviour, IStompable, IDangerous, ISuspendable, ITeleportable
{
    public float speed = 10f;
    public new Rigidbody2D rigidbody2D;
    public BoxCollider2D triggerCollider2D;
    public BoxCollider2D collisionCollider2D;
    public Transform child;

	public SpriteRenderer[] sprites;

    public LayerMask whatIsGround;
    public GameObject poff;

    private bool isSuspended;
    private Vector2 vectorDirection;
    private bool initalized = false;

	private List<Tween> tweens = new List<Tween>();
    
    private static List<Bullet> bullets = new List<Bullet>();

    public void Initalize(Vector2 direction)
    {
        this.vectorDirection = direction;
        initalized = true;

        child.localScale = Vector3.zero;
        tweens.Add(child.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad));

        //Destroy(gameObject, 4f);
    }

    public void Initalize(Direction8 direction)
    {
        vectorDirection = direction.ToVector();
        initalized = true;

        child.localScale = Vector3.zero;
        tweens.Add(child.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad));

        //Destroy(gameObject, 4f);
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(!whatIsGround.Contains(other.gameObject.layer))
            return;

        if(other.transform == transform.parent)
            return;

		if(other.gameObject.CompareTag("OneWayPlatform"))
			return;



        if(other.gameObject.CompareTag("OneWayGateNorth") && vectorDirection.y > 0)
			return;

        if(other.gameObject.CompareTag("OneWayGateEast") && vectorDirection.x > 0)
			return;

        if(other.gameObject.CompareTag("OneWayGateSouth") && vectorDirection.y < 0)
			return;

        if(other.gameObject.CompareTag("OneWayGateWest") && vectorDirection.x < 0)
			return;
        


        if(other.gameObject.CompareTag("Breakable"))
            other.gameObject.GetComponent<IBreakable>().Break();

        DestroyBullet();
    }

    public void OnStomped(Character character)
    {
        //Snap character to bullet
        character.controller.collisionInfo.left = character.controller.collisionInfo.right = false;
        
        Vector3 targetPos = new Vector3(transform.position.x, character.transform.position.y, character.transform.position.z);
        Vector2 center = (Vector2)targetPos + character.controller.boxCollider2D.offset;

        if (!Physics2D.OverlapBox(center, character.controller.boxCollider2D.size, 0, whatIsGround))
            character.controller.transform.position = targetPos;

        character.controller.stateController.OverideState(State.Bouncing);
        character.controller.motion.rawVelocity = new Vector2(0, 5f);
        character.controller.motion.timeSpentFluttering = 0;

        Die();
    }   

    private void Die()
    {
        collisionCollider2D.enabled = false;
        triggerCollider2D.enabled = false;
        vectorDirection = Direction8.Down.ToVector();
        speed = speed * 2f;

        child.DOComplete();
        tweens.Add(child.DOScale(new Vector2(1.3f, 0.7f), 0f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            tweens.Add(child.DOScale(Vector2.one, 0.2f).SetEase(Ease.Linear));
        }));

		foreach(SpriteRenderer sR in  sprites)
		{
			tweens.Add(sR.DOFade(0f, 0.3f));
		}
    }

    private void FixedUpdate()
    {
        if(!initalized)
            return;

        if(isSuspended)
            return;

        Vector2 deltaMovement = vectorDirection.normalized * speed * Time.fixedDeltaTime * SaveManager.currentSave.gameSpeed;
        rigidbody2D.MovePosition(rigidbody2D.position + deltaMovement);
    }

    public Vector3 GetPortalOffset()
    {
        return new Vector3(0, 0, 0);
    }

    public Vector2 GetSize()
    {
        return collisionCollider2D.size;
    }

    public void DestroyBullet()
    {
        collisionCollider2D.enabled = false;
        
        if(transform.localScale.magnitude > 0.1f)
            Instantiate(poff, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void Reset()
    {
        initalized = false;
        isSuspended = false;
        Destroy(gameObject);
    }

    public void Suspend(bool suspend)
    {
        isSuspended = suspend;
    }

    public void On_SuspensionEvent(bool suspend)
    {
        Suspend(suspend);
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_ExitPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;
        SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;

        RegisterBullet(this);
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_ExitPlayMode -= Reset;
        GameMaster.On_LevelReset -= On_LevelReset;
        SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;

        UnRegisterBullet(this);
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
		transform.DOKill();
		foreach(Tween t in tweens)
			t.Kill();

		tweens = new List<Tween>();
	}

    public static void ClearBulletList()
    {
        bullets = new List<Bullet>();
    }

    public static int GetBulletCount()
    {
        return bullets.Count;
    }

    public static void RegisterBullet(Bullet bullet)
    {
        bullets.Add(bullet);
    }

    public static void UnRegisterBullet(Bullet bullet)
    {
        if(bullets.Contains(bullet))
            bullets.Remove(bullet);
    }

    public static void KillFirst()
    {
        if(bullets == null || bullets.Count == 0)
            return;

        Bullet b = bullets.FirstOrDefault();

        if(b != null)
        {
            b.DestroyBullet();
        }
    }
}
