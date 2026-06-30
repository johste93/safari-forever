using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using StandardCharacterController2D;

public class ConveyourBelt : MonoBehaviour, ISuspendable
{
    public GameObject beltPrefab;
    public Transform child;

    private LevelEntity levelEntity;
    
    private Vector2Int[] nodes = new Vector2Int[4];

    private Transform[] belts;
    private int[] targetNode;
    
    private const float speed = 1f;
    private const int numberOfBeltsPrUnit = 4;

    public BoxCollider2D boxCollider2D;
    private new bool enabled = false;

    private ContactPoint2D[] contacts = new ContactPoint2D[6];

	private bool isSuspended;
    private int framesOfPower;

    private FSM_CharacterController controller;
    private int lastActiveFrame;

    //private List<CharacterController2D> characterController2Ds = new List<CharacterController2D>();
    private Dictionary<GameObject, int> objectsOnThreadThisFrame = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, int> cache = new Dictionary<GameObject, int>();

    private LayerMask mask = new LayerMask();

    private void Awake()
    {
        levelEntity = GetComponentInParent<LevelEntity>();

        mask = mask.CombineLayerMask(Globals.gameConstants.whatIsPlayer);
        mask = mask.CombineLayerMask(LayerMask.GetMask("Enemy"));
    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        if (!enabled)
            return;

        if (!mask.Contains(other.gameObject.layer))
            return;

        if(levelEntity.inputNode.HasConnections() && framesOfPower < 2)
            return;

        if(!objectsOnThreadThisFrame.ContainsKey(other.gameObject))
        {
            //Debug.Log("Other is on converyor!");
            objectsOnThreadThisFrame.Add(other.gameObject, Time.frameCount);
        }
        else
            objectsOnThreadThisFrame[other.gameObject] = Time.frameCount;

        controller = other.gameObject.GetComponent<FSM_CharacterController>();

        
        if(controller != null)
        {
            if(controller.stateController.currentState.Equals(State.StuckInSlime) || controller.stateController.currentState.Equals(State.StuckInSlimeOnWall) || controller.stateController.currentState.Equals(State.StuckInSlimeOnRoof))
                return;

            switch (GetCollisionRelativeDirection((BoxCollider2D)other.collider))
            {            
                case Direction4.Down:
                    controller.motion.conveyorSpeed = new Vector2((int)levelEntity.GetSerializableData().gizmoRotation * 3, 0);
                break;
                case Direction4.Right:
                    controller.motion.conveyorSpeed = new Vector2(0, ((int)levelEntity.GetSerializableData().gizmoRotation * GetSpeedAdjustedGravity()));
                break;
                case Direction4.Left:
                    controller.motion.conveyorSpeed = new Vector2(0, (int)levelEntity.GetSerializableData().gizmoRotation * -GetSpeedAdjustedGravity());
                break;
            }
            return;
        }
        CharacterController2D characterController2D = other.gameObject.GetComponent<CharacterController2D>();
        if(characterController2D != null)
        {
            switch (GetCollisionRelativeDirection((BoxCollider2D)other.collider))
            {
                case Direction4.Down:
                    characterController2D.MotionController.SetConveyorVelocity(new Vector2((int)levelEntity.GetSerializableData().gizmoRotation * 3, 0));
                    break;
                case Direction4.Right:
                    characterController2D.MotionController.SetConveyorVelocity(new Vector2(0, (int)levelEntity.GetSerializableData().gizmoRotation * GetSpeedAdjustedGravity()));
                    break;
                case Direction4.Left:
                    characterController2D.MotionController.SetConveyorVelocity(new Vector2(0, (int)levelEntity.GetSerializableData().gizmoRotation * -GetSpeedAdjustedGravity()));
                    break;
            }
            return;
        }
    }

    private float GetSpeedAdjustedGravity()
    {
        float baseSpeed = 2.5f;
        float speedStep = -(0.307f/3f);

        float gameSpeed = SaveManager.currentSave.gameSpeed;

        if(gameSpeed > 0.55f && gameSpeed <= 0.65f)
            return baseSpeed + (speedStep * 3);

        if(gameSpeed > 0.65f && gameSpeed <= 0.75f)
            return baseSpeed + (speedStep * 2);

        if(gameSpeed > 0.75f && gameSpeed <= 0.85f)
            return baseSpeed + (speedStep * 1);

        return baseSpeed;
    }

