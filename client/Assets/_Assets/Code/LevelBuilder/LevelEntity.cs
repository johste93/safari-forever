using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using SafariForever.Toolbar;
using SF.LogicSystem.v2;

public class LevelEntity : MonoBehaviour
{
    [HideInInspector]public string id = System.Guid.NewGuid().ToString();
    public string uniqueId{ get{ return serializableData.uniqueId; } }
    public bool unique;
    
    public GameObject prefab;
    public bool repeat;

    public bool cameraScaleIgnore;
    public bool hasOutline;

    public Vector2 minSize = Vector2.one;

    public LevelEntityBehaviour behaviour;
    public ToolbarTab requiredTab;

    public bool fixedCenter;
    public AllowedResizingDirections allowedResizingDirections;

    public bool resizeUp;
    public bool resizeRight;
    public bool resizeDown;
    public bool resizeLeft;

    public SerializableData serializableData = new SerializableData();
    
    public LineRenderer lineRenderer;
    public Material defaultMaterial;
    public Material dottedMaterial;
    public LineRenderer dottedLineRenderer;

    public Gizmo gizmo;
    public InputNode inputNode;
    public OutputNode outputNode;

    public RailNode_Input railNodeInput;
    public RailNode_Output railNodeOutput;

    public SizeGizmo[] sizeGizmos;
    public ScaleGizmo[] scaleGizmos;
    float scaleGizmoOffset = 1f;

    public Vector2[] excludedTiles;
    public bool allowBottomLinePlacement;

    //These Values fixes a bug where scaling the size pushes the oposit corner around.
    private float topRightYCache;
    private float topRightXCache;
    private float bottomLeftYCache;
    private float bottomLeftXCache;

    private List<Direction4> directionsGizmosAllowedToMove;

    public delegate void EntityEvent(LevelEntity entity);
    public static EntityEvent On_EntitySelected;
    public static EntityEvent On_EntityMoved;
    public static EntityEvent On_EntityStoppedMoving;
    public static EntityEvent On_EntityStartedChangingSize;
    public static EntityEvent On_EntityChangedSize;
    public static EntityEvent On_EntityStoppedChangingSize;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private Vector2 cachedPosition = new Vector2(-2147483648,-2147483648);
    private bool isSelected = true;

    public delegate void DestroySelfEvent();
    public DestroySelfEvent On_DestroySelf;

    private void Awake()
    {
        Initalize();
    }

    private void Initalize()
    {
        serializableData.bottomLeft = Vector2Int.zero;
        serializableData.topRight = minSize.RoundVector2ToInt();

        foreach(ScaleGizmo gizmo in scaleGizmos)
        {
            gizmo.Initalize(this);
            gizmo.gameObject.SetActive(
                (gizmo.scaleDirection == Direction4.Up && resizeUp) ||
                (gizmo.scaleDirection == Direction4.Right && resizeRight) ||
                (gizmo.scaleDirection == Direction4.Down && resizeDown) ||
                (gizmo.scaleDirection == Direction4.Left && resizeLeft)
            );
        }

        //Reset Cache To prevent resize on drop!
        topRightYCache = int.MaxValue;
        topRightXCache = int.MaxValue;
        bottomLeftYCache = int.MinValue;
        bottomLeftXCache = int.MinValue;

        SetColor(ColorGenerator.GetRandomColor(0.75f));

        UpdateScaleGizmoSize();

        if(gizmo != null)
            gizmo.Initalize(this);
            gizmo.Initalize(this);

        inputNode?.Initalize(this);
        outputNode?.Initalize(this);

        railNodeInput?.Initalize(this);
        railNodeOutput?.Initalize(this);
    }

    private void OnEnable()
    { 
        UpdateCenterPosition();
        UpdateLineRenderer();
        LevelBuilder.instance.UpdateTiles();
        UpdateScaleGizmoPositions();
        UpdateContent();

        lineRenderer.enabled = hasOutline;

        LevelBuilder.instance.RegisterLevelEntity(this);

        Subscribe();
    }

