using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafariForever.Toolbar;
using FSM_CharacterController2D;
using StandardCharacterController2D;
using DG.Tweening;

public class Portals : MonoBehaviour, ISuspendable
{
    public Portal entrance;
    public Portal exit;
    public LayerMask whatIsPlatform;
    
    public GameObject particlePrefab;
    public LineRenderer lineRenderer;

	private LevelEntity entity;
    
	private PositionGizmo positionGizmo;
	private Vector2 offset = new Vector2(0.5f, 0.5f);
	private Vector2 pointA;
    private Vector2 pointB;

    private bool isOpen = true;
    private int framesOfPower;
    private bool isSuspended;

    public List<TransportingObject> transportingObjects = new List<TransportingObject>();

    public class TransportingObject
    {
        public Transform movingObject;
        public Collider2D collider;
        public ParticleSystem particleSystem;
        public AudioPlayer audioPlayer;
        public List<Tween> tweens = new List<Tween>();
        public int frameOfEntry;
        public ITeleportable teleportableObject;

        public void Cancel()
        {
            KillTweens();

            if(movingObject != null)
                movingObject.localScale = Vector3.one;
            
            if(collider != null)
                collider.enabled = true;

            if(particleSystem != null)
                Destroy(particleSystem.gameObject);

            //audioPlayer.Kill();
            //audioPlayer.DOFadeAndKill(0.1f, Ease.InSine);
        }

        private void KillTweens()
        {
            foreach(Tween tween in tweens)
            {
                tween?.Kill();
            }
            tweens = new List<Tween>();
        }

		public void Suspend(bool suspend)
		{
			foreach(Tween tween in tweens)
            {
				if(suspend)
                	tween?.Pause();
				else
					tween?.Play();
            }
		}
    }

    public void OnEnterPortal(Collider2D collider, Portal portal, ITeleportable teleportableObject)
    {
        if (!collider.enabled)
            return;

        collider.enabled = false;

        portal.PunchPortal();

        TransportingObject tO = new TransportingObject();
        tO.teleportableObject = teleportableObject;
        tO.collider = collider;
        tO.frameOfEntry = Time.frameCount;

        transportingObjects.Add(tO);

        FSM_CharacterController playerController = tO.collider.GetComponent<FSM_CharacterController>();
        CharacterController2D characterController2D = tO.collider.GetComponent<CharacterController2D>();


        if (playerController != null)
        {
            playerController.Suspend(true);	
            playerController.motion.isBeingTransported = true;
        }
        
        //characterController2D?.Suspend(true);

        if(characterController2D != null)
            characterController2D.enabled = false;

        //Play OnEnterSound
        Audio.Play(SFX.instance.level.warp.enter, Channel.Game);
        //tO.audioPlayer = Audio.Play(SFX.instance.level.warp.travel, Channel.Game).SetVolume(0f);
		//tO.audioPlayer.source.audioSource.DOFade(SFX.instance.level.warp.travel.defaultVolume, 0.1f).SetEase(Ease.OutSine);

        //Shrink Object
        tO.movingObject = playerController != null ? playerController.centerPivot : tO.collider.transform;
		tO.movingObject.DOKill();
		tO.tweens.Add(tO.movingObject.DOScale(Vector2.zero, 0.3f / (SaveManager.currentSave.gameSpeed + 0.1f)).SetEase(Ease.InBack));

        //Move object to center of portal.
        Vector3 targetPostion = portal.transform.position + tO.teleportableObject.GetPortalOffset();
		tO.tweens.Add(tO.collider.transform.DOMove(targetPostion, 0.4f / (SaveManager.currentSave.gameSpeed + 0.1f)).SetEase(Ease.OutQuad).OnComplete(()=>
		{
            if(tO.collider == null)
            {
                transportingObjects.Remove(tO);
                return;
            }

            //Create new particles!
            GameObject newParticles = Instantiate(particlePrefab, portal.transform.position, Quaternion.identity, transform);
            ParticleSystem ps = newParticles.GetComponent<ParticleSystem>();

            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(portal.color, Color.white); 
            
			ps.transform.DOKill();
			ps.transform.position = portal.transform.position;
			ps.gameObject.SetActive(true);

			Vector3 exitPos;
			if(portal == entrance)
				exitPos = exit.transform.position;
			else
				exitPos = entrance.transform.position;
		
			//float movementSpeed = characterController != null ? characterController.properties.movementSpeed : characterController2D.GetMovementSpeed();
			tO.tweens.Add(ps.transform.DOMove(exitPos, 12f * (SaveManager.currentSave.gameSpeed + 0.1f)).SetSpeedBased(true).SetEase(Ease.Linear).OnComplete(()=>
			{
                if(tO.collider == null)
                {
                    transportingObjects.Remove(tO);
                    return;
                }

				ps.Stop();
                tO.tweens.Add(DOVirtual.DelayedCall(0.25f/ (SaveManager.currentSave.gameSpeed + 0.1f), ()=>{
                    if(ps != null)
                        Destroy(ps.gameObject);

                    transportingObjects.Remove(tO);
                }));

                //move object
                tO.collider.enabled = true;
				tO.collider.transform.position = exitPos + tO.teleportableObject.GetPortalOffset();

				Audio.Play(SFX.instance.level.warp.exit, Channel.Game);
                
                if(characterController2D != null)
                    characterController2D.enabled = true;

                if(playerController != null)
                {
                    playerController.motion.isBeingTransported = false;
                    playerController.Suspend(false);
                    playerController.motion.rawVelocity.y = 0;
                    playerController.motion.timeSpentFluttering = 0f;
                    playerController.motion.timeOfLastJumpAttempt = 0f;
                    playerController.stateController.SetState(State.Falling);
                }
                
                characterController2D?.MotionController.SetInternalVelocity(Vector2.zero);
                characterController2D?.StateController.SetState(new StandardCharacterController2D.Spikeball.V1.Falling());

                tO.movingObject.DOKill();
                tO.tweens.Add(tO.movingObject.DOScale(Vector2.one, 0.3f/ (SaveManager.currentSave.gameSpeed + 0.1f)).SetEase(Ease.OutQuad));

                //DrawGizmo(exitPos + tO.teleportableObject.GetPortalOffset() + new Vector3(0, tO.teleportableObject.GetSize().y*0.5f, 0), tO.teleportableObject.GetSize());

                if (Physics2D.OverlapBox(exitPos + tO.teleportableObject.GetPortalOffset() + new Vector3(0, tO.teleportableObject.GetSize().y*0.5f, 0), tO.teleportableObject.GetSize() * 0.5f, 0, whatIsPlatform))
                {
                    //Return back to enterance!
                    OnEnterPortal(tO.collider, portal == entrance ? exit : entrance, teleportableObject);
                }
            }));
		}));
    }