    private Direction4 GetCollisionRelativeDirection(BoxCollider2D other)
    {
        //Debug.Log("Top: " + other.bounds.min.y + " >= " + boxCollider2D.bounds.max.y + "==" + (other.bounds.min.y > boxCollider2D.bounds.max.y || Mathf.Approximately(other.bounds.min.y , boxCollider2D.bounds.max.y)));
        if (other.bounds.min.y > boxCollider2D.bounds.max.y || CustomApproximately(other.bounds.min.y , boxCollider2D.bounds.max.y, 0.001f))
        {
            //Debug.Log("Oher is above");
            return Direction4.Down; //Other is above
        }
        
        if (other.bounds.max.y < boxCollider2D.bounds.min.y || CustomApproximately(other.bounds.max.y, boxCollider2D.bounds.min.y, 0.001f))
        {
            //Debug.Log("Other is below");
            return Direction4.Up; //Other is below
        }

        if (other.bounds.max.x < boxCollider2D.bounds.min.x || CustomApproximately(other.bounds.max.x, boxCollider2D.bounds.min.x, 0.001f))
        {
            //Debug.Log("Other is Left");
            return Direction4.Right; //Other is on the left side of conveyor
        }

        //Debug.Log("Other is Right");
        return Direction4.Left; //Other is on the right side of conveyor;
    }

    private bool CustomApproximately(float a, float b, float tolerance)
    {
        return (double) Mathf.Abs(b - a) <= tolerance;
    }

    private void Start()
    {
        LevelEntity.SerializableData temp = levelEntity.GetSerializableData();

        nodes[0] = temp.topRight;
        nodes[1] = new Vector2Int(temp.topRight.x, temp.bottomLeft.y);
        nodes[2] = temp.bottomLeft;
        nodes[3] = new Vector2Int(temp.bottomLeft.x, temp.topRight.y);

        child.localScale = new Vector3(Vector2.Distance(nodes[1], nodes[2]), Vector2.Distance(nodes[0], nodes[1]), 1);
        boxCollider2D.size = new Vector2(child.localScale.x, child.localScale.y);

        float totalLength = 0f;
        for(int i = 0; i < nodes.Length-1; i++)
        {
            totalLength += Vector2.Distance(nodes[i], nodes[i+1]);
        }
        totalLength += Vector2.Distance(nodes[nodes.Length-1], nodes[0]);

        
        int numberOfBelts = numberOfBeltsPrUnit * Mathf.RoundToInt(totalLength);

        float distanceBetweenNodes = totalLength/numberOfBelts;
        //Debug.Log("length: " + totalLength + " belts: " + numberOfBelts + " distance: " + distanceBetweenNodes);

        //int numberOfBelts = Mathf.FloorToInt(totalLength / distanceBetweenNodes);
        belts = new Transform[numberOfBelts];
        targetNode = new int[numberOfBelts];
        Vector3 startPosition = (Vector2) nodes[0];
        for(int i = 0; i < numberOfBelts; i++)
        {
            GameObject lastSpawn = Instantiate(beltPrefab, startPosition, Quaternion.identity, transform);
            lastSpawn.name = i.ToString();
            belts[i] = lastSpawn.transform;

            if(i % 2 == 0)
                belts[i].localScale += new Vector3(0.1f,0.1f,0f);

            int startNode = GetStartNode(i);
            startNode += (int)((RotationGizmo)levelEntity.GetDragHandle()).rotation;
            
            if(startNode >= nodes.Length)
                startNode = 0;

            if(startNode < 0)
                startNode = nodes.Length-1;

            targetNode[i] = startNode;
            startPosition = Vector2.MoveTowards(startPosition, (Vector2) nodes[startNode], distanceBetweenNodes);
        }
    }

