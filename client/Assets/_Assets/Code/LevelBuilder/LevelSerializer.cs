using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Linq;

public class LevelSerializer
{
    public delegate void LevelSerializationEvent();
    public static LevelSerializationEvent On_LevelSaved;
    public static LevelSerializationEvent On_LevelLoaded;

    public static string Save(bool includeThumbnail)
    {
        SerializableLevel level = new SerializableLevel();
        level.gameVersion = Application.version;
        level.palette = LevelBuilder.instance.GetCurrentColors().GetSerializableData();
        level.floorSpriteIndex = LevelBuilder.instance.GetFloorSpriteIndex();
        level.patternSpriteIndex = LevelBuilder.instance.GetPatternSpriteIndex();
        level.musicIndex = LevelBuilder.instance.GetMusicIndex();

        LevelBuilder.instance.SaveRoom();
        for(int i = 0; i < LevelBuilder.instance.GetNumberOfRooms(); i++)
        {
            level.rooms.Add(LevelBuilder.instance.GetSerializedEntities(i));
        }

        string json = JsonConvert.SerializeObject(level);

        //System.IO.File.WriteAllText(Application.persistentDataPath + "/level.json",json);

        if(On_LevelSaved != null)
            On_LevelSaved();

        return json;
    }

    public static SerializableLevel DeserializeLevel(string json)
    {     
        SerializableLevel level = JsonConvert.DeserializeObject<SerializableLevel>(json);
        return level;
    }

    public static void Load(SerializableLevel level)
    {     
        LevelBuilder.instance.ClearRoom();

        LevelBuilder.instance.LoadSerializedRooms(level.rooms);
        LevelBuilder.instance.SetRoomIndex(LevelBuilder.instance.GetFirstRoomIndex());

        LevelBuilder.instance.SetPatternSprite(level.patternSpriteIndex);
        LevelBuilder.instance.SetFloorSprite(level.floorSpriteIndex);

        LevelBuilder.instance.SetMusic((Music)level.musicIndex);

        ColorPalette colorPalette = (ColorPalette) ScriptableObject.CreateInstance(typeof(ColorPalette));
        colorPalette.Deserialize(level.palette);
        LevelBuilder.instance.SetColorPalette(colorPalette);

        if(On_LevelLoaded != null)
            On_LevelLoaded();
    }

    public static List<LevelEntity.SerializableData> SerializeEntities(List<LevelEntity> entities)
    {
        List<LevelEntity.SerializableData> result = new List<LevelEntity.SerializableData>();
        foreach(LevelEntity entity in entities)
            result.Add(entity.SerializeData());

        return result;
    }

    public static void SpawnEntities(List<LevelEntity.SerializableData> entities)
    {
        foreach(LevelEntity.SerializableData serializableData in entities)
        {
            GameObject lastSpawn = GameObject.Instantiate (LevelEntityPrefabCatalog.GetPrefab(serializableData.prefabId), Vector3.zero, Quaternion.identity);
            LevelEntity levelEntity = lastSpawn.GetComponent<LevelEntity>();
            levelEntity.Deserialize(serializableData);
        }

        foreach(LevelEntity entity in LevelBuilder.instance.GetLevelEntities())
        {
            entity.outputNode?.Deserialize();
        }

        LevelBuilder.instance.UpdateTiles();
        LevelBuilder.instance.UpdateTileMasks();

        if(LevelBuilder.On_RoomLoaded != null)
            LevelBuilder.On_RoomLoaded();
    }
}