    private void UpdateContent()
    {
        gizmo.Scale(new Vector2(GetWidth(), GetHeight()));

        if(prefab == null)
            return;

        DestroyContent();

        if(repeat)
        {
            List<Vector2> positions = GetPositionsInside();
            foreach(Vector2 pos in positions)
            {
                spawnedObjects.Add(Instantiate(prefab, (Vector2)pos, Quaternion.identity, transform));
            }
        }
        else
        {
            spawnedObjects.Add(Instantiate(prefab, serializableData.centerPosition, Quaternion.identity, transform));
        }
    }

    public List<Vector2> GetPositionsInside()
    {
        List<Vector2> result = new List<Vector2>();

        for(int x = 0; x < GetWidth(); x++)
        {
            for(int y = 0; y < GetHeight(); y++)
            {
                result.Add(new Vector2(serializableData.bottomLeft.x + x, serializableData.bottomLeft.y + y) + Globals.gameConstants.tileOffset);
            }
        }

        if(gizmo.includeInPositionsInside)
        {
            if(GetSerializableData().gizmoOffset != null)
            {
                result.Add((Vector2)gizmo.transform.position + GetSerializableData().gizmoOffset.ToVector2());
            }
        }

        return result;
    }

    public List<Vector2> GetExcludedTilePositionsInside()
    {
        List<Vector2> result = new List<Vector2>();

        for(int x = 0; x < GetWidth(); x++)
        {
            for(int y = 0; y < GetHeight(); y++)
            {
                if(excludedTiles.Contains(new Vector2(x,y)))
                    result.Add(new Vector2(serializableData.bottomLeft.x + x, serializableData.bottomLeft.y + y) + Globals.gameConstants.tileOffset);
            }
        }

        return result;
    }

    public void UpdateCenterPosition()
    {
        if(fixedCenter)
        {
            gizmo.transform.position = new Vector3(serializableData.centerPosition.x, serializableData.centerPosition.y, gizmo.transform.position.z);
        }
        else
        {
            Vector2 newCenterPosition = new Vector2(Mathf.Lerp(serializableData.bottomLeft.x, serializableData.topRight.x, 0.5f), Mathf.Lerp(serializableData.bottomLeft.y, serializableData.topRight.y, 0.5f));
            serializableData.centerPosition = newCenterPosition;
            gizmo.transform.position = new Vector3(serializableData.centerPosition.x, serializableData.centerPosition.y, gizmo.transform.position.z);
		}

        if(inputNode != null)
            inputNode.transform.position = gizmo.transform.position;

        if(outputNode != null)
            outputNode.transform.position = gizmo.transform.position;

        if(railNodeInput != null)
            railNodeInput.transform.position = gizmo.transform.position;

        if(railNodeOutput != null)
            railNodeOutput.transform.position = gizmo.transform.position;
    }

    public void UpdateScaleGizmoPositions()
    {        
        scaleGizmos[0].transform.position = new Vector2(serializableData.centerPosition.x, serializableData.topRight.y + scaleGizmoOffset);     
        scaleGizmos[1].transform.position = new Vector2(serializableData.topRight.x + scaleGizmoOffset, serializableData.centerPosition.y);     
        scaleGizmos[2].transform.position = new Vector2(serializableData.centerPosition.x, serializableData.bottomLeft.y - scaleGizmoOffset);  
        scaleGizmos[3].transform.position = new Vector2(serializableData.bottomLeft.x - scaleGizmoOffset, serializableData.centerPosition.y);  

        UpdateSizeGizmoPositions();

        UpdateDirectionGizmosAllowedToMove();
    }