    private void Update()
    {
        if(!enabled)
            return;

		if(isSuspended)
			return;

        if(levelEntity.inputNode.HasConnections() && framesOfPower < 2)
            return;

        Rotate();
    }

    private void Rotate()
    {
        for(int i = 0; i < belts.Length; i++)
        {
            belts[i].position = Vector2.MoveTowards(belts[i].position, (Vector2)nodes[targetNode[i]], Time.deltaTime * SaveManager.currentSave.gameSpeed * speed);
            if(Vector2.Distance(belts[i].position,nodes[targetNode[i]]) < Mathf.Epsilon)
            {
                targetNode[i] += (int)levelEntity.GetSerializableData().gizmoRotation;
                if(targetNode[i] >= nodes.Length)
                    targetNode[i] = 0;

                if(targetNode[i] < 0)
                    targetNode[i] = nodes.Length-1;
            }
        }
    }

    private int GetStartNode(int beltIndex)
    {   
        if(levelEntity.GetSerializableData().gizmoRotation == Rotation.Clockwise)
        {
            int beltsPassed = 0;
            for(int i = 0; i < nodes.Length; i++)
            {
                int targetNode = i+1;
                if(targetNode >= nodes.Length)
                    targetNode = 0;

                //Debug.Log("start: " + i + " target: " + targetNode);

                int distance = Mathf.RoundToInt(Vector2.Distance(nodes[i], nodes[targetNode]));
                int beltsOnDistance = distance * numberOfBeltsPrUnit;

                if(beltIndex < beltsOnDistance + beltsPassed)
                    return i;

                beltsPassed += beltsOnDistance;
            }
            return nodes.Length-1;
        }
        else
        {
            int beltsPassed = 0;
            for(int i = nodes.Length; i >= 0; i--)
            {
                int startNode = i == 4 ? 0 : i;
                int targetNode = i-1;
                if(targetNode < 0)
                    targetNode = nodes.Length-1;

                //Debug.Log("start: " + startNode + " target: " + i);

                int distance = Mathf.RoundToInt(Vector2.Distance(nodes[startNode], nodes[targetNode]));
                int beltsOnDistance = distance * numberOfBeltsPrUnit;

                if(beltIndex < beltsOnDistance + beltsPassed)
                    return i;

                beltsPassed += beltsOnDistance;
            }
            return 0;
        }
    }

    private void On_EnterPlayMode()
    {
        enabled = true;
        Reset();
    }

    private void On_ExitPlayMode()
    {
        enabled = false;
        UpdateObjectsOnThread(true);
    }

    private void Reset()
    {
        framesOfPower = 0;
        UpdateObjectsOnThread(true);
    }

    private void On_LevelReset(bool manual)
    {
        Reset();
    } 

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

		if(isSuspended)
			return;

        if(levelEntity.inputNode.HasConnections() && levelEntity.inputNode.IsPowered())
        {
            framesOfPower++;
        }
        else
        {
            if(framesOfPower > 0)
                On_LostPower();

            framesOfPower = 0;
        }
    }

    private void FixedUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

		if(isSuspended)
			return;

        this.DelayEndOfFrame(()=>{
            UpdateObjectsOnThread();
        }); 
    }

    private void UpdateObjectsOnThread( bool removeAll = false)
    {
        cache = new Dictionary<GameObject, int>(objectsOnThreadThisFrame);
        foreach(KeyValuePair<GameObject, int> kVP in cache)
        {
            if(kVP.Value != Time.frameCount || removeAll)
            {
                FSM_CharacterController player = kVP.Key.GetComponent<FSM_CharacterController>();
                if(player != null)
                    player.motion.conveyorSpeed = Vector2.zero;

                CharacterController2D characterController2D = kVP.Key.GetComponent<CharacterController2D>();
                if (characterController2D != null)
                    characterController2D.MotionController.SetConveyorVelocity(Vector2.zero);

                objectsOnThreadThisFrame.Remove(kVP.Key);
                //Debug.Log("Other left the converyor!");
            }
        }
    }

    private void On_LostPower()
    {
        UpdateObjectsOnThread(true);
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
