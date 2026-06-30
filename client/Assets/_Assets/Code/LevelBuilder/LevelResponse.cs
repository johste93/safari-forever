using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LevelResponse
{
    public string id;
    public string name;
    public string creatorUserId;
    public string gameVersion;

    public int deaths = 0;
    public int wins = 0;
    public int likes = 0;

    public string levelJson;

    public SerializableLevel GetLevelDTO()
    {
        return JsonConvert.DeserializeObject<SerializableLevel>(levelJson);
    }
}