    public void UpdateSizeGizmoPositions()
    {
        if(sizeGizmos.Length < 2)
            return;

        sizeGizmos[0].transform.position = new Vector2(serializableData.bottomLeft.x - scaleGizmoOffset, serializableData.centerPosition.y);  
        sizeGizmos[1].transform.position = new Vector2(serializableData.centerPosition.x, serializableData.bottomLeft.y - scaleGizmoOffset);  

        if(sizeGizmos.Length < 4)
            return;

        sizeGizmos[2].transform.position = new Vector2(serializableData.centerPosition.x, serializableData.topRight.y + scaleGizmoOffset);     
        sizeGizmos[3].transform.position = new Vector2(serializableData.topRight.x + scaleGizmoOffset, serializableData.centerPosition.y);   
    }

    public void UpdateLineRenderer()
    {
        Vector3[] positions = new Vector3[]{
            serializableData.bottomLeft.ToVector(),
            new Vector3(serializableData.bottomLeft.x, serializableData.topRight.y),
            serializableData.topRight.ToVector(),
            new Vector3(serializableData.topRight.x, serializableData.bottomLeft.y),
            serializableData.bottomLeft.ToVector()
        };

        SetLineRenderPositions(positions);
    }

    public void SetLineRenderPositions(Vector3[] positions)
    {
        lineRenderer.positionCount = 5;
        lineRenderer.SetPositions(positions);

        for(int i = 0; i < positions.Length; i++)
        {
            positions[i] = positions[i] + new Vector3(0,0,-1);
        }

        dottedLineRenderer.positionCount = lineRenderer.positionCount;
        dottedLineRenderer.SetPositions(positions);
    }

    public void UpdateSize()
    {
        Vector2 halfSize = new Vector2(Globals.gameConstants.levelWidth/2f, Globals.gameConstants.levelHeight/2f);
        Vector2 maxPosition =   halfSize; //Top Right
        Vector2 minPosition =   -halfSize; //Bottom Left

        if(allowBottomLinePlacement)
            minPosition.y -= 1;
        
        //Apply level size constraints
        float topRightY = Mathf.Min(scaleGizmos[0].transform.position.y-1 + 0.01f, maxPosition.y);
        float topRightX = Mathf.Min(scaleGizmos[1].transform.position.x-1 + 0.01f, maxPosition.x);
        float bottomLeftY = Mathf.Max(scaleGizmos[2].transform.position.y+1 + 0.01f, minPosition.y);
        float bottomLeftX = Mathf.Max(scaleGizmos[3].transform.position.x+1 + 0.01f, minPosition.x);

        //Apply minSize constraints to keep us inside levelsize
        topRightY = Mathf.Max(topRightY, minPosition.y + minSize.y);
        topRightX = Mathf.Max(topRightX, minPosition.x + minSize.x);
        bottomLeftY = Mathf.Min(bottomLeftY, maxPosition.y - minSize.y);
        bottomLeftX = Mathf.Min(bottomLeftX, maxPosition.x - minSize.x);

        topRightY = Mathf.Max(topRightY, bottomLeftYCache + minSize.y);
        topRightX = Mathf.Max(topRightX, bottomLeftXCache + minSize.x);
        bottomLeftY = Mathf.Min(bottomLeftY, topRightYCache - minSize.y);
        bottomLeftX = Mathf.Min(bottomLeftX, topRightXCache - minSize.x);

        topRightYCache = topRightY;
        topRightXCache = topRightX;
        bottomLeftYCache = bottomLeftY;
        bottomLeftXCache = bottomLeftX;

        
        if(fixedCenter)
        {
            float maxCenterX = maxPosition.x - Globals.gameConstants.tileOffset.x;
            float maxCenterY = maxPosition.y - Globals.gameConstants.tileOffset.y;
            serializableData.centerPosition = new Vector2(Mathf.Min(serializableData.centerPosition.x, maxCenterX), Mathf.Min(serializableData.centerPosition.y, maxCenterY));

            float minCenterX = minPosition.x + Globals.gameConstants.tileOffset.x;
            float minCenterY = minPosition.y + Globals.gameConstants.tileOffset.y;
            serializableData.centerPosition = new Vector2(Mathf.Max(serializableData.centerPosition.x, minCenterX), Mathf.Max(serializableData.centerPosition.y, minCenterY));

            gizmo.transform.position = serializableData.centerPosition;
            
            if(inputNode != null)
                inputNode.transform.position = gizmo.transform.position;

            if(outputNode != null)
                outputNode.transform.position = gizmo.transform.position;

            if(railNodeInput != null)
                railNodeInput.transform.position = gizmo.transform.position;

            if(railNodeOutput != null)
                railNodeOutput.transform.position = gizmo.transform.position;
        }
        
        Vector2Int newTopRight = (new Vector2(topRightX, topRightY)).RoundVector2ToInt();
        Vector2Int newBottomLeft = (new Vector2(bottomLeftX, bottomLeftY)).FloorVector2ToInt();

        if(serializableData.topRight != newTopRight || serializableData.bottomLeft != newBottomLeft)
        {
            serializableData.topRight    = newTopRight;
            serializableData.bottomLeft  = newBottomLeft;

            UpdateCenterPosition();
            UpdateLineRenderer();
            LevelBuilder.instance.UpdateTiles();
            UpdateScaleGizmoPositions();
            UpdateContent();

            UpdateScaleGizmoSize();

            ShowScaleGizmos(true);

            if(On_EntityChangedSize != null)
                On_EntityChangedSize(this);
        }
    }

