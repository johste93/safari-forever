using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchLeaderboardResponse 
{
    public List<EndlessLeaderboardDTO> Leaderboard { get; set; }
    
    public int PersonalScore { get; set; }
    public int PersonalRank { get; set; }
}
