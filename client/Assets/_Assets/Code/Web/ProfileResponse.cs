using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileResponse
{
    public string UserId { get; set; }
    public string Nickname { get; set; }
    public int Identifier { get; set; }
    public string Color { get; set; }
    public int FollowerCount { get; set; }
    public bool IsFollowed { get; set; }
    public List<LevelInfoDTO> Levels { get; set; }
	public bool AlphaAccount { get; set; } 
	public bool BetaAccount { get; set; } 
    public int Coins { get; set; }
    public string ShareUrl { get; set; }
    
    public int EndlessScore { get; set; }
    public int EndlessRank { get; set; }
    public int DailyChallengesWon { get; set; }
}
