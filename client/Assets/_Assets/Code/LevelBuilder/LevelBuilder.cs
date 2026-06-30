using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using System.Linq;

public class LevelBuilder : Singleton<LevelBuilder>
{
    public List<SerializedEntity> serializedEntities;

    public GameObject tilePrefab;

    public BoxCollider2D[] edges;
    
    private Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();
    private List<LevelEntity> levelEntities = new List<LevelEntity>();
	private Dictionary<Spike, Vector2Int> spikes = new Dictionary<Spike, Vector2Int>();
    public List<List<LevelEntity.SerializableData>> rooms;

    public ColorPalette currentColorPalette;
    public SpriteRenderer background;

    private Vector2 offset = new Vector2(0.5f, 0.5f);

    public Sprite[] floorSprites;
    public Sprite[] patternSprites;

    private int floorSpriteIndex;
    private int patternSpriteIndex;
    private int musicIndex;

    public delegate void ColorPaletteEvent(ColorPalette newPalette);
    public static ColorPaletteEvent On_ColorPaletteChanged;

    public delegate void LevelBuilderEvent();
    public static LevelBuilderEvent On_LevelEntityRegistered;
    public static LevelBuilderEvent On_LevelEntityUnregistered;
    public static LevelBuilderEvent On_CostUpdate;
    public static LevelBuilderEvent On_AddedRoom;
    public static LevelBuilderEvent On_DeletedRoom;
    public static LevelBuilderEvent Before_ChangedRoom;
    public static LevelBuilderEvent On_ChangedRoom;
    public static LevelBuilderEvent On_RoomLoaded;

    private int cost;
    private int currentRoom = 0;

    public static int frameRoomLoaded;

    private void Awake()
    {
        rooms = new List<List<LevelEntity.SerializableData>>();

        for(int i = 0; i < 4; i++)
        {
            AddRoom();
        }
        SetRoomIndex(0);
        
        if(GlobalSingleton.colorPalette == null)
            GlobalSingleton.colorPalette = ColorGenerator.GetRandomColorPalette(true);
    }

    public void AddRoom()
    {
        rooms.Add(new List<LevelEntity.SerializableData>());

        SaveRoom();
        SetRoomIndex(rooms.Count-1);

        if(On_AddedRoom != null)
            On_AddedRoom();
    }

    public int GetNumberOfRooms()
    {
        return rooms.Count;
    }

    public int GetNumberOfRoomsFilled()
    {
        int result = 0;
        foreach(List<LevelEntity.SerializableData> room in rooms)
        {
            if(room.Count > 0)
                result++;
        }
        return result;
    }

    public int GetLastRoomIndex()
    {
        int result = 0;
        for(int i = rooms.Count-1; i >= 0; i--)
        {
            if(rooms[i].Count > 0)
            {
                result = i;
                break;
            }
        }

        if(result < currentRoom)
            result = currentRoom;

        return result;
    }

    public int GetFirstRoomIndex()
    {
        int result = 0;
        for(int i = 0; i < rooms.Count; i++)
        {
            if(rooms[i].Count > 0)
            {
                result = i;
                break;
            }
        }

        return result;
    }

    public int GetNextFilledRoomIndex()
    {
        if(currentRoom+1 >= 4)
        {
            Debug.LogError("This is the last room!");
            return currentRoom;
        }

        for(int i = currentRoom+1; i < 4; i++)
        {
            if(rooms[i].Count > 0)
                return i;
        }
        Debug.LogError("This is the last room!");
        return currentRoom;
    }

    public void DeleteRoom()
    {
        rooms.RemoveAt(currentRoom);
        
        if(currentRoom >= GetNumberOfRooms())
            currentRoom = GetNumberOfRooms()-1;

        SetRoomIndex(currentRoom);

        if(On_DeletedRoom != null)
            On_DeletedRoom();
    }

