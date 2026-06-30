using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsByUserResponse
{
    public string UserId { get; set; }
    public string Nickname { get; set; }
    public int LevelsPrPage { get; set; }
    public List<LevelInfoDTO> Levels { get; set; }
}
