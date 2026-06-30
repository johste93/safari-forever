using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelInfoDTO
{
    public string LevelId { get; set; }
    public string Name { get; set; }
    public string Creator { get; set; }
    public string CreatorUserId { get; set; }
    public string GameVersion { get; set; }
    public int CoinsInvested { get; set; }
    public int Plays { get; set; }
    public int RewardMultipler { get; set; }
    public bool CanBeBoosted { get; set; }
    public bool HasGraduated { get; set; }
    public int BoostedRank { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public byte[] Thumbnail { get; set; }
    public byte[] MiniThumbnail { get; set; }
    public Difficulty Difficulty { get; set; }
}
