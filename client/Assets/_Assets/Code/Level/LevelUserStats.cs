using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUserStats
{
    public string LevelId { get; set; }
    public LevelOpinion Opinion { get; set; }
    public double bestTime { get; set; } = -1d;
}