    public void UpdateScaleGizmoSize()
    {
        int width = GetWidth();
        int height = GetHeight();

        scaleGizmos[0].SetWidth(width);
        scaleGizmos[1].SetWidth(height);
        scaleGizmos[2].SetWidth(width);
        scaleGizmos[3].SetWidth(height);

        UpdateSizeGizmos(width, height);
    }

    public void UpdateSizeGizmos(int width, int height)
    {
        if(sizeGizmos.Length < 2)
            return;

        sizeGizmos[0].UpdateSize(height);
        sizeGizmos[1].UpdateSize(width);

        if(sizeGizmos.Length < 4)
            return;

        sizeGizmos[2].UpdateSize(width);
        sizeGizmos[3].UpdateSize(height);
    }

    public void SetPositon(Vector2 targetPosition)
    {
        int width = GetWidth();
        int height = GetHeight();

        Vector2  newCenterPosition;
        bool hasMoved = false;

        if(fixedCenter)
        {   
            newCenterPosition = targetPosition.FloorVector2ToInt() + Globals.gameConstants.tileOffset;
            if(newCenterPosition == cachedPosition)
                return;

            cachedPosition = newCenterPosition;

            Vector2 localBottomLeft = serializableData.bottomLeft - ((Vector2) gizmo.transform.position);
            Vector2 localTopRight = serializableData.topRight - ((Vector2) gizmo.transform.position);

            hasMoved = serializableData.centerPosition != newCenterPosition;

            serializableData.centerPosition = newCenterPosition;
            gizmo.transform.position = serializableData.centerPosition;

            serializableData.bottomLeft = ((Vector2)gizmo.transform.position + localBottomLeft).FloorVector2ToInt();
            serializableData.topRight = ((Vector2)gizmo.transform.position + localTopRight).FloorVector2ToInt();
        }
        else
        {
            Vector2Int tilePos;
            if(width % 2 == 0)
                tilePos = new Vector2Int(Mathf.RoundToInt(targetPosition.x), 0);
            else
                tilePos = new Vector2Int(Mathf.FloorToInt(targetPosition.x), 0);

            if(height % 2 == 0)
                tilePos = new Vector2Int(tilePos.x, Mathf.RoundToInt(targetPosition.y));
            else
                tilePos = new Vector2Int(tilePos.x, Mathf.FloorToInt(targetPosition.y));

            if(tilePos == cachedPosition)
                return;

            cachedPosition = tilePos;

            serializableData.bottomLeft = new Vector2Int(tilePos.x - Mathf.FloorToInt(width/2f), tilePos.y - Mathf.FloorToInt(height/2f));
            serializableData.topRight = serializableData.bottomLeft + new Vector2Int(width, height);

            newCenterPosition = new Vector2(Mathf.Lerp(serializableData.bottomLeft.x, serializableData.topRight.x, 0.5f), Mathf.Lerp(serializableData.bottomLeft.y, serializableData.topRight.y, 0.5f));
            hasMoved = serializableData.centerPosition != newCenterPosition;
        }

        //Reset Cache To prevent resize on drop!
        topRightYCache = int.MaxValue;
        topRightXCache = int.MaxValue;
        bottomLeftYCache = int.MinValue;
        bottomLeftXCache = int.MinValue;
       
        UpdateCenterPosition();
        UpdateLineRenderer();
        LevelBuilder.instance.UpdateTiles();
        UpdateScaleGizmoPositions();
        UpdateContent();

        if(hasMoved)
            if(LevelEntity.On_EntityMoved != null)
                LevelEntity.On_EntityMoved(this);
    }

