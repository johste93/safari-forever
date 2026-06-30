using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignInfo 
{
    public CampaignInfo(World world, int index)
    {
		this.world = world;
        this.campaignIndex = index;
    }
	
	public World world;
    public int campaignIndex = -1;
}