    public void OnEnable()
    {
#if !UNITY_WEBGL
        SetColorPalette(GlobalSingleton.colorPalette);
#else
        SetColorPalette(new ColorPalette());
#endif
        PositionEdges();

        SpawnTiles();        

        patternSpriteIndex = Random.Range(0, patternSprites.Length);

        musicIndex = Random.Range(2, Music.GetNames(typeof(Music)).Length);
        
        SetPatternSprite(patternSpriteIndex);

        floorSpriteIndex = Random.Range(0, floorSprites.Length);
        SetFloorSprite(floorSpriteIndex);
    }

    private void PositionEdges()
    {
        Vector2 halfSize = new Vector2(Globals.gameConstants.levelWidth, Globals.gameConstants.levelHeight)/2f;

        edges[0].offset = new Vector2(-halfSize.x-1 + offset.x, 0f);
        edges[0].size = new Vector2(1, Globals.gameConstants.levelHeight);

        edges[1].offset = new Vector2(0f, halfSize.y + offset.y);
        edges[1].size = new Vector2(Globals.gameConstants.levelWidth, 1);

        edges[2].offset = new Vector2(halfSize.x + offset.x, 0f);
        edges[2].size = new Vector2(1, Globals.gameConstants.levelHeight);

        edges[3].offset = new Vector2(0f, -halfSize.y-1+ offset.y);
        edges[3].size = new Vector2(Globals.gameConstants.levelWidth, 1);
    }

    private void SpawnTiles()
    {
        DestroyTiles();
        
        Vector2 halfSize = new Vector2(Globals.gameConstants.levelWidth, Globals.gameConstants.levelHeight)/2f;

        for(int x = 0; x < Globals.gameConstants.levelWidth; x++)
        {
            for(int y = -1; y < Globals.gameConstants.levelHeight; y++)
            {
                Vector2 spawnPos = new Vector2(x ,y) - halfSize + Globals.gameConstants.tileOffset;
                GameObject tileObj = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.On_ColorPaletteChanged(currentColorPalette);

                if(y == -1)
                {
                    tile.isBottomRow = true;
                    tile.boxCollider2D.enabled = true;
                }


                tiles.Add(spawnPos, tile);
            }
        }

        UpdateTiles();
    }

    private void DestroyTiles()
    {
        foreach(KeyValuePair<Vector2, Tile> tile in tiles)
        {
            Destroy(tile.Value.gameObject);
        }
        tiles = new Dictionary<Vector2, Tile>();
    }

    public void UpdateTiles()
    {
        int newCost = 0;

        //Count overlapping entities
        foreach(LevelEntity levelEntity in levelEntities)
        {
            newCost += levelEntity.GetCost();

            int modifier = 0;
            if(levelEntity.behaviour == LevelEntityBehaviour.Carve)
                modifier = 1;
            else if(levelEntity.behaviour == LevelEntityBehaviour.Fill)
                modifier = -1000;
			else if(levelEntity.behaviour == LevelEntityBehaviour.CarveOverride)
				 modifier = 10000;

            List<Vector2> positionInside = levelEntity.GetPositionsInside();
            List<Vector2> excludedTiles = levelEntity.GetExcludedTilePositionsInside();

            foreach(Vector2 position in positionInside)
            {
                if(tiles.ContainsKey(position))
                    tiles[position].tileModifier += modifier;

                if(excludedTiles.Contains(position) && tiles.ContainsKey(position))
                    tiles[position].tileModifier = -1000;                    
            }
        }

        foreach(KeyValuePair<Vector2, Tile> tile in tiles)
        {
            if(!tile.Value.isBottomRow)
            {
                tile.Value.pattern.enabled = tile.Value.tileModifier >= 1;
                tile.Value.background.enabled = tile.Value.tileModifier >= 1;
                tile.Value.boxCollider2D.enabled = tile.Value.tileModifier <= 0;
            }
            tile.Value.tileModifier = 0; //Reset tileModifier
        }

        //Update Collisionmap
        foreach(KeyValuePair<Vector2, Tile> tile in tiles)
        {
            tile.Value.UpdateTile();
        }

        UpdateTileMasks();

        if(newCost != cost)
        {
            cost = newCost;
            if(On_CostUpdate != null)
                On_CostUpdate();
        }
    }