    public void OnFinishedMoving()
    {
        if(LevelEntity.On_EntityStoppedMoving != null)
            LevelEntity.On_EntityStoppedMoving(this);

        Vector2 viewPortPos = Camera.main.WorldToViewportPoint(serializableData.centerPosition);
        if(DestroyArea.instance.PositionInsideDestroyArea(viewPortPos))
        {
            DestroySelf();
            LevelBuilder.instance.UpdateTiles();
            return;
        }

        UpdateSize();
        UpdateContent();
    }

    public int GetWidth()
    {
        return Mathf.RoundToInt(Mathf.Abs(serializableData.bottomLeft.x - serializableData.topRight.x));
    }

    public int GetHeight()
    {
        return Mathf.RoundToInt(Mathf.Abs(serializableData.bottomLeft.y - serializableData.topRight.y));
    }

    public int GetCost()
    {
        return GetWidth() * GetHeight();
    }

    public Vector2Int GetBottomLeft()
    {
        return serializableData.bottomLeft;
    }

    public Vector2Int GetTopRight()
    {
        return serializableData.topRight;
    }

    public Gizmo GetDragHandle()
    {
        return gizmo;
    }

    public Vector2 GetCenterPosition()
    {
        return serializableData.centerPosition;
    }

    public void SetInteractable(bool interactable, bool showDottedLine)
    {
        lineRenderer.enabled = (interactable || showDottedLine) && hasOutline;
        gizmo.gameObject.SetActive(interactable);

        lineRenderer.material = !interactable && showDottedLine ? dottedMaterial : defaultMaterial;

        dottedLineRenderer.enabled = interactable && !showDottedLine && hasOutline;

        isSelected = interactable;
    }

    public void ShowScaleGizmos(bool show)
    {
        isSelected = show;
        dottedLineRenderer.enabled = isSelected;
        UpdateDirectionGizmosAllowedToMove();
        

        foreach(ScaleGizmo gizmo in scaleGizmos)
        {
            gizmo.gameObject.SetActive(
                show && 
                directionsGizmosAllowedToMove.Contains(gizmo.scaleDirection) &&
                (
                (gizmo.scaleDirection == Direction4.Up && resizeUp) ||
                (gizmo.scaleDirection == Direction4.Right && resizeRight) ||
                (gizmo.scaleDirection == Direction4.Down && resizeDown) ||
                (gizmo.scaleDirection == Direction4.Left && resizeLeft)
                )
            );
        }

        ShowSizeGizmos(show);
    }

