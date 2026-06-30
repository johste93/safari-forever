using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickCardResponse
{
    public CardError Error { get; set; }
    public bool DidWin { get; set; }
    public Hat Reward { get; set; }
}