    public void UpdateTileMasks()
    {
        foreach(KeyValuePair<Vector2, Tile> tile in tiles)
        {
            tile.Value.UpdateMask();
        }
    }
 
    public Tile GetTileAtPosition(Vector2 position)
    {
        if(tiles.ContainsKey(position))
            return tiles[position];

        return null;
    }

    public Vector4 GetRealLevelSize()
    {
        Vector2 halfSize = new Vector2(Globals.gameConstants.levelWidth, Globals.gameConstants.levelHeight)/2f;
        Vector2 mostBottomLeft = halfSize;
        Vector2 mostTopRight = -halfSize;

        if(levelEntities.Count == 0)
        { 
            mostBottomLeft = -halfSize;
            mostTopRight = halfSize;
        }
        else
        {
            foreach(LevelEntity entity in levelEntities)
            {
                if (entity.cameraScaleIgnore)
                    continue;

                if (entity.GetSerializableData().bottomLeft.x < mostBottomLeft.x)
                    mostBottomLeft.x = entity.GetSerializableData().bottomLeft.x;

                if(entity.GetSerializableData().bottomLeft.y < mostBottomLeft.y)
                    mostBottomLeft.y = entity.GetSerializableData().bottomLeft.y;

                if(entity.GetSerializableData().topRight.x > mostTopRight.x)
                    mostTopRight.x = entity.GetSerializableData().topRight.x;
                
                if(entity.GetSerializableData().topRight.y > mostTopRight.y)
                    mostTopRight.y = entity.GetSerializableData().topRight.y;

                if(entity.GetSerializableData().gizmoOffset != null)
                {
                    
                    float x = entity.gizmo.transform.position.x + entity.GetSerializableData().gizmoOffset.x;
                    float y = entity.gizmo.transform.position.y + entity.GetSerializableData().gizmoOffset.y;

                    if(x < mostBottomLeft.x)
                        mostBottomLeft.x = x;

                    if(y < mostBottomLeft.y)
                        mostBottomLeft.y = y;

                    if(x > mostTopRight.x)
                        mostTopRight.x = x;
                    
                    if(y > mostTopRight.y)
                        mostTopRight.y = y;
                }
            }
        }

        int levelWidth = Mathf.RoundToInt( Mathf.Abs(mostBottomLeft.x - mostTopRight.x));
        int levelHeight = Mathf.RoundToInt( Mathf.Abs(mostBottomLeft.y - mostTopRight.y));
        float centerX = Mathf.Lerp(mostBottomLeft.x, mostTopRight.x, 0.5f);
		float centerY = Mathf.Lerp(mostBottomLeft.y, mostTopRight.y, 0.5f);

        return new Vector4(levelWidth, levelHeight, centerX, centerY);
    }

    public bool IsLevelEmpty()
    {
        int entities = 0;
        for(int i = 0; i < rooms.Count; i++)
        {
            entities += rooms[i].Count;
        }

        return entities + levelEntities.Count == 0;
    }

    public bool IsRoomEmpty()
    {
        return levelEntities.Count == 0;
    }

	public void RegisterSpike(Spike spike)
	{
		spikes.Add(spike, spike.postion);
	}

	public void UnRegisterSpike(Spike spike)
	{
		if(spikes.ContainsKey(spike))
			spikes.Remove(spike);
	}

	public List<Vector2Int> GetSpikePositions()
	{
		return spikes.Values.ToList();
	}

    public void RegisterLevelEntity(LevelEntity levelEntity)
    {
        levelEntities.Add(levelEntity);

        if(On_LevelEntityRegistered != null)
            On_LevelEntityRegistered();
    }