    public virtual void ShowSizeGizmos(bool show)
    {
        switch(allowedResizingDirections)
        {
            case AllowedResizingDirections.AllDirections:
                foreach(SizeGizmo gizmo in sizeGizmos)
                    gizmo.gameObject.SetActive(show);
            break;
            case AllowedResizingDirections.TwoDirections:
                foreach(SizeGizmo gizmo in sizeGizmos)
                {
                    //prioritize left and down.
                    bool mayShowLeft = !directionsGizmosAllowedToMove.Contains(Direction4.Left);
                    bool mayShowDown = !directionsGizmosAllowedToMove.Contains(Direction4.Down);

                    bool scaleGizmoShowing = GetScaleGizmoByDirection(gizmo.side).gameObject.activeInHierarchy;

                    bool active = show && !scaleGizmoShowing && !(gizmo.side == Direction4.Right && mayShowLeft) && !(gizmo.side == Direction4.Up && mayShowDown);

                    gizmo.gameObject.SetActive(active);
                }
            break;
            case AllowedResizingDirections.OneDirection:
                 foreach(SizeGizmo gizmo in sizeGizmos)
                {
                    //prioritize left and down.
                    bool mayShowLeft = !directionsGizmosAllowedToMove.Contains(Direction4.Left);
                    bool mayShowDown = !directionsGizmosAllowedToMove.Contains(Direction4.Down);

                    

                    gizmo.gameObject.SetActive(show && !directionsGizmosAllowedToMove.Contains(gizmo.side) && !(gizmo.side == Direction4.Right && mayShowLeft) && !(gizmo.side == Direction4.Up && mayShowDown));
                }
            break;
            default:
                foreach(SizeGizmo gizmo in sizeGizmos)
                    gizmo.gameObject.SetActive(false);
            break;
        }
        
    }

    private ScaleGizmo GetScaleGizmoByDirection(Direction4 direction)
    {
        switch(direction)
        {
            case Direction4.Up:
                return scaleGizmos[0];
            case Direction4.Right:
                return scaleGizmos[1];
            case Direction4.Down:
                return scaleGizmos[2];
            case Direction4.Left:
                return scaleGizmos[3];
        }
        Debug.LogError("Direction not found!");
        return null;
    }

    public void UpdateDirectionGizmosAllowedToMove()
    {
        directionsGizmosAllowedToMove = new List<Direction4>();

        switch(allowedResizingDirections)
        {
            case AllowedResizingDirections.AllDirections:

                for(int i = 0; i < 4; i++)
                    directionsGizmosAllowedToMove.Add((Direction4) i);

            break;
            case AllowedResizingDirections.TwoDirections:

                if(GetWidth() == GetHeight())
                {
                    for(int i = 0; i < 4; i++)
                        directionsGizmosAllowedToMove.Add((Direction4) i);
                }
                else if(GetWidth() > GetHeight())
                {
                    directionsGizmosAllowedToMove.Add(Direction4.Right);
                    directionsGizmosAllowedToMove.Add(Direction4.Left);
                }
                else
                {
                    directionsGizmosAllowedToMove.Add(Direction4.Up);
                    directionsGizmosAllowedToMove.Add(Direction4.Down);
                }

            break;
            case AllowedResizingDirections.OneDirection:
                
               
                if(GetWidth() == GetHeight())
                {
                    for(int i = 0; i < 4; i++)
                        directionsGizmosAllowedToMove.Add((Direction4) i);
                }
                else
                {
                    //Find gizmo furthest away from center!
                    ScaleGizmo furthestAwayFromCenter = null;
                    foreach(ScaleGizmo gizmo in scaleGizmos)
                    {
                        if(furthestAwayFromCenter == null || furthestAwayFromCenter.GetLength() < gizmo.GetLength())
                            furthestAwayFromCenter = gizmo;
                    }

                    directionsGizmosAllowedToMove.Add(furthestAwayFromCenter.scaleDirection);
                }
            break;
        }
    }

    public void SetColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        gizmo.SetColor(color);

