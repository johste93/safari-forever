using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyChallengeResponse
{
	public bool HasChampion { get; set; }
	public string CurrentLeaderNickname { get; set; }
	public string CurrentHighscore { get; set; }

	public LevelInfoDTO Level { get; set; }
}