    public void UnRegisterLevelEntity(LevelEntity levelEntity)
    {
        levelEntities.Remove(levelEntity);

        if(GlobalSingleton.isShuttingDown)
			return;

        if(SceneLoader.isLoadingScene)
			return;

        if(On_LevelEntityUnregistered != null)
            On_LevelEntityUnregistered();
    }

    public List<LevelEntity.SerializableData> GetSerializedEntities(int index)
    {
        return rooms[index];
    }

    public List<LevelEntity> GetLevelEntities()
    {
        return levelEntities;
    }

    public void SetColorPalette(ColorPalette palette)
    {
        if(SaveManager.currentSave.monoChromatic)
            palette = ColorGenerator.GetHighContrastMonoChromatic();

        currentColorPalette = palette;
        Camera.main.backgroundColor = currentColorPalette.main;
        background.color = currentColorPalette.main;

        if(On_ColorPaletteChanged != null)
            On_ColorPaletteChanged(currentColorPalette);
    }

    public void SetPatternSprite(int index)
    {
        patternSpriteIndex = index;
        foreach(KeyValuePair<Vector2, Tile> tile in tiles)
        {
            tile.Value.pattern.sprite = patternSprites[patternSpriteIndex];
        }
    }

    public void SetFloorSprite(int index)
    {
        floorSpriteIndex = index;
        foreach(KeyValuePair<Vector2, Tile> tile in tiles)
        {
            tile.Value.floor.sprite = floorSprites[floorSpriteIndex];
        }
    }

    public void SetMusic(Music music)
    {
        musicIndex = (int) music;
    }

    public void PlayMusic(bool crossFade = true)
    {
        MusicManager.Play((Music) musicIndex, crossFade, true);
    }

    public int GetPatternSpriteIndex()
    {
        return patternSpriteIndex;
    }

    public int GetFloorSpriteIndex()
    {
        return floorSpriteIndex;
    }

    public int GetMusicIndex()
    {
        return musicIndex;
    }

    public ColorPalette GetCurrentColors()
    {
        return currentColorPalette;
    }

    public void CreateNewLevel()
    {
        ClearRoom();

        //tiles = new Dictionary<Vector2, Tile>();
        currentRoom = 0;
        levelEntities = new List<LevelEntity>();
        rooms = null;

        Awake();
        
        patternSpriteIndex = Random.Range(0, patternSprites.Length);
        SetPatternSprite(patternSpriteIndex);

        musicIndex = Random.Range(2, Music.GetNames(typeof(Music)).Length);
        SetMusic((Music)musicIndex);
        
        floorSpriteIndex = Random.Range(0, floorSprites.Length);
        SetFloorSprite(floorSpriteIndex);

        SetColorPalette(ColorGenerator.GetRandomColorPalette(false));   

        SaveManager.currentWorkInProgressName = "";
    }

    public void ClearRoom()
    {
		//Delete all links first
		

        for(int i = levelEntities.Count-1; i >= 0; i--)
        {
            Destroy(levelEntities[i].gameObject);
        }
        UpdateTiles();
        UpdateTileMasks();
    }

    public int GetCost()
    {
        return cost;
    }

    public int GetCurrentRoomIndex()
    {
        return currentRoom;
    }

    public void SaveRoom()
    {
        rooms[currentRoom] = LevelSerializer.SerializeEntities(GetLevelEntities());
    }

    public void SetRoomIndex(int index)
    {
        if(Before_ChangedRoom != null)
            Before_ChangedRoom();

        ClearRoom();
        currentRoom = index;
        LevelSerializer.SpawnEntities(rooms[currentRoom]);

        frameRoomLoaded = Time.frameCount;

        if(On_ChangedRoom != null)
            On_ChangedRoom();
    }

    public void LoadSerializedRooms(List<List<LevelEntity.SerializableData>> rooms)
    {
        currentRoom = 0;
        this.rooms = rooms;
        if(On_AddedRoom != null)
            On_AddedRoom();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        GlobalSingleton.isShuttingDown = true;
    }
}
