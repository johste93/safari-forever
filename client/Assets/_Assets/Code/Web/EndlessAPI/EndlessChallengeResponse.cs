using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EndlessChallengeResponse
{
    public JObject SerializedLevel;
    public JObject PublishedLevelMeta;
    public EndlessError Error;
    public byte[] thumbnail;

    public int PersonalScore { get; set; }
    
    public int RerollCost { get; set; }

    public Level ToLevel()
    {
        Level result = new Level{
            serializableLevel = SerializedLevel.ToObject<SerializableLevel>(),
            PublishedLevelMeta = PublishedLevelMeta.ToObject<PublishedLevelMeta>()
        };

        return result;
    }
}
