using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LevelDownloadResponse
{
    public JObject SerializedLevel;
    public JObject PublishedLevelMeta;

    public Level ToLevel()
    {
        Level result = new Level{
            serializableLevel = SerializedLevel.ToObject<SerializableLevel>(),
            PublishedLevelMeta = PublishedLevelMeta.ToObject<PublishedLevelMeta>()
        };

        //Debug.Log(result.PublishedLevelMeta.CreatorUserName);
        return result;
    }
}