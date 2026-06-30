using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;
using SafariForever.Toolbar;

public class Bubble : MonoBehaviour, ISuspendable
{
    public GameObject particlePrefab;
    public float speed = 10f;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsSolidPlatform;
    public Transform graphic_1;
    public Transform graphic_2;
    public Transform anchor;

    public BoxCollider2D safeArea;

    public Rotate rotator;

    public CircleCollider2D circleCollider2D;
    public LineRenderer lineRenderer;
    
    public FSM_CharacterController controller;
    private Transform controllerParent;

    private LevelEntity entity;
    private PositionGizmo positionGizmo;

    private List<Tween> tweens = new List<Tween>();
    private bool popped = false;

    private float sizeMulitplier = 1f;
    private float animationSpeed = 0f;
    private int spinDirection = 1;
    private float time;
    private float position;
    private int direction = 1;
    private float respawnTime = 2f;
    private float timeOfPopAttempt = -1f;
    
    private bool isSuspended;
    private int framesOfPower;

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 offset = new Vector2(0.5f, 0.5f);

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
		positionGizmo = entity.GetComponentInChildren<PositionGizmo>();

        pointA = entity.GetSerializableData().topRight - offset;
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;

        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});
        UpdateLineRendererVisibility(Toolbar.instance != null && Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
    }



    private void OnTriggerEnter2D(Collider2D other)
	{
		OnTriggerStay2D(other);
	}

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.enabled == false)
            return;

        if(!GameMaster.instance.IsPlaying())
            return;

        if (popped)
            return;

        if(!whatIsPlayer.Contains(other.gameObject.layer))
            return;

        FSM_CharacterController controller = other.GetComponent<FSM_CharacterController>();

		if(controller.character.IsDead())
			return;

        EnterBubble(controller);       
    }

    public void EnterBubble(FSM_CharacterController characterController2D)
    {
        if(characterController2D.stateController.currentState.Equals(State.InsideBubble))
            return;

        KillAllTweens();

        controller = characterController2D;
        controller.stateController.OverideState(State.InsideBubble);
        controller.enabled = false;
        
        circleCollider2D.enabled = false;

        anchor.eulerAngles = Vector3.zero;

        controllerParent = characterController2D.transform.parent;
        characterController2D.transform.SetParent(anchor);
        
        spinDirection = -controller.motion.runningDirection;

        //Scale bubble
        tweens.Add(DOTween.To(()=> sizeMulitplier, x=> sizeMulitplier = x, 1.5f, 0.3f).SetEase(Ease.OutBack));

        animationSpeed = 1f;
        tweens.Add(DOTween.To(()=> animationSpeed, x=> animationSpeed = x, 0f, 4f).SetEase(Ease.OutQuad));
        //tweens.Add(graphic_1.DOPunchScale(Vector3.one * 0.3f, 1f, 5).SetEase(Ease.OutBack));

        rotator.enabled = true;
    }

    public void Pop()
    {
        controller.transform.SetParent(controllerParent);
        controllerParent = null;

        controller.transform.position = transform.position - (Vector3) controller.boxCollider2D.offset;

        controller.enabled = true;
        
        controller.stateController.OverideState(State.Jumping);

        if(!PositionSafe())
        {
            if (controller.boxCollider2D.enabled == false)
                return;

            controller.character.Hurt();
        }

        controller = null;
        graphic_2.gameObject.SetActive(false);
        popped = true;
        timeOfPopAttempt = -1f;

        Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);

        tweens.Add(DOVirtual.DelayedCall(respawnTime/(SaveManager.currentSave.gameSpeed + 0.1f), ()=>{
            Respawn();
        }));
    }

    public bool PositionSafe()
    {
        Vector2 halfsize =  (controller.boxCollider2D.size/2f) + new Vector2(RaycastInfo.skinWidth, RaycastInfo.skinWidth);
        Vector2 bottomCorner = (Vector2)transform.position - halfsize;
        Vector3 topCorner  = (Vector2)transform.position + halfsize;
        Collider2D collider = Physics2D.OverlapArea(bottomCorner, topCorner, whatIsSolidPlatform);

        return !collider;
    }


    public void Respawn()
    {
        //graphic_1.transform.eulerAngles = Vector3.zero;
        //graphic_1.transform.DOScale(Vector3.one, 0.5f);

        KillAllTweens();

        sizeMulitplier = 0f;
        tweens.Add(DOTween.To(()=> sizeMulitplier, x=> sizeMulitplier = x, 1f, 0.6f).SetEase(Ease.OutBack));

        animationSpeed = 1f;
        tweens.Add(DOTween.To(()=> animationSpeed, x=> animationSpeed = x, 0f, 4f).SetEase(Ease.OutQuad));

        graphic_2.gameObject.SetActive(true);
        circleCollider2D.enabled = true;
        popped = false;
    }

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        circleCollider2D.enabled = true;
    }

    private void Reset()
    {
        KillAllTweens();

        graphic_2.gameObject.SetActive(true);
        graphic_2.localScale = Vector3.one;
        graphic_1.localScale = Vector3.one;
        circleCollider2D.enabled = false;
        anchor.eulerAngles = Vector3.zero;
        popped = false;
        sizeMulitplier = 1f;
        spinDirection = 1;
        animationSpeed = 0f;
        rotator.enabled = false;
        time = 0f;
        framesOfPower = 0;
        transform.position = pointA;
        position = 0;
        direction = 1;
        controller = null;
        controllerParent = null;
    }

    private void UpdatePosition()
    {
        pointB = positionGizmo.transform.position;
        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});
    }

    private void On_EnterPlayMode()
    {
        UpdateLineRendererVisibility(false);
    }

    private void UpdateLineRendererVisibility(bool interactable)
    {
        lineRenderer.enabled = interactable;
    }

    private void On_LevelReset(bool manual)
    {
        Reset();
    }

    private void On_ExitPlayMode()
    {
        Reset();
        UpdateLineRendererVisibility(Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
    }

    private void On_TabChange(int tabIndex)
    {
        UpdateLineRendererVisibility(tabIndex == (int)entity.requiredTab);
    }

    private void On_LogicCanvasUpdate(bool visible)
    {
        if(visible || GameMaster.instance.IsPlaying())
        {
            UpdateLineRendererVisibility(false); 
        }
        else
        {  
            UpdateLineRendererVisibility(Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
        }      
    }


    private void Update()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(isSuspended)
			return;

        time += Time.deltaTime * Mathf.Lerp(1f, 10f, animationSpeed) * SaveManager.currentSave.gameSpeed;
        graphic_2.localScale = new Vector3(EaseCurve.InSine(1f, 1.2f, Mathf.Abs(Mathf.Sin(time))) * sizeMulitplier , EaseCurve.InSine(1f, 1.2f, Mathf.Abs(Mathf.Cos(time + (Mathf.PI/1.5f)))) * sizeMulitplier, 1);
        rotator.speed = Mathf.Lerp(25f, 250f, animationSpeed) * spinDirection;

        if(framesOfPower >= 2 || !entity.inputNode.HasConnections())
        {
            float distance = Vector2.Distance(pointA, pointB);
            position += Time.deltaTime * SaveManager.currentSave.gameSpeed * (speed/distance) * direction;
            if(position > 1 && direction == 1)
            {
                position = 1;
                direction = -1;
            }
            else if(position < 0 && direction == -1)
            {
                position = 0;
                direction = 1;
            }

            transform.position = InOutSine(pointA, pointB, position);
        }

        //Position player
        if(controller == null)
            return;

        Vector3 targetPositon = new Vector3(0.04f, -0.5f, 0);

        controller.transform.localPosition = Vector3.MoveTowards(controller.transform.localPosition, targetPositon, Time.deltaTime * controller.properties.movementSpeed * SaveManager.currentSave.gameSpeed);
    
        //Input
        if(CharacterInput.On_TouchStart(0))
        {
            timeOfPopAttempt = Time.time;
        }

        if(timeOfPopAttempt > 0f)
        {
            if((Time.time - timeOfPopAttempt) < controller.properties.bubbleWiggleRoomSeconds)
            {
                if(PositionSafe())
                {
                    Pop();
                }
            }
            else
            {
                Pop();
            }
        }
    }

    private Vector2 InOutSine(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(EaseCurve.InOutSine(a.x, b.x, t), EaseCurve.InOutSine(a.y, b.y, t));
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

    private void OnEnable()
    {
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        positionGizmo.On_PositionMoved += UpdatePosition;

        Toolbar.On_TabChange += On_TabChange;
        LogicCanvas.On_LogicCanvasUpdate += On_LogicCanvasUpdate;

        SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        positionGizmo.On_PositionMoved -= UpdatePosition;

        Toolbar.On_TabChange -= On_TabChange;
        LogicCanvas.On_LogicCanvasUpdate -= On_LogicCanvasUpdate;

        SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
    }

    private void KillAllTweens()
    {
        foreach(Tween t in tweens)
            t?.Kill();

        tweens = new List<Tween>();
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

    public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
	}
}
