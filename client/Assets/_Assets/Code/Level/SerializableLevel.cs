using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SerializableLevel
{
    public string gameVersion;
    public ColorPalette.SerializableData palette;
    public List<List<LevelEntity.SerializableData>> rooms;
    public int patternSpriteIndex;
    public int floorSpriteIndex;
    public int musicIndex;

    public SerializableLevel()
    {
        rooms = new List<List<LevelEntity.SerializableData>>();
    }

    public int GetNumberOfRooms()
    {
        return rooms.Where(x => x.Count > 0).Count();
    }
}
