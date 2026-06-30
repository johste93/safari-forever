using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatsResponse
{
    public float ClearRate { get; set; }
    public int Plays { get; set; }
    public int Likes { get; set; }
    public int Wins { get; set; }
    public int AvgJumps { get; set; }
    public int AvgDeaths { get; set; }
    public int TotalJumps { get; set; }
    public int TotalDeaths { get; set; }
    public string Record { get; set; }
    public string RecordHolderId { get; set; }
    public string RecordHolderNicknameAndIdentifier { get; set; }
    public Difficulty Difficulty { get; set; }
}