        foreach(Gizmo g in scaleGizmos)
            g.SetColor(color);
        
        foreach(SizeGizmo g in sizeGizmos)
            g.SetColor(color);
    }

    public Color GetColor()
    {
        return lineRenderer.startColor;
    }

    private void On_TabChange(int tabIndex)
    {
        SetInteractable(tabIndex == (int)requiredTab, tabIndex == (int)requiredTab);

        ShowScaleGizmos(false);
    }

    public void OnTouchStart(TouchInfo touch)
    {
        if(On_EntitySelected != null)
            On_EntitySelected(this);
    }

    private void OnEntitySelected(LevelEntity levelEntity)
    {
        if(levelEntity == this)
            UpdateScaleGizmoPositions();

        ShowScaleGizmos(levelEntity == this);
    }

    private void Update()
    {
        if(isSelected)
        {
            dottedLineRenderer.material.SetTextureOffset("_MainTex", new Vector2(Time.time, 0f));
        }

        centerPosition = serializableData.centerPosition;
        topRight = serializableData.topRight;
        bottomLeft = serializableData.bottomLeft;
    }

    public Vector2 centerPosition;
    public Vector2Int bottomLeft;
    public Vector2Int topRight;

    private void On_LogicCanvasUpdate(bool visible)
    {
        if(visible || GameMaster.instance.IsPlaying())
        {
            ShowScaleGizmos(false);
            SetInteractable(false, false); 
        }
        else
        {  
            SetInteractable(Toolbar.instance.GetCurrentButtonIndex() == (int)requiredTab, Toolbar.instance.GetCurrentButtonIndex() == (int)requiredTab);
        }      
    }

    private void On_RailCanvasUpdate(bool visible)
    {
        if(visible || GameMaster.instance.IsPlaying())
        {
            ShowScaleGizmos(false);
            SetInteractable(false, false); 
        }
        else
        {  
            SetInteractable(Toolbar.instance.GetCurrentButtonIndex() == (int)requiredTab, Toolbar.instance.GetCurrentButtonIndex() == (int)requiredTab);
        } 
    }

    private void On_EnterPlayMode()
    {
        SetInteractable(false, false);
        foreach(ScaleGizmo scaleGizmo in scaleGizmos)
        {
            scaleGizmo.gameObject.SetActive(false);
        }

        foreach(SizeGizmo sizeGizmo in sizeGizmos)
        {
            sizeGizmo.gameObject.SetActive(false);
        }
    }

    private void On_ExitPlayMode()
    {
        SetInteractable(Toolbar.instance.GetCurrentButtonIndex() == (int)requiredTab, Toolbar.instance.GetCurrentButtonIndex() == (int)requiredTab);

        ShowScaleGizmos(false);
    }

    public void Subscribe()
    {
        if(gizmo != null)
            gizmo.OnTouchStart += OnTouchStart;

        Toolbar.On_TabChange += On_TabChange;
        LevelEntity.On_EntitySelected += OnEntitySelected;

        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;

        LogicCanvas.On_LogicCanvasUpdate += On_LogicCanvasUpdate;
        RailCanvas.On_RailCanvasUpdate += On_RailCanvasUpdate;
    }

    public void Unsubscribe()
    {
        if(gizmo != null)
            gizmo.OnTouchStart -= OnTouchStart;

        Toolbar.On_TabChange -= On_TabChange;
        LevelEntity.On_EntitySelected -= OnEntitySelected;

        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

        LogicCanvas.On_LogicCanvasUpdate -= On_LogicCanvasUpdate;
        RailCanvas.On_RailCanvasUpdate -= On_RailCanvasUpdate;
    }

    private void DestroyContent()
    {
        foreach(GameObject go in spawnedObjects)
        {
            Destroy(go);
        }
        spawnedObjects = new List<GameObject>();
    }

    private void OnDisable()
    {
        DestroyContent();

        if(LevelBuilder.instance != null)
            LevelBuilder.instance.UnRegisterLevelEntity(this);

        Unsubscribe();
    }

    private void OnDestroy()
    {
        DestroyContent();

        Unsubscribe();
    }

    public List<Link> GetAttachedLinks()
    {
        List<Link> result = new List<Link>();

        if(outputNode != null)
            result.AddRange(outputNode.links);

        if(inputNode != null)
            result.AddRange(inputNode.links);

        return result;
    }

    public List<RailLink> GetAttachedRailLinks()
    {
        List<RailLink> result = new List<RailLink>();

        if(railNodeOutput != null)
            result.AddRange(railNodeOutput.links);

        if(railNodeInput != null)
            result.AddRange(railNodeInput.links);

        return result;
    } 

    public SerializableData SerializeData()
    {
        serializableData.prefabId = id;

        if(outputNode != null)
            serializableData.connectedEntities = outputNode.GetConnectedEntities();

        return serializableData;
    }

    public SerializableData GetSerializableData()
	{
		return serializableData;
	}

	public void Deserialize(SerializableData serializableData)
	{  
		this.serializableData = serializableData;
        
        UpdateCenterPosition();
        UpdateLineRenderer();
        UpdateScaleGizmoPositions();
        UpdateContent();
        if(Toolbar.instance != null)
        {
            SetInteractable(Toolbar.instance.GetCurrentButtonIndex() == (int)requiredTab, Toolbar.instance.GetCurrentButtonIndex() == (int)requiredTab);
        }
        else
        {
            SetInteractable(false, false);
        }
       
        ShowScaleGizmos(false);
		UpdateSizeGizmos(GetWidth(), GetHeight());
	}

    public void DestroySelf()
    {
        if(On_DestroySelf != null)
            On_DestroySelf();

        Destroy(gameObject);
    }

    [System.Serializable]
	public class SerializableData
	{
        public string prefabId;
        [JsonIgnore] public Vector2 centerPosition{
            get
            {
                if (_centerPosition == null)
                    _centerPosition = new Position2D();

                return _centerPosition.ToVector2();
            }
            set
            {
                _centerPosition = new Position2D(value);
            }
        }
        [JsonProperty("centerPosition")] private Position2D _centerPosition;

        [JsonIgnore] public Vector2Int bottomLeft{
        //Unless we serialize vector2Int as vector2, deserialization does not work on iOS
            get{
                return _bottomLeft.ToVector2().RoundVector2ToInt();
            }
            set{
                _bottomLeft = new Position2D(value);
            }
        }
        [JsonIgnore] public Vector2Int topRight{
        //Unless we serialize vector2Int as vector2, deserialization does not work on iOS
            get{
                return _topRight.ToVector2().RoundVector2ToInt();
            }
            set{
                _topRight = new Position2D(value);
            }
        }
        [JsonProperty] private Position2D _bottomLeft;
        [JsonProperty] private Position2D _topRight;

        public string uniqueId {
            get{
                if(_instanceId == null)
                    _instanceId = System.Guid.NewGuid().ToString();

                return _instanceId;}
            set{_instanceId = value;}
        }
        private string _instanceId;
        public List<string> connectedEntities;
        
        public Direction4? gizmoDirection;
        public Rotation? gizmoRotation; 
        public Position2D gizmoOffset; 
        public int? gizmoIndex;

        public override string ToString()
        {
            return  "prefabId: " + prefabId + "\n" +
                    "centerPosition: " + centerPosition.ToString() + "\n" +
                    "bottomLeft: " + bottomLeft.ToString() + "\n" + 
                    "topRight: " + topRight.ToString() + "\n" +
                    "gizmoDirection: " + gizmoDirection + "\n" +
                    "gizmoRotation: " + gizmoRotation + "\n" + 
                    "gizmoPosition: " + gizmoRotation + "\n" +
                    "gizmoIndex: " + gizmoIndex;
        }
	}
}
