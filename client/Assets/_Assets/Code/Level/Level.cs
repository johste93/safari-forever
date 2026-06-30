using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public SerializableLevel serializableLevel; //This is the recipe used to reconstruct the level
    public PublishedLevelMeta PublishedLevelMeta = null; //This remains null untill we download the level from the server
    public CampaignInfo campaignInfo = null; //This remains null untill level is loaded.
}

