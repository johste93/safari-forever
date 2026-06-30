using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CampaignLevel
{
	public bool beaten;
	public int seconds { get; set; } = -1;
	public int milliseconds { get; set; } = -1;

	[Obsolete] public double localHighscore = -1;
}