    private void DrawGizmo(Vector3 pos, Vector2 size)
    {
        Vector3 halfSize = size/2f;
        Vector3 bottomLeftCorner = pos - halfSize;
        Vector3 topRightCorner = pos + halfSize;
        Vector3 bottomRightCorner = pos + new Vector3(halfSize.x, -halfSize.y);
        Vector3 topLeftCorner = pos + new Vector3(-halfSize.x, halfSize.y);

        Debug.DrawRay(bottomLeftCorner, new Vector2(size.x, 0), Color.red, 3);
        Debug.DrawRay(topRightCorner, new Vector2(-size.x, 0), Color.red, 3);

        Debug.DrawRay(bottomRightCorner, new Vector2(0, size.y), Color.red, 3);
        Debug.DrawRay(topLeftCorner, new Vector2(0, -size.y), Color.red, 3);
    }

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
		positionGizmo = entity.GetComponentInChildren<PositionGizmo>();

        entrance.Initalize(this);
        exit.Initalize(this);

		pointA = entity.GetSerializableData().topRight - offset;
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;

        exit.transform.position = pointB;

        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});
        UpdateLineRendererVisibility(Toolbar.instance != null && Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
	}

    private void UpdatePosition()
    {
        pointB = positionGizmo.transform.position;
        exit.transform.position = pointB;
        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});
        LevelBuilder.instance.UpdateTiles();
    }

    private void Reset()
    {
        SetIsOpen(true);
        framesOfPower = 0;
        CancelAllTransportations();
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

        if(framesOfPower >= 2 || !entity.inputNode.HasConnections())
        {
            SetIsOpen(true);
        }
        else
        {   
            SetIsOpen(false);
        }
    }

    private void SetIsOpen(bool open, bool instant = false)
    {
        if(open)
        {
            if(!isOpen)
            {
                entrance.Open(instant);
                exit.Open(instant);
                isOpen = true;
            }
        }
        else
        {
            if(isOpen)
            {
                entrance.Close(instant);
                exit.Close(instant);
                isOpen = false;
            }
        }
    }

    private void On_EnterPlayMode()
    {
        UpdateLineRendererVisibility(false);
        Reset();
        lineRenderer.enabled = false;
    }

    private void On_ExitPlayMode()
    {
        CancelAllTransportations();
        Reset();

        UpdateLineRendererVisibility(Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
        entrance.Open(true);
        exit.Open(true);
    }

    private void UpdateLineRendererVisibility(bool interactable)
    {
        lineRenderer.enabled = interactable;
    }

    private void On_TabChange(int tabIndex)
    {
        UpdateLineRendererVisibility(tabIndex == (int)entity.requiredTab);
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;

        positionGizmo.On_PositionMoved += UpdatePosition;

        Toolbar.On_TabChange += On_TabChange;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;


        //levelEntity.inputNode.On_BecamePowered += On_BecamePowered;
        //levelEntity.inputNode.On_LostPower += On_LostPower;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

        positionGizmo.On_PositionMoved -= UpdatePosition;

        Toolbar.On_TabChange -= On_TabChange;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;

        //levelEntity.inputNode.On_BecamePowered -= On_BecamePowered;
        //levelEntity.inputNode.On_LostPower -= On_LostPower;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        CancelAllTransportations();
    }

    private void CancelAllTransportations()
    {
        entrance.KillAllTweens();
        exit.KillAllTweens();

        foreach(TransportingObject transportingObject in transportingObjects)
        {
            transportingObject.Cancel();
        }
        transportingObjects = new List<TransportingObject>();
    }

    public bool ObjectIsBeingTransported(Collider2D other, out TransportingObject tO)
	{
		for(int i = 0; i < transportingObjects.Count; i++)
		{
			if(transportingObjects[i].collider == other)
			{
				tO = transportingObjects[i];
				return true;
			}
		}
		
		tO = null;
		return false;
	}

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
        this.isSuspended = suspend;

		foreach(TransportingObject tO in transportingObjects)
		{
			tO.Suspend(this.isSuspended);
		}
	}
}
